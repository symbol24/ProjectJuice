using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class HPScript : ExtendedMonobehaviour
{
    [SerializeField] private float _maxHp;
    private float _currentHp;

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
        if (checkDamaging != null && checkDamaging.IsAvailableForConsumption && !IsAChild(checkDamaging))
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
                StartCoroutine(AddForceCoroutine(checkDamaging.ImpactForce, checkDamaging.TimeToApplyForce));
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

    IEnumerator AddForceCoroutine(Vector2 forceToAdd, float timeToApplyForce)
    {
        float currentTime = 0f;
        while (currentTime < timeToApplyForce)
        {
            print(currentTime);
            _mainRigidbody.velocity = forceToAdd;
            currentTime += Time.deltaTime;
            yield return null;
        }
        _mainRigidbody.velocity = Vector2.zero;
        yield return null;
        //_mainRigidbody.AddForce(forceToAdd, ForceMode2D.Impulse);
    }

}
