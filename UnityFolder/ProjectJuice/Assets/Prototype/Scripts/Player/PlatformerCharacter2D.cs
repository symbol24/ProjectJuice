using System;
using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class PlatformerCharacter2D : MonoBehaviour
    {
        private IPlatformer2DUserControl m_Controller;

        [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
        [SerializeField] private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
        [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
        [Range(0, 1)] [SerializeField] private float m_AirSpeed = 0.25f;    // Percentage of speed in the air from controller input
        private bool m_HasDoubleJump = false;                               // Determines if has special second jump
        private bool m_CanDash = false;                                     // Whether the player can dash. Restes on timer on ground, landing or double jump.
        [SerializeField] private ForceMode2D m_DashType = ForceMode2D.Force;
        [SerializeField] private float m_DashImpulse;                       // Force of dash impulse
        [SerializeField] private float m_DashDelay = 0.5f;                  // Delay before dashing again
        private float m_DashTimer = 0.0f;

        [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character
        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
        private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up

        private Animator m_Anim;            // Reference to the player's animator component.
        private Rigidbody2D m_Rigidbody2D;
        public GameObject m_Body;           //for sprite and animation

        private void Awake()
        {
            // Setting up references.
            m_Controller = GetComponent<IPlatformer2DUserControl>();
            if (m_Controller == null) print("NO Platformer2DUserControls on player");
            m_GroundCheck = transform.FindChild("GroundCheck");
            m_CeilingCheck = transform.FindChild("CeilingCheck");
            m_Anim = m_Body.GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            m_Grounded = SetGrounded();

            // Set the vertical animation
            m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);

            Move(m_Controller.m_XAxis, m_Controller.m_Dash, m_Controller.m_Jump);
        }

        public void Move(float move, bool dash, bool jump)
        {
            // If crouching, check to see if the character can stand up
            if (!dash && m_Anim.GetBool("Crouch"))
            {
                // If the character has a ceiling preventing them from standing up, keep them crouching
                if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
                {
                    dash = true;
                }
            }

            if (m_DashTimer < Time.time && !m_CanDash)
            {
                m_CanDash = true;
                m_AirControl = true;
            }

            // Set whether or not the character is crouching in the animator
            //m_Anim.SetBool("Crouch", dash);

            //only control the player if grounded or airControl is turned on
            if ((m_Grounded || m_AirControl) && m_CanDash)
            {
                // Reduce the speed in air by the airSpeed multiplier
                move = (!m_Grounded ? move * m_AirSpeed : move);

                // The Speed animator parameter is set to the absolute value of the horizontal input.
                m_Anim.SetFloat("Speed", Mathf.Abs(move));

                // Move the character
                m_Rigidbody2D.velocity = new Vector2(move*m_MaxSpeed, m_Rigidbody2D.velocity.y);

                // If the input is moving the player right and the player is facing left...
                if ((move > 0 && !m_Controller.m_FacingRight) || (move < 0 && m_Controller.m_FacingRight)) Flip(); // ... flip the player.

            }

            // If the player should jump...
            if (m_Grounded && jump && m_Anim.GetBool("Ground")) Jump();

            // If the player should dash
            if (m_CanDash && dash) Dash();
        }


        private void Flip()
        {
            // Switch the way the player is labelled as facing.
            m_Controller.m_FacingRight = !m_Controller.m_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = m_Body.transform.localScale;
            theScale.x *= -1;
            m_Body.transform.localScale = theScale;
        }

        private void Dash()
        {
            if (m_CanDash)
            {
                // Add a directional force to the player as per input.
                m_CanDash = false;

                Vector2 angle = new Vector2(m_Controller.m_XAxis, m_Controller.m_YAxis); //get dash angle from x axis

                //if no x input set direction to horizontal
                if (m_Controller.m_XAxis <= 0.1f && m_Controller.m_XAxis >= -0.1f && m_Controller.m_YAxis <= 0.1f && m_Controller.m_YAxis >= -0.1f)
                {
                    angle.x = 1f;
                    angle.y = 0f;
                }

                // if grounded force y to positive
                if (m_Grounded && angle.y < 0) angle.y = -angle.y;

                // if not facing right force x negative
                if (!m_Controller.m_FacingRight && angle.x > 0) angle.x = -angle.x;

                // normalize and add impulse value
                angle = angle.normalized * m_DashImpulse;

                //add the force as force
                m_Rigidbody2D.AddForce(angle, ForceMode2D.Force);

                //set values for cooldown
                m_AirControl = false;
                m_DashTimer = Time.time + m_DashDelay;
            }
        }

        private void Jump()
        {
            // Add a vertical force to the player.
            m_Grounded = false;
            m_Anim.SetBool("Ground", false);
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }

        private bool SetGrounded()
        {

            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.


            bool isGrounded = false;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                    isGrounded = true;
            }
            m_Anim.SetBool("Ground", isGrounded);

            if (isGrounded)
            {
                //m_CanDash = true;
                m_AirControl = true;
            }

            return isGrounded;
        }
    }
}
