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
    [SerializeField] private float _DashForce = 450f;
    private bool _completedAerialAttack = true;
    [SerializeField] private GameObject _swingerCollider;
    [SerializeField] private GameObject _flipReference;
    [SerializeField] private DelayManager _delayManager;
    [SerializeField] private bool _addImpactForce = true;
    [SerializeField] private PlatformerCharacter2D _physicsManager;
    [SerializeField] private ParticleSystem _clashingEffectPrefab;
    [SerializeField] private ParticleSystem _clashCroudFlashes;
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

    private LightFeedbackTemp _lightFeedback;

    // Use this for initialization
    private void Start()
    {
        if (_delayManager == null) _delayManager = GetComponent<DelayManager>();
        if (_inputManager == null) _inputManager = GetComponent<IPlatformer2DUserControl>();
        if (_mouvementManager == null) _mouvementManager = GetComponent<PlatformerCharacter2D>();
        _swingerCollider.gameObject.SetRotationEulerZ(startingRotation);
        if (_physicsManager == null) _physicsManager = GetComponent<PlatformerCharacter2D>();
        _lightFeedback = GetComponent<LightFeedbackTemp>();
        _lightFeedback.LightDone += MeleeTimerReset;
    }

    private void MeleeTimerReset(object sender, EventArgs e)
    {
        _delayManager.SetDelay(0);
    }

    // Update is called once per frame
    void Update()
    {

        if (_delayManager.CanShoot && _inputManager.m_Melee)
        {
            if (isAbility && !_mouvementManager.isGrounded)
            {
                _swingingAnimation = StartSwingingAnimation();
                StartCoroutine(_swingingAnimation);
                //need its own special thing here for airial ability melee, for now we can do it in the swing anim
            }
            else
            {
                _swingingAnimation = StartSwingingAnimation();
                StartCoroutine(_swingingAnimation);
            }
        }
        if (_inputManager.m_FacingRight)
        {
            _flipReference.transform.localScale = _flipReference.transform.localScale.SetX(1);
        }
        else
        {
            _flipReference.transform.localScale = _flipReference.transform.localScale.SetX(-1);
        }
        _impactForceSettings.DirectionComingForm = _inputManager.m_FacingRight ? Direction2D.Left : Direction2D.Right;

        if (!_isSwingingAnimationOnGoing && _completedAerialAttack && _mouvementManager.MeleeDownDashComplete && _swingerCollider.activeInHierarchy)
            _swingerCollider.SetActive(false);
    }

    private bool _isSwingingAnimationOnGoing = false;
    private IEnumerator _swingingAnimation;
    



    private IEnumerator StartSwingingAnimation()
    {
        
        bool isGroundedAtStart = _mouvementManager.isGrounded;
        if (isAbility)
        {
            _mouvementManager.ChangeCanFlip();
            if (_abilityHasSpike && !isGroundedAtStart)
                _completedAerialAttack = false;

            if(!isGroundedAtStart) SoundManager.PlaySFX(AbilityAerial);
        }

        SoundManager.PlaySFX(Swipe);

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
            else
                SoundManager.PlaySFX(AbilitySecondSound);
        }


        yield return new WaitForSeconds(_delayAfterSwing);
        _wasConsumedDuringThisAnimation = false;
        _isSwingingAnimationOnGoing = false;
        SoundManager.PlaySFX(Sheath);
        _lightFeedback.StartLightFeedback(0);
    }


    private bool _wasConsumedDuringThisAnimation = false;
    private List<HPScript> _immuneTargets = new List<HPScript>();


    public bool IsAvailableForConsumption
    {
        get { return _isSwingingAnimationOnGoing && !_wasConsumedDuringThisAnimation; }
    }

    public void Consumed()
    {
        _wasConsumedDuringThisAnimation = true;
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

    public void ClashedWithOtherMelee(MeleeDamagingCollider otherMelee)
    {
        _physicsManager.AddKnockBack(otherMelee);
        otherMelee.Consumed();
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
        InstatiateParticle(_clashingEffectPrefab, toParent, true);
        InstatiateParticle(_clashCroudFlashes, toParent, false, _FlashTimerOnScreen);
    }
}
