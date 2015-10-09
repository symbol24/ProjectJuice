using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
//using UnityEditor;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityStandardAssets._2D;

public class HPScript : HPBase
{
    
    private IPlatformer2DUserControl _inputController;
    public IPlatformer2DUserControl inputController { get { return _inputController; } }
    private PlatformerCharacter2D _character;

    [Range(0,3)][SerializeField] private float _raycastVariationPerTry = 0.1f;
    [Range(0,20)][SerializeField] private int _raycastIterationsToFindTarget = 5;
    [SerializeField] private Transform _centerOfReferenceForJuice;
    [SerializeField] private Rigidbody2D _mainRigidbody;


    #region EventsAvailable
    public event EventHandler<ImpactEventArgs> HpImpactReceived;
    public event EventHandler Dead;

    protected virtual void OnDead()
    {
        EventHandler handler = Dead;
        if (handler != null) handler(this, EventArgs.Empty);
        Debug.Log("addDeath Animation");

        Destroy(gameObject);
    }
    protected virtual void OnHpImpactReceived(ImpactEventArgs e)
    {
        EventHandler<ImpactEventArgs> handler = HpImpactReceived;
        if (handler != null) handler(this, e);
    }
    #endregion EventsAvailable

    // Use this for initialization
    protected override void Start()
    {
        MaxHp = Database.instance.PlayerBaseHP;
        base.Start();
        if (_centerOfReferenceForJuice == null) _centerOfReferenceForJuice = transform;
        if (_mainRigidbody == null) _mainRigidbody = GetComponent<Rigidbody2D>();
        if (_inputController == null) _inputController = GetComponent<IPlatformer2DUserControl>();
        if (_character == null) _character = GetComponent<PlatformerCharacter2D>();
        HpChanged += OnHpChanged;
    }

    private void OnHpChanged(object sender, HpChangedEventArgs hpChangedEventArgs)
    {
        if (CurrentHp <= 0) StartCoroutine(DestroyNextFrame());
    }

    private IEnumerator DestroyNextFrame()
    {
        yield return null;
        OnDead();
    }

    public void RouteOnTriggerEnter2D(Collider2D collider, bool isBackCollider)
    {
        
        CheckForDamage(collider, isBackCollider);
        if(!isBackCollider)
            CheckForPowerUp(collider);
    }
    public void RouteOnCollisionEnter2D(Collision2D collision, bool isBackCollider)
    {
        Collider2D collider = collision.collider;

        CheckForDamage(collider, isBackCollider);
        if (!isBackCollider)
            CheckForPowerUp(collider);
    }

    private void CheckForPowerUp(Collider2D collider)
    {
        var checkPowerUp = collider.gameObject.GetComponent<IPowerUp>();
        if (checkPowerUp != null)
        {
            if (checkPowerUp.IsAvailableForConsumption(this))
            {
                CurrentHp += checkPowerUp.HPRecov;
                checkPowerUp.Consumed();
            }
        }
    }
    

    private bool DamagingDoesDamage(IDamaging checkDamaging, Vector2 pointOfCollision, out float damage)
    {
        bool ret = true;
        damage = checkDamaging.Damage;
        //Debug.Log("original Damage = " + damage);
        if (checkDamaging.ImpactForceSettings.IsFadeDmgOnDistance)
        {
            var distance = Vector2.Distance(checkDamaging.gameObject.transform.position, transform.position);
            if (distance <= checkDamaging.ImpactForceSettings.FadeMaxDmgDistance)
            {
                damage = damage * (checkDamaging.ImpactForceSettings.FadeMaxDmgDistance - distance)/
                         checkDamaging.ImpactForceSettings.FadeMaxDmgDistance;
            }
            else
            {
                ret = false;
            }
        }
        Debug.Log("afterCalc Damage = " + damage);
        return ret;
    }

    private void CheckForDamage(Collider2D collider, bool isBack)
    {
        var checkDamaging = collider.gameObject.GetComponent<IDamaging>();

        //If it is not damaging, dont bother with calculations
        if (checkDamaging != null && CheckIfIDamagableIsActive(checkDamaging))
        {
            Vector2 pointOfCollision = GetPointOfImpact(checkDamaging, collider, _centerOfReferenceForJuice,
                _raycastIterationsToFindTarget, _raycastVariationPerTry);
            float damage;
            if (DamagingDoesDamage(checkDamaging, pointOfCollision, out damage))
            {
                float mulitpliedDmg = damage;
                if (isBack) mulitpliedDmg = Database.instance.BackDamageMultiplier * damage;

                CurrentHp -= mulitpliedDmg;
                checkDamaging.Consumed();

                if (checkDamaging.AddImpactForce)
                {
                    checkDamaging.UpdateImpactForceSetting(GetDirectionFromImpact(collider,
                        checkDamaging.ImpactForceSettings));

                    _character.AddKnockBack(checkDamaging);
                }

                var e = new ImpactEventArgs
                {
                    Damage = damage,
                    PointOfCollision = pointOfCollision,
                    color = _inputController.m_PlayerData.PlayerSponsor.SponsorColor
                };
                OnHpImpactReceived(e);
            }
        }
    }



}
