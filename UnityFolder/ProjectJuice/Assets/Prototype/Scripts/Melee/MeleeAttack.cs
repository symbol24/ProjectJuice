using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

public class MeleeAttack : ExtendedMonobehaviour
{
    public IPlatformer2DUserControl InputManager { get { return _inputManager; } }
    private IPlatformer2DUserControl _inputManager;
    [SerializeField] private bool _isAbility = false; //for spawner
    public bool isAbility { get { return _isAbility; } }
    [SerializeField] private GameObject _swingerCollider;
    [SerializeField] private GameObject _flipReference;
    [SerializeField] private DelayManager _delayManager;
    [SerializeField] private float _damage = 80;
    [SerializeField] private bool _addImpactForce = true;
    [SerializeField] private PlatformerCharacter2D _physicsManager;
    [SerializeField] private GameObject _clashingEffectPrefab;

    public ImpactForceSettings _impactForceSettings;
    

    public float rotationSpeed = 100f;
    public float startingRotation = -45;
    public float endingRotation = 45;
    public float _delayAfterSwing = 0.5f;

    // Use this for initialization
    private void Start()
    {
        if (_delayManager == null) _delayManager = GetComponent<DelayManager>();
        if (_inputManager == null) _inputManager = GetComponent<IPlatformer2DUserControl>();
        _swingerCollider.gameObject.SetRotationEulerZ(startingRotation);
        if (_physicsManager == null) _physicsManager = GetComponent<PlatformerCharacter2D>();
    }

    // Update is called once per frame
     void Update()
    {
        if (_inputManager.m_Melee && !_isSwingingAnimationOnGoing)
        {
            _swingingAnimation = StartSwingingAnimation();
            StartCoroutine(_swingingAnimation);
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
    }

    private bool _isSwingingAnimationOnGoing = false;
    private IEnumerator _swingingAnimation;
    



    private IEnumerator StartSwingingAnimation()
    {
        _isSwingingAnimationOnGoing = true;
        _delayManager.AddDelay(100f);
        _swingerCollider.SetActive(true);
        yield return null;
        while (_swingerCollider.transform.rotation.eulerAngles.z.ToNormalizedAngle() <= endingRotation)
        {
            _swingerCollider.transform.Rotate(Vector3.forward * Time.deltaTime * rotationSpeed);
            
            yield return new WaitForEndOfFrame();
        }
        _swingerCollider.SetActive(false);
        yield return null;
        _swingerCollider.gameObject.SetRotationEulerZ(startingRotation);
        yield return new WaitForSeconds(_delayAfterSwing);
        _wasConsumedDuringThisAnimation = false;
        _delayManager.Reset();
        _isSwingingAnimationOnGoing = false;
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
        get { return _damage; }
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

        GameObject particles = null;
        if (otherMelee.HasPreferredImpactPoint)
        {
            particles = (GameObject)Instantiate(_clashingEffectPrefab, otherMelee.PreferredImpactPoint, otherMelee.transform.rotation);
        }
        else
        {
            particles = (GameObject)Instantiate(_clashingEffectPrefab);
        }
        particles.transform.parent = transform;
    }
}
