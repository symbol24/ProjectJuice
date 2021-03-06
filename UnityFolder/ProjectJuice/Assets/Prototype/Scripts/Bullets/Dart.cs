﻿using System.Linq;
using System.Timers;
using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;

public class Dart : DartBase
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

    public IPlatformer2DUserControl InputManager { get; set; }

    private float _currentLifetimeAfterHit = 0f;
    private bool _ignoreWalls = false;

    [Range(0,10)][SerializeField] private int m_BulletsToGiveShield = 2;
    public int BulletsToGiveShield { get { return m_BulletsToGiveShield; } }

    public override HPScript Target
    {
        get
        {
            return _targetHpScript;
        }
    }

    //[SerializeField] private float _maxDistanceToTravel = 30.6f;

    


    void Update ()
    {
        bool dartToBeDestroyed = !InputManager.m_SpecialStay && !_debugInvencible;

        if(_hitAWall)
        {
            destroyTimer += Time.deltaTime;
            if(destroyTimer >= DartCollTimerDisappear)
            {
                dartToBeDestroyed = true;
            }
        }
        else if (_targetHpScript != null)
        {
            //CheckLifetime
            _currentLifetimeAfterHit += Time.deltaTime;
            if (_currentLifetimeAfterHit >= LifetimeSinceHit)
            {
                dartToBeDestroyed = true;
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

        if (dartToBeDestroyed) OnDartDestroyed();

        if (!_nextUpdateIsCollision && !_hitAWall && _targetHpScript == null)
        {
            Debug.DrawRay(transform.position, MainRigidbody2D.velocity*Time.deltaTime, Color.green);
            var colliders = Physics2D.RaycastAll(transform.position, MainRigidbody2D.velocity,
                MainRigidbody2D.velocity.magnitude*(Time.deltaTime + 0.001f));
            var collisionFound =
                colliders.FirstOrDefault(
                    c =>
                        c.collider.gameObject.GetComponent<HPScript>() != null ||
                        c.collider.gameObject.GetComponent<shield>() != null ||
                        (c.collider.gameObject.GetComponent<Ground>() != null &&
                         !c.collider.gameObject.GetComponent<Ground>().IsPassThrough));
            if (collisionFound != default(RaycastHit2D))
            {
                var newVel = MainRigidbody2D.velocity.normalized*
                             ((Vector3.Distance(collisionFound.transform.position, transform.position))/Time.deltaTime);
                MainRigidbody2D.velocity = Vector3.Magnitude(newVel) - Vector3.Magnitude(newVel.normalized*_correctionToNewVel) < 0
                    ? newVel.normalized
                    : newVel - newVel.normalized*_correctionToNewVel;
                Debug.Log("clamping velocity");
                Debug.Log(MainRigidbody2D.velocity);
                _nextUpdateIsCollision = true;
            }
            //Debug.Break();
        }
        else
        {
            
        }
        //Debug.Log(MainRigidbody2D.velocity);
    }
    private bool _nextUpdateIsCollision = false;

    protected override void Initialize()
    {
        base.Initialize();
        DartCollision += Dart_DartCollision;
        SourceHPScript.Dead += SourceHpScriptOnDead;
    }

    private void SourceHpScriptOnDead(object sender, EventArgs eventArgs)
    {
        OnDartDestroyed();
    }
    void Dart_DartCollision(object sender, EventArgs e)
    {
        /*
        var hits = Physics2D.Raycast(transform.position, transform.position - SourceHPScript.transform.position,
            Vector3.Distance(transform.position, SourceHPScript.transform.position));*/
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
        CheckForHPCollision(collision.gameObject);
        if (_targetHpScript == null)
        {
            if (_ignoreWalls) _ignoreWalls = !_ignoreWalls;
            else CheckForWall(collision.gameObject);
        }
        /*
        if (!CheckForHPCollision(collision.gameObject))
        {
            CheckForWall(collision.gameObject);
        }
        */
    }
    #endregion CollisionChecking

    private void CheckForWall(GameObject toCheckWall)
    {
        var checkForPassthrough = toCheckWall.GetComponent<Ground>();
        if (checkForPassthrough == null || !checkForPassthrough.IsPassThrough)
        {
            _hitAWall = true;
            StickDart(toCheckWall);
            OnDartCollision();
            SoundManager.PlaySFX(GroundImpact);
        }
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
                    SoundManager.PlaySFX(GroundImpact);
                }
            }
            else
            {
                //If target is shielding, do not use the target script;
                var shield = _targetHpScript.gameObject.GetComponent<shield>();
                if (shield.IsShieldActive) _targetHpScript = null;
                OnDartDestroyed();
            }


            if (_targetHpScript == SourceHPScript)
            {
                _targetHpScript = null;
                _ignoreWalls = true;
            }

            //Check if we got our hpScript
            if (_targetHpScript != null)
            {
                SubscribeToTargetHPScript(true);
                OnDartCollision();
                ret = true;
                StickDart(toCheck);
                SoundManager.PlaySFX(PlayerImpact);
            }
        }
        return ret;
    }

    private bool _isSubscribedToTargetHPScript = false;
    [SerializeField]private float _correctionToNewVel = 50;

    private void SubscribeToTargetHPScript(bool isToSubscribe)
    {
        if (isToSubscribe && !_isSubscribedToTargetHPScript)
        {
            _targetHpScript.Dead += TargetHpScriptOnDead;
            _isSubscribedToTargetHPScript = true;
        }
        else if (!isToSubscribe && _isSubscribedToTargetHPScript)
        {
            _targetHpScript.Dead -= TargetHpScriptOnDead;
            _isSubscribedToTargetHPScript = false;
        }

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
    private bool _onDartAlreadyDestroyed = false;
    public override event EventHandler DartDestroyed;
    protected override void OnDartDestroyed()
    {
        try
        {
            if (DartDestroyed != null) DartDestroyed(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            ex.Log("When calling the event");
            throw;
        }

        try
        {
            SubscribeToTargetHPScript(false);
            if (!_onDartAlreadyDestroyed && gameObject != null)
            {
                Destroy(gameObject);
                _onDartAlreadyDestroyed = true;
            }
        }
        catch (Exception ex)
        {
            ex.Log("When attempting to destroy GameObject");
            throw;
        }
    }
    #endregion events

    public void ListenToCrossSection(DartChainV2 brandNewCrossSection)
    {
        brandNewCrossSection.HitFloor += HitFloor;
        brandNewCrossSection.BrokenOnTolerance += BrokenOnTolerance;
    }

    private void BrokenOnTolerance(object sender, EventArgs eventArgs)
    {
        
        OnDartDestroyed();
    }

    private void HitFloor(object sender, EventArgs eventArgs)
    {
        //Debug.Log("Dart Hit the Floor");
        SoundManager.PlaySFX(Severed);
        OnDartDestroyed();
    }
}
