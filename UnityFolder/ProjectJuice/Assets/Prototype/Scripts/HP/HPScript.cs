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

    private bool m_ShieldImmunity = false;
    public bool ShieldImunity { get { return m_ShieldImmunity; } }
    public void SwitchShieldImunity()
    {
        m_ShieldImmunity = !m_ShieldImmunity;
    }

    [SerializeField] private GameObject _ragdoll;
    private bool _ragdollSpawned = false;

    #region EventsAvailable
    public event EventHandler<ImpactEventArgs> HpImpactReceived;
    public event EventHandler Dead;

    protected virtual void OnDead()
    {
        try
        {
            EventHandler handler = Dead;
            if (handler != null) handler(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            ex.Log("Before PlayDeathFX and DestroyingGameObject");
            throw;
        }
        PlayDeathFX();
        if (!_ragdollSpawned) _ragdollSpawned = SpawnRagdoll();
        Destroy(gameObject);
    }

    private bool SpawnRagdoll()
    {
        if (_ragdoll != null)
            Instantiate(_ragdoll, transform.position, transform.localRotation);

        return true;
    }

    protected virtual void OnHpImpactReceived(ImpactEventArgs e)
    {
        try
        {
            EventHandler<ImpactEventArgs> handler = HpImpactReceived;
            if (handler != null) handler(this, e);
        }
        catch (Exception ex)
        {
            ex.Log();
            throw;
        }
    }
    #endregion EventsAvailable

    
    [HideInInspector] public ParticleSystem _deathFlashes;
    [Range(0,5)][SerializeField] private float _deathFlashesLength = 2f;

    // Use this for initialization
    protected override void Start()
    {
        var ex = new NullReferenceException("TestException");
        ex.Log("Testing comment");
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
        yield return new WaitForFixedUpdate();
        OnDead();
    }

    public void RouteOnTriggerEnter2D(Collider2D collider, bool isBackCollider)
    {
        if (!m_ShieldImmunity)
        {
            CheckForDamage(collider, isBackCollider);
            if (!isBackCollider)
                CheckForPowerUp(collider);
        }
    }
    public void RouteOnCollisionEnter2D(Collision2D collision, bool isBackCollider)
    {
        if (!m_ShieldImmunity)
        {
            Collider2D collider = collision.collider;

            RouteOnTriggerEnter2D(collider, isBackCollider);
            /*
            CheckForDamage(collider, isBackCollider);
            if (!isBackCollider)
                CheckForPowerUp(collider);
                */
        }
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
                SoundManager.PlaySFX(Database.instance.JuicePickup);
            }
        }
    }
    
    private void PlayDeathFX()
    {
        InstatiateParticle(_deathFlashes, gameObject, false, _deathFlashesLength);
        SoundManager.PlaySFX(Database.instance.RobotDeath);
        SoundManager.PlaySFX(Database.instance.RobotDeathCrowd);
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
        //Debug.Log("afterCalc Damage = " + damage);
        return ret;
    }

    private void CheckForDamage(Collider2D collider, bool isBack)
    {
        var checkDamaging = collider.gameObject.GetComponent<IDamaging>();

        //If it is not damaging, dont bother with calculations
        if (checkDamaging != null && CheckIfIDamagableIsActive(checkDamaging))
        {

            Vector2 pointOfCollision = GetPointOfImpact(checkDamaging, collider, _centerOfReferenceForJuice, _raycastIterationsToFindTarget, _raycastVariationPerTry);
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

                PlayRightSound(collider, isBack);

                var e = new ImpactEventArgs
                {
                    Damage = damage,
                    type = checkDamaging.TypeOfDamage,
                    PointOfCollision = pointOfCollision,
                    color = _inputController.m_PlayerData.PlayerSponsor.SponsorColor
                };
                if(CurrentHp >= 0)
                    OnHpImpactReceived(e);
            }
        }
    }

    private void PlayRightSound(Collider2D collider, bool isWeakPoint = false)
    {
        string toPlay = "";

        var bullet = collider.GetComponent<Bullet>();
        if(bullet != null)
        {
            if (isWeakPoint) toPlay = bullet.WeakpointBulletImpact;
            else toPlay = bullet.RobotBulletImpact;
        }

        var melee = collider.GetComponent<MeleeDamagingCollider>();
        if (melee != null)
        {
            toPlay = melee._meleeAttack.PlayerImpact;
        }
        SoundManager.PlaySFX(toPlay);
    }

}
