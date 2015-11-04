using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

public class MeleeAttack : ExtendedMonobehaviour
{
    public IPlatformer2DUserControl InputManager { get { return _inputManager; } }
    private IPlatformer2DUserControl _inputManager;
    public PlatformerCharacter2D MovementManager { get { return _mouvementManager; } }
    private PlatformerCharacter2D _mouvementManager;

    [SerializeField] private bool _isAbility = false; //for spawner
    public bool isAbility { get { return _isAbility; } }
    [SerializeField] private bool _abilityHasSpike = false;
    [SerializeField] private bool _abilityAerialCancelOnGround = true;

    #region Phil's refactor
    [SerializeField] private Animator m_animator;
    private bool m_isSwinging = false;
    public bool isSwinging { get { return m_isSwinging; } }
    [SerializeField] private Collider2D _collider;
    #endregion

    [SerializeField] private GameObject _ParticleReference;
    [Range(0,5)][SerializeField] private float _trailGroundLifeTime = 1f;
    [Range(0,5)][SerializeField] private float _trailAerialLifeTime = 2f;

    private bool isGroundedAtStart = false;

    [SerializeField] private DamageType _damageType = DamageType.Melee;
    public DamageType TypeOfDamage { get { return _damageType; } }

    [SerializeField] private float _DashForce = 450f;

    private bool _completedAerialAttack = true;

    [SerializeField] private GameObject _swingerCollider;
    [SerializeField] private GameObject _flipReference;
    [SerializeField] private DelayManager _delayManager;
    [SerializeField] private bool _addImpactForce = true;
    [Range(0,4)][SerializeField] private float _FlashTimerOnScreen = 2f;

    public ImpactForceSettings _impactForceSettings;
    
    
    public float rotationSpeed = 100f;
    public float startingRotation = -45;
    public float endingRotation = 45;
    [Range(0,3)]public float _delayAfterSwing = 0.5f;

    [HideInInspector] public string Swipe;
    [HideInInspector] public string PlayerImpact;
    [HideInInspector] public string Sheath;
    [HideInInspector] public string Clash;
    [HideInInspector] public string ClashCrowd;
    [HideInInspector] public string ClashAftermath;
    [HideInInspector] public string AbilitySecondSound;
    [HideInInspector] public string AbilityAerial;
    private AudioSource _sound;

    [HideInInspector] public ParticleSystem Trail;
    [HideInInspector] public ParticleSystem ClashingParticle;
    [HideInInspector] public ParticleSystem CrowdParticle;
    private ParticleSystem InstantiatedTrail;

    private LightFeedbackTemp _lightFeedback;

    // Use this for initialization
    private void Start()
    {
        if (_delayManager == null) _delayManager = GetComponent<DelayManager>();
        if (_inputManager == null) _inputManager = GetComponent<IPlatformer2DUserControl>();
        if (_mouvementManager == null) _mouvementManager = GetComponent<PlatformerCharacter2D>();
        //_swingerCollider.gameObject.SetRotationEulerZ(startingRotation);
        if (_collider == null) Debug.LogError(ColliderString());
        _lightFeedback = GetComponent<LightFeedbackTemp>();
        _lightFeedback.LightDone += MeleeTimerReset;
    }

    private void MeleeTimerReset(object sender, EventArgs e)
    {
        _delayManager.SetDelay(0);
    }

    private string ColliderString()
    {
        string ret = "";

        if (_collider == null && isAbility) ret = "Battle Axe missing assigned colliders in inspector!";
        else if (_collider == null && !isAbility) ret = "Knife missing assigned colliders in inspector!";

        return ret;
    }

    // Update is called once per frame
    void Update()
    {

        if (_delayManager.CanShoot && !m_isSwinging && _inputManager.m_Melee) //Battle Axe Ability swinging!
        {
            if (m_animator == null)
            {
                //legacy swing done by code
                _swingingAnimation = LegacyStartSwingingAnimation();
                StartCoroutine(_swingingAnimation);
            }
            else
            {
                //new Animated swing
                m_isSwinging = StartAnimatedSwing();
            }
            
        }

        #region FacingStuff
        if (_inputManager.m_FacingRight)
        {
            //_flipReference.transform.localScale = _flipReference.transform.localScale.SetX(1);
        }
        else
        {
            //_flipReference.transform.localScale = _flipReference.transform.localScale.SetX(-1);
        }
        #endregion

        _impactForceSettings.DirectionComingForm = _inputManager.m_FacingRight ? Direction2D.Left : Direction2D.Right;

        /*
        if (!_isSwingingAnimationOnGoing && _completedAerialAttack && _mouvementManager.MeleeDownDashComplete && _swingerCollider.activeInHierarchy)
            _swingerCollider.SetActive(false);
            */

        if (_sound != null && _sound.isPlaying && !isGroundedAtStart && _mouvementManager.IsGrounded)
            StopAerialSwingOnLand();
    }

    private bool StartAnimatedSwing()
    {
        if (!_collider.enabled) _collider.enabled = true;
        isGroundedAtStart = _mouvementManager.IsGrounded;
        if (isAbility)
        {
            _mouvementManager.ChangeCanFlip();
            if (_abilityHasSpike && !isGroundedAtStart)
                _completedAerialAttack = false;

            if (!isGroundedAtStart)
            {
                _sound = SoundManager.PlaySFX(AbilityAerial);
                m_animator.SetBool("Air", true);
            }
            else
            {
                _sound = SoundManager.PlaySFX(Swipe);
                m_animator.SetBool("Grounded", true);
            }
        }
        else
        {
            _sound = SoundManager.PlaySFX(Swipe);
            m_animator.SetBool("Grounded", true);
        }

        return true;
    }

    private void StopAerialSwingOnLand()
    {
        if (isAbility && _abilityAerialCancelOnGround)
        {
            _sound.Stop();
            ResetSwing("Air", false);
        }
    }

    public void ResetSwing(string change, bool with)
    {
        _collider.enabled = true;
        m_isSwinging = false;
        m_animator.SetBool(change, with);
        _delayManager.AddDelay(_delayAfterSwing);
        _wasConsumed = false;
    }

    public void StartTrail()
    {
        float timer = _trailAerialLifeTime;
        if (isAbility && isGroundedAtStart) timer = _trailGroundLifeTime;

        InstatiateParticle(Trail, _ParticleReference, true, timer);
    }


    private bool _isSwingingAnimationOnGoing = false;
    private IEnumerator _swingingAnimation;

    private IEnumerator LegacyStartSwingingAnimation()
    {
        /*
        if (!_collider.enabled) _collider.enabled = true;
        bool isGroundedAtStart = _mouvementManager.IsGrounded;
        if (isAbility)
        {
            _mouvementManager.ChangeCanFlip();
            if (_abilityHasSpike && !isGroundedAtStart)
                _completedAerialAttack = false;

            if(!isGroundedAtStart) SoundManager.PlaySFX(AbilityAerial);
            else SoundManager.PlaySFX(Swipe);
        }
        else
        {
            SoundManager.PlaySFX(Swipe);
        }


        _delayManager.AddDelay(100f);
        _isSwingingAnimationOnGoing = true;
        _swingerCollider.SetActive(true);
        yield return null;
        while (_swingerCollider.transform.rotation.eulerAngles.z.ToNormalizedAngle() <= endingRotation)
        {
            _swingerCollider.transform.Rotate(Vector3.forward * Time.deltaTime * rotationSpeed);

            yield return null;
        }
        _swingerCollider.SetActive(false);
        yield return null;
        _swingerCollider.gameObject.SetRotationEulerZ(startingRotation);

        if (isAbility)
        {
            _mouvementManager.ChangeCanFlip();
            if(_abilityHasSpike && !isGroundedAtStart)
            {
                _completedAerialAttack = true;
                _swingerCollider.SetActive(true);
                _mouvementManager.PhysicsDashForMeleeAbility(_DashForce);
            }
            /*
            else
                SoundManager.PlaySFX(AbilitySecondSound);
                
        }


        yield return new WaitForSeconds(_delayAfterSwing);
        _wasConsumedDuringThisAnimation = false;
        _isSwingingAnimationOnGoing = false;
        SoundManager.PlaySFX(Sheath);
        _lightFeedback.StartLightFeedback(0);
        */
        Debug.LogWarning("In Legacy swing, ignoring!");
        yield return 0;
    }


    private bool _wasConsumed = false;
    private List<HPScript> _immuneTargets = new List<HPScript>();


    public bool IsAvailableForConsumption
    {
        get {
            
            if(m_animator == null)
                return _isSwingingAnimationOnGoing && !_wasConsumed;
            else
               return !_wasConsumed;
        }
    }

    public void Consumed()
    {
        _collider.enabled = false;
        _wasConsumed = true;

    }

    public float Damage
    {
        get {
            if (isAbility)
                return Database.instance.MeleeAbilityDamage;
            else
                return Database.instance.MeleeBaseDamage;
        }
    }

    public bool AddImpactForce
    {
        get { return _addImpactForce; }
        private set { _addImpactForce = value; }
    }

    public IEnumerable<HPScript> ImmuneTargets { get { return _immuneTargets; } }
    public bool HasImmuneTargets { get { return false; }}
    public ImpactForceSettings ImpactForceSettings { get { return _impactForceSettings; } }

    public event EventHandler MeleeClashed;

    protected virtual void OnMeleeClashed()
    {
        try
        {
            EventHandler handler = MeleeClashed;
            if (handler != null) handler(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            ex.Log();
            throw;
        }
    }

    public void ClashedWithOtherMelee(MeleeDamagingCollider otherMelee)
    {
        _mouvementManager.AddKnockBack(otherMelee);
        otherMelee.Consumed();
        OnMeleeClashed();
        ClashFX(otherMelee);
    }

    private void PlayClashSFX()
    {

        SoundManager.PlaySFX(Clash);
        SoundManager.PlaySFX(ClashCrowd);
        SoundManager.PlaySFX(ClashAftermath);
    }

    private void ClashFX(MeleeDamagingCollider otherMelee)
    {
        PlayClashSFX();
        GameObject toParent = gameObject;
        if (otherMelee.HasPreferredImpactPoint)
        {
            toParent = otherMelee.gameObject;
        }
        InstatiateParticle(ClashingParticle, toParent, true);
        InstatiateParticle(CrowdParticle, toParent, false, _FlashTimerOnScreen);
    }
}
