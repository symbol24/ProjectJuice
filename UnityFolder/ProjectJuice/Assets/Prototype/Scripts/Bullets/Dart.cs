using System.Runtime.InteropServices;
using System.Timers;
using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;

public class Dart : ExtendedMonobehaviour
{
    [SerializeField] private DartChainV2 _staticCrossSection;
    public DartChainV2 StaticCrossSection { get { return _staticCrossSection; } }
    public bool _debugInvencible = false;

    public bool IsContiniousSucking { get; set; }
    public float SuckingInterval { get; set; }
    public float HpSuckedPerSecond { get; set; }
    public float DartCollTimerDisappear { get; set; }
    public float LifetimeSinceHit { get; set; }
    public HPScript SourceHPScript { get; set; }

    private HPScript _targetHpScript;
    private float currentIntervalTimer = 0f;
    private bool _hitAWall = false;
    private float destroyTimer;
    private Rigidbody2D _mainRigidbody;
    private Rigidbody2D MainRigidbody2D
    {
        get
        {
            if (_mainRigidbody == null)
            {
                _mainRigidbody = GetComponent<Rigidbody2D>();
            }
            return _mainRigidbody;
        }
    }

    public IPlatformer2DUserControl InputManager { get; set; }

    private float _currentLifetimeAfterHit = 0f;
    private Transform _sourceTransform;
    private bool _ignoreWalls = false;


    void Update ()
    {
        
        if (!InputManager.m_SpecialStay && !_debugInvencible)
        {
            OnDartDestroyed();
        }


        if(_hitAWall)
        {
            destroyTimer += Time.deltaTime;
            if(destroyTimer >= DartCollTimerDisappear)
            {
                OnDartDestroyed();
            }
        }
        else if (_targetHpScript != null)
        {
            //CheckLifetime
            _currentLifetimeAfterHit += Time.deltaTime;
            if (_currentLifetimeAfterHit >= LifetimeSinceHit)
            {
                OnDartDestroyed();
            }

            //ApplyDamage
            if (IsContiniousSucking)
            {
                SuckHP();
            }
            else
            {
                currentIntervalTimer += Time.deltaTime;
                if (currentIntervalTimer >= SuckingInterval)
                {
                    SuckHP();
                    currentIntervalTimer = 0f;
                }
            }
        }
	}

    void Start()
    {
        DartCollision += Dart_DartCollision;
        SourceHPScript.Dead += SourceHpScriptOnDead;
    }

    private void SourceHpScriptOnDead(object sender, EventArgs eventArgs)
    {
        OnDartDestroyed();
    }
    void Dart_DartCollision(object sender, EventArgs e)
    {
        var hits = Physics2D.Raycast(transform.position, transform.position - SourceHPScript.transform.position,
            Vector3.Distance(transform.position, SourceHPScript.transform.position));
        
    }


    public void ShootBullet(float force, Transform sourceTransform)
    {
        if (force < 1f) force = 1;
        MainRigidbody2D.AddForce(transform.up * force);
        _sourceTransform = sourceTransform;
    }

    #region CollisionChecking
    private void OnTriggerEnter2D(Collider2D collider)
    {
        CheckForHPCollision(collider.gameObject);
        if(_targetHpScript == null)
        {
            if (_ignoreWalls) _ignoreWalls = !_ignoreWalls;
            else CheckForWall(collider.gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!CheckForHPCollision(collision.gameObject))
        {
            CheckForWall(collision.gameObject);
        }
    }
    #endregion CollisionChecking

    private void CheckForWall(GameObject toCheckWall)
    {
        //For now
        _hitAWall = true;
        StickDart(toCheckWall);
        OnDartCollision();
    }
    private bool CheckForHPCollision(GameObject toCheck)
    {
        var ret = false;
        if (_targetHpScript == null)
        {
            _targetHpScript = toCheck.GetComponent<HPScript>();
            if (_targetHpScript == null)
            {
                var checkShield = toCheck.GetComponent<ShieldReRouter>();
                if (checkShield == null)
                {
                    _targetHpScript = toCheck.GetComponentInParent<HPScript>();
                }
                else
                {
                    _hitAWall = true;
                    StickDart(toCheck);
                    OnDartCollision();
                }
            }


            if (_targetHpScript == SourceHPScript)
            {
                _targetHpScript = null;
                _ignoreWalls = true;
            }

            //Check if we got our hpScript
            if (_targetHpScript != null)
            {
                _targetHpScript.Dead += TargetHpScriptOnDead;
                OnDartCollision();
                ret = true;
                StickDart(toCheck);
            }
        }
        return ret;
    }

    private void TargetHpScriptOnDead(object sender, EventArgs eventArgs)
    {
        OnDartDestroyed();
    }

    private void SuckHP()
    {
        
        float hpToSuck;
        if (IsContiniousSucking)
        {
            hpToSuck = HpSuckedPerSecond*Time.deltaTime;
        }
        else
        {
            hpToSuck = HpSuckedPerSecond*SuckingInterval;
        }
        //Debug.Log("hpToSuck = " + hpToSuck + " Time.deltaTime=" + Time.deltaTime);
        OnJuiceSucked(new JuiceSuckedEventArgs { HpSucked = hpToSuck });
        _targetHpScript.CurrentHp -= hpToSuck;
    }

    public void StickDart(GameObject toUseAsParent)
    {
        gameObject.transform.parent = toUseAsParent.transform;
        MainRigidbody2D.velocity = Vector3.zero;
        MainRigidbody2D.isKinematic = true;
    }


    #region events
    public event EventHandler<JuiceSuckedEventArgs> JuiceSucked;
    protected void OnJuiceSucked(JuiceSuckedEventArgs e)
    {
        if(JuiceSucked != null)
        {
            JuiceSucked(this, e);
        }
    }
    public event EventHandler DartCollision;
    protected void OnDartCollision()
    {
        if (DartCollision != null) DartCollision(this, EventArgs.Empty);
        
    }
    public event EventHandler DartDestroyed;
    protected void OnDartDestroyed()
    {
        if (DartDestroyed != null) DartDestroyed(this, EventArgs.Empty);
        if (gameObject != null)
        {
            Destroy(gameObject);

        }
        else
        {
            Debug.Log("NeedToLook at this");
        }
    }
    #endregion events

    public void ListenToCrossSection(DartChainV2 brandNewCrossSection)
    {
        brandNewCrossSection.HitFloor += HitFloor;
    }

    private void HitFloor(object sender, EventArgs eventArgs)
    {
        OnDartDestroyed();
    }
}
