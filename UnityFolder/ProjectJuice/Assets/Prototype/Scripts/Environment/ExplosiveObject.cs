﻿using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ExplosiveObject : HPBase
{
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private float _explosionDestroyTimeout = 3f;
    [SerializeField] private List<GameObject> _ragdollParticles;
    [SerializeField] private bool _enableTimerBeforeExplosion = false;
    [SerializeField] private float _timerBeforeExplosion;
    [SerializeField] private float _damage = 100;
    [SerializeField] private bool _addImpactForce = true;
    [SerializeField] private ImpactForceSettings _impactForceSettings;
    [SerializeField] private float _massAdditionWhenIgnited = -10;
    [SerializeField] private float _explosionDuration = 0.2f;

    [SerializeField] private float _XMinForce = 1f;
    [SerializeField] private float _XMaxForce = 1f;
    private float RandomXSpeed
    {
        get
        {
            var range = _XMaxForce - _XMinForce;
            var ret = UnityEngine.Random.value * range + _XMinForce;
            return ret * Mathf.Sign(UnityEngine.Random.value - 0.5f);
        }
    }
    [SerializeField] private float _YMinForce = 1f;
    [SerializeField] private float _YMaxForce = 1f;
    private float RandomYSpeed
    {
        get
        {
            var range = _YMaxForce - _YMinForce;
            var ret = UnityEngine.Random.value * range + _YMinForce;
            return ret;
        }
    }

    private Rigidbody2D _mainRigidbody;
    [SerializeField] private Collider2D _bulletExplosionCollisionEvaluator;
    public List<GameObject> _explosiveColliders;
    public List<GameObject> _collidersToDisableDuringExplosion;


    public GameObject ExplosionPrefab
    {
        get { return _explosionPrefab; }
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        _mainRigidbody = GetComponent<Rigidbody2D>();
        if(_bulletExplosionCollisionEvaluator == null) _bulletExplosionCollisionEvaluator = GetComponent<Collider2D>();
        HpChanged += OnHpChanged;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckForDamaging(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        CheckForDamaging(collider);
    }

    private void CheckForDamaging(Collider2D toCheck)
    {
        var damaging = toCheck.GetComponent<IDamaging>();
        if (damaging != null && CheckIfIDamagableIsActive(damaging))
        {
            //Get Hit
            CurrentHp -= damaging.Damage;
            damaging.Consumed();

            //Instantiate Explosion
            var contactPoint = damaging.HasPreferredImpactPoint
                ? damaging.PreferredImpactPoint
                : toCheck.transform.position;
            //Debug.DrawRay(transform.position, contactPoint);
            //Debug.Break();
            var particleObject = (GameObject) Instantiate(ExplosionPrefab, contactPoint, Quaternion.identity);
            var destroyer = particleObject.AddComponent<DestroyOnTimer>();
            destroyer.Timeout = _explosionDestroyTimeout;
            
        }
    }

    private void OnHpChanged(object sender, HpChangedEventArgs hpChangedEventArgs)
    {
        if (hpChangedEventArgs.NewHp <= 0 && !_waitingForExplosionNoReturn)
        {
            //TimeToExplode
            if (_enableTimerBeforeExplosion)
            {
                _mainRigidbody.mass += _massAdditionWhenIgnited;
                StartCoroutine(TimerForExplosion());
            }
            else
            {
                Kaboom();
            }
        }
    }

    private bool _waitingForExplosionNoReturn = false;
    private IEnumerator TimerForExplosion()
    {
        _waitingForExplosionNoReturn = true;
        print("need to add some kind of combustion here");
        yield return new WaitForSeconds(_timerBeforeExplosion);
        Kaboom();
    }

    private void Kaboom()
    {
        print("need to add fireworks here");
        foreach (var ragdollParticlePrefab in _ragdollParticles)
        {
            var particle = (GameObject) Instantiate(ragdollParticlePrefab, transform.position, Quaternion.identity);
            var particleRigidbody = particle.GetComponent<Rigidbody2D>();
            particleRigidbody.AddForce(new Vector2(RandomXSpeed, RandomYSpeed));
        }
        foreach (var explosiveCollider in _explosiveColliders)
        {
            var script = explosiveCollider.AddComponent<ExplosiveObjectCollider>();
            script._explosiveObject = this;
            _mainRigidbody.isKinematic = true;
        }
        var timer = gameObject.AddComponent<DestroyOnTimer>();
        timer.Timeout = _explosionDuration;
    }

    public bool IsAvailableForConsumption
    {
        get { return true; }
    }
    public void Consumed()
    {

    }
    public float Damage
    {
        get { return _damage; }
    }
    public bool HasPreferredImpactPoint { get; private set; }
    public Vector3 PreferredImpactPoint { get; private set; }
    public bool AddImpactForce
    {
        get { return _addImpactForce; }
    }
    public ImpactForceSettings ImpactForceSettings
    {
        get { return _impactForceSettings; }
    }
    public IEnumerable<HPScript> ImmuneTargets { get; private set; }
    public bool HasImmuneTargets
    {
        get { return false; }
    }
}