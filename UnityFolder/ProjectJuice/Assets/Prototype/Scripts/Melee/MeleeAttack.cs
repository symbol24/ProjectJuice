﻿using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

public class MeleeAttack : ExtendedMonobehaviour
{
    public IPlatformer2DUserControl InputManager { get { return _inputManager; } }
    private IPlatformer2DUserControl _inputManager;
    [SerializeField] 
    private GameObject _swingerCollider;

    [SerializeField] private GameObject _flipReference;
    [SerializeField]
    private DelayManager _delayManager;
    [SerializeField]
    private float _damage = 80;
    [SerializeField]
    private Vector2 _impactForce;
    [SerializeField]
    private bool _addImpactForce = true;
    [SerializeField]
    private float _timeToApplyForce;

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

    public Vector2 ImpactForce
    {
        get { return _impactForce*(_inputManager.m_FacingRight ? 1 : -1); }
        private set { _impactForce = value; }
    }

    public float TimeToApplyForce
    {
        get { return _timeToApplyForce; }
        private set { _timeToApplyForce = value; }
    }

    public IEnumerable<HPScript> ImmuneTargets { get { return _immuneTargets; } }
    public bool HasImmuneTargets { get { return false; }}
}
