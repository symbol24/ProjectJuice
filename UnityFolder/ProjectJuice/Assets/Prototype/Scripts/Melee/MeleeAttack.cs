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

    private List<HPScript> _immuneTargets = new List<HPScript>();

    #region Ability info
    [SerializeField] private bool _isAbility = false; //for spawner
    public bool isAbility { get { return _isAbility; } }

    [SerializeField] private bool _abilityAerialCancelOnGround = true;

    private bool _isAerial = false;
    private bool _SwingReset = false;

    [SerializeField] private bool _abilityUseDifferentAerialDmg = true;
    public bool UsedDifferentAeralDmg { get { return _abilityUseDifferentAerialDmg; } }

    #endregion

    [SerializeField] private Animator m_animator;
    private bool m_isSwinging = false;
    public bool isSwinging { get { return m_isSwinging; } }
    [SerializeField] private Collider2D _collider;


    [SerializeField] private DamageType _damageType = DamageType.Melee;
    public DamageType TypeOfDamage { get { return _damageType; } }

    private DelayManager _delayManager;
    [SerializeField] private bool _addImpactForce = true;

    public ImpactForceSettings _impactForceSettings;

    public bool IsAvailableForConsumption(object sender)
    {
        var ret = !_wasConsumed;
        if(ret)
        {
            OnMeleeConsumed(new ConsumingObjectEventArgs { AttemptingToConsume = sender });
        }
        return ret;
    } 
    private bool _wasConsumed = false;

    [Range(0,3)]public float _delayAfterSwing = 0.5f;

    private bool ReadInput
    {
        get
        {
            if (isAbility) return _inputManager.m_Melee || _inputManager.m_Special;
            else return _inputManager.m_Melee;
        }
    }

    #region SFX and FX
    [HideInInspector] public string Swipe;
    [HideInInspector] public string PlayerImpact;
    [HideInInspector] public string Sheath;
    [HideInInspector] public string Clash;
    [HideInInspector] public string ClashCrowd;
    [HideInInspector] public string ClashAftermath;
    [HideInInspector] public string AbilitySecondSound;
    [HideInInspector] public string AbilityAerial;
    private AudioSource _sound;
    private bool _hasPlayedSheath = true;

    [HideInInspector] public ParticleSystem Trail;
    [HideInInspector] public ParticleSystem ClashingParticle;
    [HideInInspector] public ParticleSystem CrowdParticle;
    private ParticleSystem InstantiatedTrail;
    
    [Range(0,4)][SerializeField] private float _CrowdFxTimer = 2f;

    [SerializeField] private GameObject _ParticleReference;

    [SerializeField] private bool _useParticalTrail = true;
    [Range(0,5)][SerializeField] private float _trailGroundLifeTime = 1f;
    [Range(0,5)][SerializeField] private float _trailAerialLifeTime = 2f;

    [SerializeField] private TrailRenderer _trailPrefab;

    private ParticleSystem _particleTrail;
    private TrailRenderer _trail;
    #endregion

    protected Feedback _feedBack;

    // Use this for initialization
    private void Start()
    {
        if (_delayManager == null) _delayManager = GetComponent<DelayManager>();
        if (_inputManager == null) _inputManager = GetComponent<IPlatformer2DUserControl>();
        if (_mouvementManager == null) _mouvementManager = GetComponent<PlatformerCharacter2D>();
        if (_collider == null) Debug.LogError(ColliderString());
        _feedBack = GetComponent<Feedback>();
        if (_feedBack == null) Debug.LogError("Gun (Arc or shield) is unable to get the Feedback component on " + gameObject.name);
        //else _feedBack.CanShootFeedbackEvent += MeleeTimerReset;
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
        if (_delayManager.CanShoot && !_hasPlayedSheath) _hasPlayedSheath = PlaySheath();

        if (_delayManager.CanShoot && !m_isSwinging && ReadInput)
        {
            if (m_animator != null)
            {
                m_isSwinging = StartAnimatedSwing();
                OnMeleeStarted();
            }
            
        }

        _impactForceSettings.DirectionComingForm = _inputManager.m_FacingRight ? Direction2D.Left : Direction2D.Right;

        if (_sound != null && _sound.isPlaying && _isAerial && _mouvementManager.IsGrounded)
            StopAerialSwingOnLand();
    }

    private bool PlaySheath()
    {
        SoundManager.PlaySFX(Sheath);
        return true;
    }

    private bool StartAnimatedSwing()
    {
        _SwingReset = false;
        _feedBack.SetBool();
        _hasPlayedSheath = false;
        _delayManager.AddDelay(10000);
        if (!_collider.enabled) _collider.enabled = true;
        _isAerial = !_mouvementManager.IsGrounded;
        if (isAbility)
        {
            _mouvementManager.ChangeCanFlip();

            if (_isAerial)
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
        //StartTrail();
        return true;
    }

    private void StopAerialSwingOnLand()
    {
        if (isAbility && _abilityAerialCancelOnGround)
        {
            _sound.Stop();
            ResetSwing("Air", false);
            OnMeleeSpecialHitGround();
        }
    }

    public void ResetSwing(string change, bool with)
    {
        if (!_SwingReset)
        {
            _collider.enabled = true;
            m_isSwinging = false;
            m_animator.SetBool(change, with);
            _delayManager.SetDelay(_delayAfterSwing);
            _wasConsumed = false;
            if (_useParticalTrail)
            {
                if (_particleTrail != null) Destroy(_particleTrail);
            }
            else
            {
                if (_trail != null) Destroy(_trail);
            }
            if (isAbility)
            {
                _mouvementManager.ChangeCanFlip();
                /*
                if (_isAerial)
                    _isAerial = !_mouvementManager.IsGrounded;
                    */
            }
            _SwingReset = true;
        }
    }

    public void StartTrail()
    {
        float timer = _trailAerialLifeTime;
        if (isAbility && !_isAerial) timer = _trailGroundLifeTime;

        if (_useParticalTrail)
        {

            _particleTrail = InstatiateParticle(Trail, _ParticleReference, false, timer, true);
            _particleTrail.startColor = _inputManager.m_PlayerData.PlayerSponsor.SponsorColor;
        }
        else
        {
            _trail = InstatiateTrail(_trailPrefab, _ParticleReference, false, timer, true);
            _trail.material = _inputManager.m_PlayerData.PlayerSponsor.SponsorMaterial;
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
            if (isAbility) {
                if(!_isAerial) _isAerial = !_mouvementManager.IsGrounded;

                if (_isAerial && _abilityUseDifferentAerialDmg)
                    return Database.instance.MeleeAbilityAerialDamage;
                else
                    return Database.instance.MeleeAbilityDamage;
            }
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
    public event EventHandler<ConsumingObjectEventArgs> MeleeConsumed;
    protected virtual void OnMeleeConsumed(ConsumingObjectEventArgs e)
    {
        try
        {
            if (MeleeConsumed != null) MeleeConsumed(this, e);
        }
        catch(Exception ex) { ex.Log(); throw; }
    }
    public event EventHandler MeleeSpecialHitGround;
    protected virtual void OnMeleeSpecialHitGround()
    {
        if (MeleeSpecialHitGround != null) MeleeSpecialHitGround(this, EventArgs.Empty);
    }
    public event EventHandler MeleeStarted;
    protected virtual void OnMeleeStarted()
    {
        if (MeleeStarted != null) MeleeStarted(this, EventArgs.Empty);
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
        InstatiateParticle(CrowdParticle, toParent, false, _CrowdFxTimer);
    }
}


public class ConsumingObjectEventArgs : EventArgs
{
    public object AttemptingToConsume { get; set; }
}

