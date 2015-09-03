using System;
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

    // Use this for initialization
    private void Start()
    {
        CurrentHp = MaxHp;
        if (_centerOfReferenceForJuice == null) _centerOfReferenceForJuice = transform;
    }

    // Update is called once per frame
    private void Update()
    {

    }
    /*
    private void OnTriggerEnter2D(Collider2D collider)
    {
        //Debug.Log("OnTriggerEnter in " + gameObject.name);

        CheckForDamage(collider);
        CheckForPowerUp(collider);

    }
    */
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D collider = collision.collider;

        CheckForDamage(collider);
        CheckForPowerUp(collider);
    }

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
        if (checkDamaging != null && !IsAChild(collider.gameObject) && checkDamaging.IsAvailableForConsumption)
        {
            var othersPosition = collider.gameObject.transform.position - _centerOfReferenceForJuice.position;
            RaycastHit2D hit = default(RaycastHit2D);
            #region DetectImpact
            for (int i = 0; i < _raycastIterationsToFindTarget; i++)
            {
                var firstTarget = new Vector3(othersPosition.x + _raycastVariationPerTry * i, othersPosition.y + _raycastVariationPerTry * i, othersPosition.z);
                hit = Physics2D.Raycast(_centerOfReferenceForJuice.position, firstTarget, float.MaxValue);
                if (hit.collider == collider) break;
                //Testing where raycast went
                //Debug.DrawRay(_centerOfReferenceForJuice.position, firstTarget, Color.green);
                //EditorApplication.isPaused = true;
                var secondTarget = new Vector3(othersPosition.x - _raycastVariationPerTry * i, othersPosition.y - _raycastVariationPerTry * i, othersPosition.z);
                hit = Physics2D.Raycast(_centerOfReferenceForJuice.position, secondTarget, float.MaxValue);
                if (hit.collider == collider) break;
                //Debug.DrawRay(_centerOfReferenceForJuice.position, secondTarget, Color.red);
            }
            #endregion DetectImpact

            CurrentHp -= checkDamaging.Damage;
            checkDamaging.Consumed();

            Vector2 pointOfCollision = hit.point;
            OnHpImpactReceived(new ImpactEventArgs
            {
                Damage = checkDamaging.Damage,
                PointOfCollision = pointOfCollision
            });

            if (CurrentHp <= 0) OnDead();
        }
    }
}
