using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ExplosiveObject : HPBase
{
    [SerializeField] private bool _IsExplosive = true;
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
    [Range(-10,10)][SerializeField] private float _XMinForce = 1f;
    [Range(-10,10)][SerializeField] private float _XMaxForce = 1f;
    private float RandomXSpeed
    {
        get
        {
            var range = _XMaxForce - _XMinForce;
            var ret = UnityEngine.Random.value * range + _XMinForce;
            return ret * Mathf.Sign(UnityEngine.Random.value - 0.5f);
        }
    }
    [Range(-10,10)][SerializeField] private float _YMinForce = 1f;
    [Range(-10,10)][SerializeField] private float _YMaxForce = 1f;
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

    private DelayManager _delayManager;

    public GameObject ExplosionPrefab
    {
        get { return _explosionPrefab; }
    }
    
    [Range(0,10)][SerializeField] public int _bulletsToGive = 5;
    [Range(0, 1)][SerializeField] private float _delayFourPushingSound = 0.4f;

    [HideInInspector] public string Pushing;
    [HideInInspector] public string GroundImpact;
    [HideInInspector] public string BulletImpact;
    [HideInInspector] public string Explosion;

    [HideInInspector] public ParticleSystem _groundScraping;
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

    public void RouteOnCollisionStay2D(Collider2D collider)
    {
        var player = collider.GetComponent<IPlatformer2DUserControl>();
        var ground = collider.GetComponent<Ground>();
        if (player != null && ground != null)
        {
            print("both touching ground and player");
            if(_mainRigidbody.velocity.x > 0 || _mainRigidbody.velocity.x < 0)
            {
                print("volicity is greater or lower");
                if (_delayManager.SoundReady)
                {
                    SoundManager.PlaySFX(Pushing);
                    InstatiateParticle(_groundScraping, gameObject);
                    _delayManager.AddSoundDelay(_delayFourPushingSound);
                }
            }
        }
    }

    private void CheckForDamaging(Collider2D toCheck)
    {
        var damaging = toCheck.GetComponent<IDamaging>();
        if (damaging != null && CheckIfIDamagableIsActive(damaging))
        {
            //Get Hit
            CurrentHp -= damaging.Damage;
            damaging.Consumed();
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
            //Debug.DrawRay(transform.position, contactPoint);
            //Debug.Break();
            if (ExplosionPrefab != null)
            {
                var particleObject = (GameObject) Instantiate(ExplosionPrefab, contactPoint, Quaternion.identity);
                var destroyer = particleObject.AddComponent<DestroyOnTimer>();
                destroyer.Timeout = _explosionDestroyTimeout;
            }
        }

        var ground = toCheck.GetComponent<Ground>();
        if(ground != null) SoundManager.PlaySFX(GroundImpact);

        /*
        var player = toCheck.GetComponent<IPlatformer2DUserControl>();
        if (player != null) SoundManager.PlaySFX(GroundImpact);
        */

        var otherExplosif = toCheck.GetComponent<ExplosiveObjectDamagableCollider>();
        if (otherExplosif != null) SoundManager.PlaySFX(GroundImpact);
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
        DisplayFX();
        SoundManager.PlaySFX(Explosion);
        SwitchCollidersOnOff();
        foreach (var ragdollParticlePrefab in _ragdollParticles)
        {
            var particle = (GameObject) Instantiate(ragdollParticlePrefab, transform.position, Quaternion.identity);
            var particleRigidbody = particle.GetComponent<Rigidbody2D>();
            particleRigidbody.AddForce(new Vector2(RandomXSpeed, RandomYSpeed));
        }
        if (_IsExplosive)
        {
            foreach (var explosiveCollider in _explosiveColliders)
            {
                var script = explosiveCollider.AddComponent<ExplosiveObjectCollider>();
                script._explosiveObject = this;
                _mainRigidbody.isKinematic = true;
                //StartCoroutine(DeleteNextUpdate(script));
            }
        }
        var timer = gameObject.AddComponent<DestroyOnTimer>();
        timer.Timeout = _explosionDuration;
    }

    private void DisplayFX()
    {
        InstatiateParticle(_explosionFX, gameObject);
        InstatiateParticle(_shockwave, gameObject);
        InstatiateParticle(_chromaticAberation, gameObject);
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
    
        //_mainRigidbody.drag = impact.ImpactForceSettings.ImpactDrag;

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
}
