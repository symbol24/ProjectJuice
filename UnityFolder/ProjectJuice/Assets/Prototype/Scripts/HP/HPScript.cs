using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEditor;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityStandardAssets._2D;

public class HPScript : ExtendedMonobehaviour
{
    [SerializeField] private float _maxHp;
    private float _currentHp;
    private IPlatformer2DUserControl _inputController;
    private PlatformerCharacter2D _character;

    public float MaxHp
    {
        get { return _maxHp; }
    }
    public float CurrentHp
    {
        get { return _currentHp; }
        private set
        {
            if (_currentHp != value)
            {
                var e = new HpChangedEventArgs() {NewHp = value, PreviousHp = _currentHp};
                _currentHp = value;
                OnHpChanged(e);
            }
        }
    }

    [SerializeField] private float _raycastVariationPerTry = 0.1f;
    [SerializeField] private float _raycastIterationsToFindTarget = 5;
    [SerializeField] private Transform _centerOfReferenceForJuice;
    [SerializeField] private Rigidbody2D _mainRigidbody;

    #region EventsAvailable
    public event EventHandler<ImpactEventArgs> HpImpactReceived;
    public event EventHandler<HpChangedEventArgs> HpChanged;
    public event EventHandler Dead;

    protected virtual void OnDead()
    {
        EventHandler handler = Dead;
        if (handler != null) handler(this, EventArgs.Empty);
    }
    protected virtual void OnHpChanged(HpChangedEventArgs e)
    {
        EventHandler<HpChangedEventArgs> handler = HpChanged;
        if (handler != null) handler(this, e);
    }
    protected virtual void OnHpImpactReceived(ImpactEventArgs e)
    {
        EventHandler<ImpactEventArgs> handler = HpImpactReceived;
        if (handler != null) handler(this, e);
    }
    #endregion EventsAvailable

    // Use this for initialization
    private void Start()
    {
        CurrentHp = MaxHp;
        if (_centerOfReferenceForJuice == null) _centerOfReferenceForJuice = transform;
        if (_mainRigidbody == null) _mainRigidbody = GetComponent<Rigidbody2D>();
        if (_inputController == null) _inputController = GetComponent<IPlatformer2DUserControl>();
        if (_character == null) _character = GetComponent<PlatformerCharacter2D>();
    }

    // Update is called once per frame
    private void Update()
    {

    }
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        //Debug.Log("OnTriggerEnter in " + gameObject.name);

        CheckForDamage(collider);
        CheckForPowerUp(collider);

    }
    private void OnCollisionEnter2D(Collision2D collision)
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
           
            Vector2 pointOfCollision = GetPointOfImpact(checkDamaging, collider);

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

    private bool CheckIfIDamagableIsActive(IDamaging checkDamaging)
    {
        var ret = checkDamaging.IsAvailableForConsumption && !IsAChild(checkDamaging);
        if (ret && checkDamaging.HasImmuneTargets)
        {
            //If it is not in the immunetargets collection, it should do damage
            ret = !checkDamaging.ImmuneTargets.Any(c => c == this);
        }
        return ret;

    }

    private Vector2 GetPointOfImpact(IDamaging chkDamaging, Collider2D collider)
    {
        Vector2 ret = new Vector2();
        var othersPosition = collider.gameObject.transform.position - _centerOfReferenceForJuice.position;
        RaycastHit2D hit = default(RaycastHit2D);
        if (chkDamaging.HasPreferredImpactPoint)
        {
            ret = chkDamaging.PreferredImpactPoint;
        }
        else
        {
            for (int i = 0; i < _raycastIterationsToFindTarget; i++)
            {
                var firstTarget = new Vector3(othersPosition.x + _raycastVariationPerTry * i,
                    othersPosition.y + _raycastVariationPerTry * i, othersPosition.z);
                hit = Physics2D.Raycast(_centerOfReferenceForJuice.position, firstTarget, float.MaxValue);
                if (hit.collider == collider) break;
                //Testing where raycast went
                //Debug.DrawRay(_centerOfReferenceForJuice.position, firstTarget, Color.green);
                //EditorApplication.isPaused = true;
                var secondTarget = new Vector3(othersPosition.x - _raycastVariationPerTry * i,
                    othersPosition.y - _raycastVariationPerTry * i, othersPosition.z);
                hit = Physics2D.Raycast(_centerOfReferenceForJuice.position, secondTarget, float.MaxValue);
                if (hit.collider == collider) break;
                //Debug.DrawRay(_centerOfReferenceForJuice.position, secondTarget, Color.red);
            }
            ret = hit.point;
        }


        return ret;
    }
    
    IEnumerator AddForceCoroutine(ImpactForceSettings impactForceSettings)
    {
        float currentTime = 0f;
        float currentMultiplier = 1;
        _mainRigidbody.drag = 5;
        int counter = 1;
        Vector2 velocityExpected = Vector2.zero;
        _mainRigidbody.velocity = impactForceSettings.ImpactAngle;
        yield return null;
        while (currentTime < impactForceSettings.ImpactDrag)
        {
            print(_mainRigidbody.velocity);
            velocityExpected = AddPositiveForceTo(_mainRigidbody.velocity,impactForceSettings);
            _mainRigidbody.velocity = velocityExpected;// + Physics2D.gravity;//*counter;
            currentTime += Time.deltaTime;
            currentMultiplier *= impactForceSettings.FirstCycleSpeedMitigator;
            counter++;
            if (currentMultiplier <= 0.01f) currentTime = float.MaxValue;

            yield return null;
        }
        _mainRigidbody.drag = 0;
        StartCoroutine(AddAfterForce(impactForceSettings, velocityExpected));
        //_mainRigidbody.AddForce(forceToAdd, ForceMode2D.Impulse);
    }

    IEnumerator AddAfterForce(ImpactForceSettings impactForceSettings, Vector2 previousVelocity)
    {
        float currentTime = 0f;
        int counter = 1;
        while (currentTime < impactForceSettings.ImpactForce && Math.Abs(_inputController.m_XAxis) < 0.01f)
        {
            if (Mathf.Abs(previousVelocity.x) > 0.01f)
            {
                _mainRigidbody.velocity =
                    new Vector2(previousVelocity.x* Mathf.Pow(impactForceSettings.SecondCycleSpeedMitigator, counter),
                        _mainRigidbody.velocity.y);
            }
            counter++;
            yield return null;
        }
        if (impactForceSettings.ZeroVelocityAtEnd) _mainRigidbody.velocity = Vector2.zero;
    }

    private Vector2 previousVector;
    private int counterForAddRelativeForce = 1;
    private Vector2 AddPositiveForceTo(Vector2 reference, ImpactForceSettings forceSettings)
    {
        
        Vector2 ignoringY;
        float xVector = forceSettings.ImpactAngle.x * forceSettings.FirstCycleSpeedMitigator * counterForAddRelativeForce;
        xVector = Mathf.Abs(xVector);
        if (forceSettings.DirectionComingForm == Direction2D.Right) xVector *= -1;
        float yVector = reference.y*counterForAddRelativeForce;
        


        return new Vector2(xVector, yVector);
    }


}
