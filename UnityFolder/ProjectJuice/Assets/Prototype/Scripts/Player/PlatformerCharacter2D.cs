using System;
using System.Collections;
using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class PlatformerCharacter2D : ExtendedMonobehaviour
    {
        private IPlatformer2DUserControl m_Controller;

        //jump and double jump
        [Range(0,20)][SerializeField]private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
        [SerializeField]private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
        [SerializeField]private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
        [Range(0, 1)][SerializeField]private float m_AirSpeed = 0.25f;    // Percentage of speed in the air from controller input
        private bool m_HasDoubleJump = false;                               // Determines if has special second jump
        private bool m_UsedDoubleJump = false;
        [Range(-1, 0)][SerializeField]private float m_JumpDownYAxisTolerence = -0.5f;
        private Collider2D[] m_lastPassedThrough;
        [SerializeField] private bool m_UseForceToPassThrough = false;
        [Range(0,1)][SerializeField] private float m_percentOfJumpForcedUsed = 0.5f;

        //shooting slowdown
        [Range(0,1)][SerializeField] private float m_ShootingDelayToSlow = 0.25f;
        [Range(0, 1)][SerializeField]private float m_ShootingSpeed = 0.25f;
        private bool m_shootinDelayAdded = false;

        //dash
        private bool m_CanDash = false;                                     // Whether the player can dash. Restes on timer on ground, landing or double jump.
        [SerializeField]private ForceMode2D m_DashType = ForceMode2D.Force;
        [SerializeField]private float m_DashForce;
        [Range(0, 1)][SerializeField]private float m_DashDelay = 0.5f;      // Delay before dashing again
        private float m_DashTimer = 0.0f;
        [Range(0, 10)][SerializeField]private int m_DashDrag = 5;
        [Range(0, 1)][SerializeField]private float m_DashDragRemove = 0.12f;

        //grounding and ceiling
        [SerializeField]private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character
        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded

        public event EventHandler<BoolEventArgs> GroundedChanged; // Event fired when IsGrounded changes.
        private bool m_Grounded;            // Whether or not the player is grounded.
        public bool IsGrounded
        {
            get { return m_Grounded; }
            private set
            {
                if (m_Grounded != value)
                {
                    var e = new BoolEventArgs() {NewBoolValue = value, PreviousBoolValue = m_Grounded};
                    m_Grounded = value;
                    try
                    {
                        if (GroundedChanged != null) GroundedChanged(this, e);
                    }
                    catch (Exception ex)
                    {
                        ex.Log();
                        throw;
                    }
                }
            }
        }

        //private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        //[Range(0, 1)][SerializeField]private float m_TriggerResetDelay = 0.1f; // To reset the ground when passing through
        private bool m_IsPassing = false;
        //private bool m_ConfirmPassing = false;
        private bool m_ignoreCheckPassing = false;

        [SerializeField] private Animator m_Anim;            // Reference to the player's animator component.
        private Rigidbody2D m_Rigidbody2D;
        public GameObject m_Body;           //for sprite and animation
        private Collider2D[] m_MyColliders;
        private Collider2D[] m_LastPassThrough;
        private float m_Collidertimer = 1f;

        private bool m_CanFlip = true;
        private bool m_MeleeDownDashComplete = true;
        public bool MeleeDownDashComplete { get { return m_MeleeDownDashComplete; } }

        private DelayManager m_delayManager;

        [HideInInspector] public ParticleSystem m_JumpParticle;
        [HideInInspector] public ParticleSystem m_LandingParticle;
        [HideInInspector] public ParticleSystem m_DashBodyThrusters;
        [HideInInspector] public ParticleSystem m_GroundDashGrinding;
        [HideInInspector] public ParticleSystem m_DashChromaticAberation;
        [HideInInspector] public ParticleSystem m_DashParticle;
        [SerializeField] private GameObject m_BackThrusterPoint;
        [SerializeField] private GameObject m_FeetPoint;

        private Ground[] m_platforms;
 

        private void Awake()
        {
            // Setting up references.
            m_Controller = GetComponent<IPlatformer2DUserControl>();
            if (m_Controller == null) print("NO Platformer2DUserControls on player");
            m_GroundCheck = transform.FindChild("GroundCheck");
            m_delayManager = GetComponent<DelayManager>();
            //m_CeilingCheck = transform.FindChild("CeilingCheck");
            if(m_Anim == null)m_Anim = m_Body.GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_MyColliders = GetComponents<Collider2D>();
            m_platforms = FindObjectsOfType<Ground>();
        }

        void Start()
        {
            m_HasDoubleJump = CheckifHasDoubleJump();
        }

        private void Update()
        {
            if (GameManager.instance.IsPlaying)
            {
                CheckIfAbove();

                IsGrounded = SetGrounded(IsGrounded);

                // Set the vertical animation
                m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);


                Move(m_Controller.m_XAxis, m_Controller.m_Dash, m_Controller.m_Jump, m_Controller.m_Imobilize, m_Controller.m_Shoot, m_Controller.m_Special);
            }
        }

        public void Move(float move, bool dash, bool jump, bool imobile, bool isShooting, bool doubleJump)
        {
            //CheckPassThrough();

            if (m_DashTimer < Time.time && !m_CanDash && IsGrounded)
            {
                CheckDrag();
                m_CanDash = true;
                m_AirControl = true;
            }

            // Set whether or not the character is crouching in the animator
            //m_Anim.SetBool("Crouch", dash);

            //only control the player if grounded or airControl is turned on
            if ((IsGrounded || m_AirControl) && m_CanDash)
            {
                // Reduce the speed in air by the airSpeed multiplier
                float toMove = (!IsGrounded ? move * m_AirSpeed : move);

                if (isShooting && IsGrounded && !m_shootinDelayAdded)
                {
                    m_delayManager.AddOtherDelay(m_ShootingDelayToSlow);
                    m_shootinDelayAdded = true;
                }
                
                if (!isShooting && m_shootinDelayAdded) m_shootinDelayAdded = false;

                toMove = (isShooting && IsGrounded && m_delayManager.OtherReady ? move * m_ShootingSpeed : move);

                if (imobile && IsGrounded) toMove = 0;

                // The Speed animator parameter is set to the absolute value of the horizontal input.
                m_Anim.SetFloat("Speed", Mathf.Abs(toMove));

                // Move the character
                m_Rigidbody2D.velocity = new Vector2(toMove * m_MaxSpeed, m_Rigidbody2D.velocity.y);

                // If the input is moving the player right and the player is facing left...
                if ((move > 0 && !m_Controller.m_FacingRight) || (move < 0 && m_Controller.m_FacingRight)) Flip(); // ... flip the player.

            }

            // If the player should jump...
            if (m_HasDoubleJump && !IsGrounded && !m_UsedDoubleJump) jump = jump || doubleJump;
            if (jump) Jump();

            // If the player should dash
            if (m_CanDash && dash)
                PhysicsDash();
        }

        private void Flip()
        {
            if (m_CanFlip)
            {
                // Switch the way the player is labelled as facing.
                m_Controller.m_FacingRight = !m_Controller.m_FacingRight;

                // Multiply the player's x local scale by -1.
                Vector3 theScale = m_Body.transform.localScale;
                theScale.x *= -1;
                m_Body.transform.localScale = theScale;
            }
        }

        public void SpawnFlip()
        {
            Flip();
        }

        public void ChangeCanFlip()
        {
            m_CanFlip = !m_CanFlip;
        }

        private void Jump()
        {
            if (IsGrounded && m_Anim.GetBool("Ground"))
            {
                IsGrounded = false;
                m_Anim.SetBool("Ground", false);
                m_Rigidbody2D.velocity = Vector2.zero;
                if (m_Controller.m_YAxis < m_JumpDownYAxisTolerence)
                {
                    PassDownThroughWithJump();
                }
                else
                {
                    // Add a vertical force to the player.

                    m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
                    SoundManager.PlaySFX(Database.instance.Jump);
                    InstatiateParticle(m_JumpParticle, m_FeetPoint);
                }
            }
            else if (m_HasDoubleJump && !IsGrounded && !m_UsedDoubleJump)
            {
                CheckDrag();
                m_UsedDoubleJump = true;
                if (!m_CanDash) m_CanDash = !m_CanDash;
                m_Rigidbody2D.velocity = Vector2.zero;
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
                SoundManager.PlaySFX(Database.instance.Jump);
                InstatiateParticle(m_JumpParticle, m_FeetPoint);
            }
        }

        public void SpawnCheckGround()
        {
            IsGrounded = true;
        }

        private bool SetGrounded(bool previousGround)
        {

            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.


            bool isGrounded = false;


            Collider2D[] colliders = GetGroundColliders();
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject.layer != gameObject.layer)
                    isGrounded = true;
            }
            m_Anim.SetBool("Ground", isGrounded);

            if (isGrounded)
            {
                if (!m_AirControl)
                    m_AirControl = true;

                if(m_UsedDoubleJump)
                    m_UsedDoubleJump = false;

                if (m_CanDash)
                    CheckDrag();


                if (!m_MeleeDownDashComplete)
                    m_MeleeDownDashComplete = true;

                if (!IsGrounded && m_Rigidbody2D.velocity.y < 0.1f)
                {
                    SoundManager.PlaySFX(Database.instance.Landing);
                    InstatiateParticle(m_LandingParticle, m_FeetPoint);
                }

                //if (m_ignoreCheckPassing) ResetPassthrough();
            }


            return isGrounded;
        }

        private Collider2D[] GetGroundColliders()
        {
            return Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        }

        public void PhysicsDash()
        {
            if (m_CanDash)
            {

                SoundManager.PlaySFX(Database.instance.Dash);
                InstatiateParticle(m_DashBodyThrusters, m_BackThrusterPoint, true);
                InstatiateParticle(m_DashChromaticAberation, gameObject, true);

                if (IsGrounded)
                {
                    SoundManager.PlaySFX(Database.instance.DashMetalGrind);
                    InstatiateParticle(m_GroundDashGrinding, m_FeetPoint, true);

                }

                m_DashTimer = float.MaxValue;
                m_CanDash = false;
                m_AirControl = false;
                m_Rigidbody2D.drag = m_DashDrag;

                Vector2 angle = new Vector2(m_Controller.m_XAxis, m_Controller.m_YAxis); //get dash angle from x axis

                //if no x input set direction to horizontal
                if (m_Controller.m_XAxis <= 0.1f && m_Controller.m_XAxis >= -0.1f && m_Controller.m_YAxis <= 0.1f && m_Controller.m_YAxis >= -0.1f)
                {
                    angle.x = 1f;
                    angle.y = 0f;
                }

                // if grounded force y to positive
                if (IsGrounded && angle.y < 0) angle.y = 0f;

                // if not facing right force x negative
                if (!m_Controller.m_FacingRight && angle.x > 0) angle.x = -angle.x;

                //print(angle);

                // normalize and add impulse value
                angle = angle.normalized * m_DashForce;
                m_Rigidbody2D.velocity = Vector2.zero;
                m_Rigidbody2D.AddForce(angle, m_DashType);

                //set values for cooldown
                m_DashTimer = Time.time + m_DashDelay;
                StartCoroutine(DashDragReset(m_DashDragRemove, m_Rigidbody2D));
            }
        }

        public void PhysicsDashForMeleeAbility(float force)
        {
            if (m_CanDash)
            {
                m_MeleeDownDashComplete = false;
                m_DashTimer = float.MaxValue;
                m_CanDash = false;
                m_AirControl = false;
                m_Rigidbody2D.drag = m_DashDrag;

                Vector2 angle = new Vector2(0,-1); //get dash angle from x axis

                // normalize and add impulse value
                angle = angle.normalized * force;
                m_Rigidbody2D.velocity = Vector2.zero;
                m_Rigidbody2D.AddForce(angle, m_DashType);

                //set values for cooldown
                m_DashTimer = Time.time + m_DashDelay;
                StartCoroutine(DashDragReset(m_DashDragRemove, m_Rigidbody2D));
            }
        }

        public void AddKnockBack(IDamaging impact)
        {
            if (m_CanDash)
            {
                m_DashTimer = float.MaxValue;
                m_CanDash = false;
                m_AirControl = false;
                m_Rigidbody2D.drag = impact.ImpactForceSettings.ImpactDrag;

                Vector2 angle = impact.ImpactForceSettings.ImpactAngle;

                // if grounded force y to positive
                if (IsGrounded && angle.y < 0) angle.y = 0f;

                // if not facing right force x negative
                //if (!m_Controller.m_FacingRight && angle.x > 0) angle.x = -angle.x;
                if (impact.ImpactForceSettings.DirectionComingForm == Direction2D.Right) angle.x = -angle.x;


                //print(angle);

                // normalize and add impulse value
                angle = angle.normalized * impact.ImpactForceSettings.ImpactForce;
                m_Rigidbody2D.velocity = Vector2.zero;
                m_Rigidbody2D.AddForce(angle, m_DashType);

                //set values for cooldown
                m_DashTimer = Time.time + impact.ImpactForceSettings.ImpactDragTimer;
                StartCoroutine(DashDragReset(m_DashDragRemove, m_Rigidbody2D));
            }
        }

        private void CheckDrag()
        {
            if (m_Rigidbody2D.drag != 0) m_Rigidbody2D.drag = 0;
        }

        private bool CheckifHasDoubleJump()
        {
            bool hasDoubleJump = false;

            if (m_Controller.m_PlayerData.PlayerAbility == Abilities.DoubleJump)
                hasDoubleJump = true;

            return hasDoubleJump;

        }

        IEnumerator DashDragReset()
        {
            yield return new WaitForSeconds(m_DashDragRemove);
            CheckDrag();
        }
        
        public void CheckPassThrough(Collider2D collider)
        {
            Ground ground = collider.GetComponent<Ground>();
            //CheckLastPassedThrough();

            if (!m_IsPassing)
            {
                
                if (ground != null && ground.IsPassThrough)
                {
                    foreach (Collider2D gc2d in ground.Colliders)
                    {
                        foreach (Collider2D myC2d in m_MyColliders)
                        {
                            Physics2D.IgnoreCollision(gc2d, myC2d, true);
                        }
                    }
                    m_LastPassThrough = ground.Colliders;
                    m_IsPassing = true;
                    StartCoroutine(TimedCheckColliders(m_Collidertimer));
                }
            }
        }

        public void CheckifFeetPassed(Collider2D collider)
        {
            if (m_IsPassing)
            {

                Ground ground = collider.GetComponent<Ground>();
                if (ground != null && ground.IsPassThrough)
                {
                    foreach (Collider2D gc2d in ground.Colliders)
                    {
                        foreach (Collider2D myC2d in m_MyColliders)
                        {
                            Physics2D.IgnoreCollision(gc2d, myC2d, false);
                        }
                    }
                }

                m_IsPassing = false;
            }
        }

        private void CheckLastPassedThrough()
        {
            if (m_LastPassThrough != null)
            {
                foreach (Collider2D myc2d in m_MyColliders)
                {
                    foreach (Collider2D c2d in m_LastPassThrough)
                    {
                        if (Physics2D.GetIgnoreCollision(myc2d, c2d))
                            Physics2D.IgnoreCollision(myc2d, c2d, false);
                    }
                }
                m_LastPassThrough = null;
                m_IsPassing = false;
            }

            
        }

        IEnumerator TimedCheckColliders(float timer)
        {
            yield return new WaitForSeconds(timer);
            CheckLastPassedThrough();
        }

        public void FlushAnimState()
        {
            m_Anim.SetFloat("Speed", 0f);
        }

        private void CheckIfAbove()
        {
            if (!m_ignoreCheckPassing)
            {
                foreach (Ground g in m_platforms)
                {
                    if (g.IsPassThrough)
                    {
                        EdgeCollider2D ec = g.GetComponent<EdgeCollider2D>();
                        if (ec != null)
                        {
                            if (ec.transform.position.y > m_GroundCheck.transform.position.y)
                            {
                                IgnoreCollision(ec, true);
                            }
                            else
                            {
                                IgnoreCollision(ec, false);
                            }
                        }
                    }
                }
            }
        }

        private void IgnoreCollision(Collider2D col1, bool isIgnore = false)
        {
            foreach (Collider2D myc2d in m_MyColliders)
            {
                Physics2D.IgnoreCollision(col1, myc2d, isIgnore);
            }
        }

        private void PassDownThroughWithJump()
        {
            Collider2D[] colls = GetGroundColliders();

            foreach(Collider2D col in colls)
            {
                Ground gr = col.GetComponent<Ground>();
                if (gr != null && gr.IsPassThrough)
                {
                    EdgeCollider2D ec = col.GetComponent<EdgeCollider2D>();
                    if (ec != null)
                    {
                        IgnoreCollision(ec, true);
                        m_ignoreCheckPassing = true;
                    }
                }
            }
            if (m_ignoreCheckPassing)
            {
                m_LastPassThrough = colls;
                if(m_UseForceToPassThrough)
                    m_Rigidbody2D.AddForce(new Vector2(0f, -m_JumpForce * m_percentOfJumpForcedUsed));
            }
        }

        public void ResetPassthrough()
        {
            if (m_ignoreCheckPassing)
            {
                foreach (Collider2D col in m_LastPassThrough)
                {
                    Ground gr = col.GetComponent<Ground>();
                    if (gr != null && gr.IsPassThrough)
                    {
                        EdgeCollider2D ec = col.GetComponent<EdgeCollider2D>();
                        if (ec != null)
                        {
                            IgnoreCollision(ec, false);
                        }
                        m_ignoreCheckPassing = false;
                    }
                }
            }
        }

        public event EventHandler PlayerDashed;
        protected void OnPlayerDashed()
        {
            if (PlayerDashed != null) PlayerDashed(this, EventArgs.Empty);
        }

    }
}
