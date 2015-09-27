using System;
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
    private PlatformerCharacter2D _character;

    [SerializeField] private float _raycastVariationPerTry = 0.1f;
    [SerializeField] private int _raycastIterationsToFindTarget = 5;
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
        base.Start();
        if (_centerOfReferenceForJuice == null) _centerOfReferenceForJuice = transform;
        if (_mainRigidbody == null) _mainRigidbody = GetComponent<Rigidbody2D>();
        if (_inputController == null) _inputController = GetComponent<IPlatformer2DUserControl>();
        if (_character == null) _character = GetComponent<PlatformerCharacter2D>();
    }

    public void RouteOnTriggerEnter2D(Collider2D collider)
    {
        //Debug.Log("OnTriggerEnter in " + gameObject.name);

        CheckForDamage(collider);
        CheckForPowerUp(collider);
    }
    public void RouteOnCollisionEnter2D(Collision2D collision)
    {
        Collider2D collider = collision.collider;

        CheckForDamage(collider);
        CheckForPowerUp(collider);
    }
    /*
    private void OnTriggerStay2D(Collider2D collider)
    {
        CheckForDamage(collider);
    }*/

    private void CheckForPowerUp(Collider2D collider)
    {
        var checkPowerUp = collider.gameObject.GetComponent<IPowerUp>();
        if (checkPowerUp != null)
        {
            if (checkPowerUp.IsAvailableForConsumption)
            {
                CurrentHp += checkPowerUp.HPRecov;
                checkPowerUp.Consumed();
            }
        }
    }

    private void CheckForDamage(Collider2D collider)
    {
        var checkDamaging = collider.gameObject.GetComponent<IDamaging>();

        //If it is not damaging, dont bother with calculations
        if (checkDamaging != null && CheckIfIDamagableIsActive(checkDamaging))
        {
            Vector2 pointOfCollision = GetPointOfImpact(checkDamaging, collider, _centerOfReferenceForJuice, _raycastIterationsToFindTarget, _raycastVariationPerTry);

            CurrentHp -= checkDamaging.Damage;
            checkDamaging.Consumed();

            if (checkDamaging.AddImpactForce)
            {
                _character.AddKnockBack(checkDamaging);
            }

            var e = new ImpactEventArgs
            {
                Damage = checkDamaging.Damage,
                PointOfCollision = pointOfCollision
            };
            OnHpImpactReceived(e);

            if (CurrentHp <= 0) OnDead();
        }
    }

    
    


}
