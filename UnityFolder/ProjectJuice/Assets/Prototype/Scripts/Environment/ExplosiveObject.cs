﻿using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ExplosiveObject : HPBase
{
    [SerializeField] private bool _IsExplosive = true;
    public bool IsExplosive { get { return _IsExplosive; } }
    [SerializeField] private GameObject _explosionPrefab;
    [Range(0,10)][SerializeField] private float _explosionDestroyTimeout = 3f;
    [SerializeField] private List<GameObject> _ragdollParticles;
    [SerializeField] private bool _enableTimerBeforeExplosion = false;
    [Range(0,10)][SerializeField] private float _timerBeforeExplosion;
    [SerializeField] private bool _addImpactForce = true;
    [SerializeField] private ImpactForceSettings _impactForceSettings;
    [Range(0,10)][SerializeField] private float _DragResetDelay;
    [Range(0,500)][SerializeField] private float _impactForceModifier = 100f;
    [Range(-50,50)][SerializeField] private float _massAdditionWhenIgnited = -10;
    [Range(0,10)][SerializeField] private float _explosionDuration = 0.2f;
    [SerializeField]private ForceMode2D m_ForceType = ForceMode2D.Force;
    [SerializeField] private float _XMinForce = 1f;
    [SerializeField] private float _XMaxForce = 1f;
    private float RandomXSpeed
    {
        get
        {
            return UnityEngine.Random.Range(_XMinForce, _XMaxForce);
        }
    }
    [SerializeField] private float _YMinForce = 1f;
    [SerializeField] private float _YMaxForce = 1f;
    private float RandomYSpeed
    {
        get
        {
            return UnityEngine.Random.Range(_YMinForce, _YMaxForce);
        }
    }
    
    private Rigidbody2D _mainRigidbody;
    [SerializeField] private Collider2D _bulletExplosionCollisionEvaluator;
    public List<GameObject> _explosiveColliders;
    public List<GameObject> _collidersToDisableDuringExplosion;

    private DelayManager _delayManager;

    public GameObject ExplosionPrefab
    {
        get { return _explosionPrefab; }
    }
    
    [Range(0,10)][SerializeField] public int _bulletsToGive = 5;

    private LoadingScreen m_fade;
    private bool isMuted = true;

    [Range(0,10)][SerializeField] private float _chromaticDelayBeforeDestruction = 0.5f;

    [HideInInspector] public string Pushing;
    [HideInInspector] public string GroundImpact;
    [HideInInspector] public string BulletImpact;
    [HideInInspector] public string Explosion;
    
    [HideInInspector] public ParticleSystem _explosionFX;
    [HideInInspector] public ParticleSystem _shockwave;
    [HideInInspector] public ParticleSystem _chromaticAberation;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        _mainRigidbody = GetComponent<Rigidbody2D>();
        if(_bulletExplosionCollisionEvaluator == null) _bulletExplosionCollisionEvaluator = GetComponent<Collider2D>();
        HpChanged += OnHpChanged;
        _delayManager = GetComponent<DelayManager>();
        _delayManager.Reset();
        m_fade = FindObjectOfType<LoadingScreen>();
        if (m_fade != null) m_fade.LoadingAnimDone += M_fade_FadeDone;
        else isMuted = false;
    }

    private void M_fade_FadeDone(object sender, EventArgs e)
    {
        isMuted = false;
    }

    private void SwitchCollidersOnOff()
    {
        foreach(GameObject expl in _explosiveColliders)
        {
            expl.SetActive(!expl.activeInHierarchy);
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public void RouteOnCollisionEnter2D(Collision2D collision)
    {
        CheckForDamaging(collision.collider);
    }

    public void RouteOnTriggerEnter2D(Collider2D collider)
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
            if(!isMuted)
                SoundManager.PlaySFX(BulletImpact);

            if (damaging.AddImpactForce)
            {
                damaging.UpdateImpactForceSetting(GetDirectionFromImpact(toCheck, damaging.ImpactForceSettings));

                AddKnockBack(damaging);

            }

                //Instantiate Explosion
                var contactPoint = damaging.HasPreferredImpactPoint
                ? damaging.PreferredImpactPoint
                : toCheck.transform.position;

            if (ExplosionPrefab != null)
            {
                var particleObject = (GameObject) Instantiate(ExplosionPrefab, contactPoint, Quaternion.identity);
                var destroyer = particleObject.AddComponent<DestroyOnTimer>();
                destroyer.Timeout = _explosionDestroyTimeout;
            }
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
        float before = _timerBeforeExplosion - _chromaticDelayBeforeDestruction;
        if (before < 0) before = 0;
        yield return new WaitForSeconds(before);
        InstatiateParticle(_chromaticAberation, gameObject, false, _chromaticDelayBeforeDestruction);
        yield return new WaitForSeconds(_chromaticDelayBeforeDestruction);
        Kaboom();
    }

    private void Kaboom()
    {
        //Debug.Break();
        DisplayFX();
        if(!isMuted)
            SoundManager.PlaySFX(Explosion);
        SwitchCollidersOnOff();
        foreach (var ragdollParticlePrefab in _ragdollParticles)
        {
            Quaternion randomRotation = new Quaternion(0, 0, UnityEngine.Random.rotation.z, 0);
            var particle = (GameObject) Instantiate(ragdollParticlePrefab, transform.position, randomRotation);
            var particleRigidbody = particle.GetComponent<Rigidbody2D>();
            particleRigidbody.AddForce(new Vector2(RandomXSpeed, RandomYSpeed), m_ForceType);
        }
        if (_IsExplosive)
        {
            foreach (var explosiveCollider in _explosiveColliders)
            {
                var script = explosiveCollider.AddComponent<ExplosiveObjectCollider>();
                script._explosiveObject = this;
                _mainRigidbody.isKinematic = true;
            }
        }
        var timer = gameObject.AddComponent<DestroyOnTimer>();
        timer.Timeout = _explosionDuration;
    }

    private void DisplayFX()
    {
        InstatiateParticle(_explosionFX, gameObject);
        InstatiateParticle(_shockwave, gameObject);
    }

    private IEnumerator DeleteNextUpdate(ExplosiveObjectCollider script)
    {
        yield return null;
        Destroy(script);
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
        get { return Database.instance.ExplosionBaseDamage; }
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

    public void AddKnockBack(IDamaging impact)
    {
        Vector2 angle = impact.ImpactForceSettings.ImpactAngle;

        if (impact.ImpactForceSettings.DirectionComingForm == Direction2D.Right) angle.x = -angle.x;

        // normalize and add impulse value
        angle = angle.normalized * impact.ImpactForceSettings.ImpactForce * _impactForceModifier;
        _mainRigidbody.velocity = Vector2.zero;
        _mainRigidbody.AddForce(angle, m_ForceType);

        //set values for cooldown
        _DragResetDelay = impact.ImpactForceSettings.ImpactDragTimer;
        StartCoroutine(DashDragReset(_DragResetDelay, _mainRigidbody));
        
    }

    private void CheckDrag()
    {
        if (_mainRigidbody.drag != 0) _mainRigidbody.drag = 0;
    }

    IEnumerator DashDragReset()
    {
        yield return new WaitForSeconds(_DragResetDelay);
        CheckDrag();
    }

    public event EventHandler Destroyed;
    protected void OnDestroyed()
    {
        if (Destroyed != null) Destroyed(this, EventArgs.Empty);
    }

    void OnDestroy()
    {
        OnDestroyed();
    }

}
