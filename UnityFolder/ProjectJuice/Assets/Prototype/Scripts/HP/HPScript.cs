using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEditor;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class HPScript : ExtendedMonobehaviour
{
    [SerializeField] private float _maxHp;
    private float _currentHp;
    private IPlatformer2DUserControl _inputController;

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
            var othersPosition = collider.gameObject.transform.position - _centerOfReferenceForJuice.position;
            RaycastHit2D hit = default(RaycastHit2D);
            Vector2 pointOfCollision;
            #region DetectImpact

            if (checkDamaging.HasPreferredImpactPoint)
            {
                pointOfCollision = checkDamaging.PreferredImpactPoint;
            }
            else
            {
                for (int i = 0; i < _raycastIterationsToFindTarget; i++)
                {
                    var firstTarget = new Vector3(othersPosition.x + _raycastVariationPerTry*i,
                        othersPosition.y + _raycastVariationPerTry*i, othersPosition.z);
                    hit = Physics2D.Raycast(_centerOfReferenceForJuice.position, firstTarget, float.MaxValue);
                    if (hit.collider == collider) break;
                    //Testing where raycast went
                    //Debug.DrawRay(_centerOfReferenceForJuice.position, firstTarget, Color.green);
                    //EditorApplication.isPaused = true;
                    var secondTarget = new Vector3(othersPosition.x - _raycastVariationPerTry*i,
                        othersPosition.y - _raycastVariationPerTry*i, othersPosition.z);
                    hit = Physics2D.Raycast(_centerOfReferenceForJuice.position, secondTarget, float.MaxValue);
                    if (hit.collider == collider) break;
                    //Debug.DrawRay(_centerOfReferenceForJuice.position, secondTarget, Color.red);
                }
                pointOfCollision = hit.point;
            }

            #endregion DetectImpact
            CurrentHp -= checkDamaging.Damage;
            checkDamaging.Consumed();
            if (checkDamaging.AddImpactForce)
            {
                StartCoroutine(AddForceCoroutine(checkDamaging.ImpactForceSettings));
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

    IEnumerator AddForceCoroutine(ImpactForceSettings impactForceSettings)
    {
        float currentTime = 0f;
        float currentMultiplier = 1;
        while (currentTime < impactForceSettings.FirstCycleTime)
        {
            //print(_mainRigidbody.velocity);
            _mainRigidbody.velocity = (impactForceSettings.DirectionComingForm == Direction2D.Right ? 1 : -1) *addPositiveForceTo(impactForceSettings.ImpactForce, currentMultiplier, _mainRigidbody.velocity);
            currentTime += Time.deltaTime;
            currentMultiplier *= impactForceSettings.FirstCycleSpeedMitigator;
            yield return null;
        }
        yield return null;
        StartCoroutine(AddAfterForce(impactForceSettings, _mainRigidbody.velocity));
        //_mainRigidbody.AddForce(forceToAdd, ForceMode2D.Impulse);
    }

    IEnumerator AddAfterForce(ImpactForceSettings impactForceSettings, Vector2 cumulativeForce)
    {
        float currentTime = 0f;
        float currentMultiplier = 1;
        while (currentTime < impactForceSettings.SecondCycleTime)
        {
            //print(currentTime);
            _mainRigidbody.velocity = (impactForceSettings.DirectionComingForm == Direction2D.Right ? 1 : -1) * addPositiveForceTo(cumulativeForce, currentMultiplier, _mainRigidbody.velocity);
            currentTime += Time.deltaTime;
            currentMultiplier *= impactForceSettings.SecondCycleSpeedMitigator;
            yield return null;
        }
        if (impactForceSettings.ZeroVelocityAtEnd) _mainRigidbody.velocity = Vector2.zero;
    }

    private Vector2 addPositiveForceTo(Vector2 reference, float multiplier, Vector2 checkForY)
    {
        Vector2 ret;
        if (checkForY.y >= 0)
        {
            ret = reference*multiplier;
        }
        else
        {
            ret = new Vector2(reference.x * multiplier, checkForY.y);
        }
        return ret;
    }


}
