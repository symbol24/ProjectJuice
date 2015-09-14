using System.Runtime.InteropServices;
using System.Timers;
using UnityEngine;
using System.Collections;
using System;

public class Dart : ExtendedMonobehaviour
{
    [SerializeField] private DartChain _crosssectionPrefab;

    public bool IsContiniousSucking { get; set; }
    public float SuckingInterval { get; set; }
    public float HpSuckedPerSecond { get; set; }
    public float DartCollTimerDisappear { get; set; }
    public float LifetimeSinceHit { get; set; }

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
    private float _currentLifetimeAfterHit = 0f;
    private Transform _sourceTransform;


    void Update ()
    {
        if(_hitAWall)
        {
            destroyTimer += Time.deltaTime;
            if(destroyTimer >= DartCollTimerDisappear)
            {
                OnDartDestroyed();
                Destroy(this.gameObject);
            }
        }
        else if (_targetHpScript != null)
        {
            //CheckLifetime
            _currentLifetimeAfterHit += Time.deltaTime;
            if (_currentLifetimeAfterHit >= LifetimeSinceHit)
            {
                OnDartDestroyed();
                Destroy(gameObject);
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
            CheckForWall(collider.gameObject);
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
                var checkShield = toCheck.GetComponent<shield>();
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

            //Check if we got our hpScript
            if (_targetHpScript != null)
            {
                OnDartCollision();
                ret = true;
                StickDart(toCheck);
            }
        }
        return ret;
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
    }
    #endregion events
}
