using System;
using UnityEngine;
using System.Collections;

public class DartChainV2 : ExtendedMonobehaviour {
    [SerializeField]
    private Rigidbody2D _mainRigidbody;
    [SerializeField]
    private DistanceJoint2D _mainHingeJoint;
    [SerializeField]
    private bool _isStaticChain = false;
    [SerializeField]
    private bool _enableScriptTranslation = true;

    [SerializeField] private float _maxForce = 5f;

    public SappingDartGun CurrentGun { get; set; }
    private Dart _currentDart;
    public Dart CurrentDart
    {
        get { return _currentDart; }
        set
        {
            _currentDart = value;
            if (!_isStaticChain)
            {
                SubscribeToDartEvents(true);
            }

        }
    }

    private bool _isSubscribedToDartEvents = false;
    private void SubscribeToDartEvents(bool isToSubscribe)
    {
        if (isToSubscribe && !_isSubscribedToDartEvents)
        {
            _currentDart.DartDestroyed += CurrentDartOnDartDestroyed;
            _currentDart.DartCollision += CurrentDart_DartCollision;
            _isSubscribedToDartEvents = true;
        }
        else if (!isToSubscribe && _isSubscribedToDartEvents)
        {
            _currentDart.DartDestroyed -= CurrentDartOnDartDestroyed;
            _currentDart.DartCollision -= CurrentDart_DartCollision;
            _isSubscribedToDartEvents = false;
        }
    }

    public DartChainV2 PreviousChain { get; set; }
    private DartChainV2 _nextChain;
    public DartChainV2 NextChain
    {
        get { return _nextChain; }
        set
        {
            _nextChain = value;
            if (value != null)
            {
                value.PreviousChain = this;
                MainHingeJoint.connectedBody = value.MainRigidbody;
                var checkDistance = Vector3.Distance(transform.position, value.transform.position);
                if (checkDistance >= CurrentGun.HoseCrossSectionLength)
                {
                    var correctedPosition = Vector3.MoveTowards(transform.position, NextChain.transform.position,
                    checkDistance - (CurrentGun.HoseCrossSectionLength * 2));
                    transform.position = (correctedPosition);
                }
            }
        }
    }
    public Rigidbody2D MainRigidbody
    {
        get { return _mainRigidbody ?? (_mainRigidbody = GetComponent<Rigidbody2D>()); }
    }
    public DistanceJoint2D MainHingeJoint
    {
        get { return _mainHingeJoint ?? (_mainHingeJoint = GetComponent<DistanceJoint2D>()); }
    }

    public bool IgnoreFloor { get; set; }

    public EventHandler HitFloor;
    public EventHandler BrokenOnTolerance;
    private bool _collidedWithSomething = false;
    [SerializeField] private ForceStrategy _strategy = ForceStrategy.Cascade;
    [SerializeField] private float _mitigationFactor = 0.5f;
    private float _breakFullTimeout = 0.1f;
    private float _breakTimer;
    [SerializeField] private float _rotationCorrection = 90f;
    [SerializeField]private float _one80Tolerance = 0.1f;

    private enum ForceStrategy
    {
        None,
        ForceUponDistance,
        Cascade,
    }


    void CurrentDart_DartCollision(object sender, EventArgs e)
    {
        _collidedWithSomething = true;
    }

    private void Start()
    {
        CorrectRotation();
    }

    // Update is called once per frame
    private void Update()
    {
        CorrectRotation();
        if (!_isStaticChain && !_collidedWithSomething && _enableScriptTranslation)
        {
            _breakTimer = 0f;
            var checkDistance = Vector3.Distance(transform.position, NextChain.transform.position);
            if (checkDistance >= CurrentGun.HoseCrossSectionLength)
            {
                var correctedPosition = Vector3.MoveTowards(transform.position, NextChain.transform.position,
                    checkDistance - CurrentGun.HoseCrossSectionLength);
                transform.position = (correctedPosition);
                //Debug.Log("detectingTooFar");

            }
        }
        else if (_collidedWithSomething)
        {
            if (_breakTimer > _breakFullTimeout)
            {
                switch (_strategy)
                {
                    case ForceStrategy.ForceUponDistance:
                        ForceOnDistance();
                        break;
                    case ForceStrategy.Cascade:
                        CascadeForce();
                        break;
                    default:
                        break;
                }
                var distanceToNext = Vector3.Distance(transform.position, NextChain.transform.position);
                if (distanceToNext > CurrentGun.HoseCrossSectionLengthTolerance)
                {
                    //Debug.Log("Dart Cut for ToleranceSetting " + distanceToNext);
                    //Debug.DrawLine(transform.position, NextChain.transform.position, Color.red);
                    //Debug.Break();
                    if (BrokenOnTolerance != null) BrokenOnTolerance(this, EventArgs.Empty);
                }
            }
            else
            {
                _breakTimer += Time.deltaTime;
            }
        }
    }

    private void CascadeForce()
    {
        if (NextChain != null && PreviousChain != null)
        {
            if (NextChain._isStaticChain)
            {
                CascadeToNeighbour(CurrentDart.gameObject, PreviousChain);
            }
            else if (PreviousChain._isStaticChain)
            {
                CascadeToNeighbour(CurrentGun._dartSpawnPoint, NextChain);
            }
        }
    }

    private void CascadeToNeighbour(GameObject measureDistanceTo, DartChainV2 targettocascade)
    {
        if (!_isStaticChain)
        {
            var distancetochain = Vector3.Distance(measureDistanceTo.transform.position, transform.position);
            if (distancetochain >= CurrentGun.HoseCrossSectionLength)
            {
                var direction = measureDistanceTo.transform.position - transform.position;
                MainRigidbody.AddForce(direction.normalized*_maxForce*distancetochain);
                targettocascade.CascadeForce(this, _maxForce*_mitigationFactor*distancetochain);
            }
        }
    }

    private void CascadeForce(DartChainV2 source, float forcetoAdd)
    {
        if (!_isStaticChain)
        {
            var direction = source.transform.position - transform.position;
            MainRigidbody.AddForce(direction.normalized*forcetoAdd);
            if (source == PreviousChain)
            {
                NextChain.CascadeForce(this, forcetoAdd*_mitigationFactor);
            }
            else if (source == NextChain)
            {
                PreviousChain.CascadeForce(this, forcetoAdd*_mitigationFactor);
            }
        }
    }


    private void ForceOnDistance()
    {
        var distanceToDart = Vector3.Distance(transform.position, CurrentDart.transform.position);
        var distanceToGun = Vector3.Distance(transform.position, CurrentGun.transform.position);
        if (distanceToDart < distanceToGun)
        {

            var direction = CurrentDart.transform.position - transform.position;
            MainRigidbody.AddForce(direction.normalized * Mathf.Sqrt(distanceToGun) * Mathf.Pow(_maxForce / (distanceToDart + distanceToGun), 3));
        }
        else
        {
            var direction = CurrentGun.transform.position - transform.position;
            MainRigidbody.AddForce(direction.normalized * Mathf.Sqrt(distanceToDart) * Mathf.Pow(_maxForce / (distanceToDart + distanceToGun), 3));

        }

        if (!IgnoreFloor && Vector3.Distance(transform.position, NextChain.transform.position) >
            (CurrentGun.HoseCrossSectionLength * 4))
        {
            //Debug.Break();
            Debug.Log(transform.position);
            Debug.Log("distance is " + Vector3.Distance(transform.position, NextChain.transform.position));
            //if (HitFloor != null) HitFloor(this, EventArgs.Empty);
        }
    }


    private void CurrentDartOnDartDestroyed(object sender, EventArgs eventArgs)
    {
        if (gameObject != null)
        {
            SubscribeToDartEvents(false);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("NeedToLook at this");
        }
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        var groundCheck = collider.GetComponent<Ground>();
        if (groundCheck != null && !groundCheck.IsPassThrough)
        {
            try
            {
                if (HitFloor != null) HitFloor(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                ex.Log();
                throw;
            }
        }
    }

    private void CorrectRotation()
    {
        if (!_isStaticChain && NextChain != null && PreviousChain != null)
        {
            var newAngle = GetAlphaFrom(-NextChain.transform.position + PreviousChain.transform.position);
            newAngle += _rotationCorrection;
            transform.rotation = Quaternion.Euler(0, 0, newAngle);
        }
    }


    public float GetAlphaFrom(float aSide, float bSide, float cSide)
    {
        if (aSide + bSide >= cSide - _one80Tolerance) return 180;
        else if (cSide < _one80Tolerance) return 0;
        else
        {
            //CosineLaw
            var num = aSide*aSide - bSide*bSide - cSide*cSide;
            var den = 2*bSide*cSide;
            if (den == 0f) den = 0.000001f;
            //print(den);
            var angle = Mathf.Acos(num/den);
            return Mathf.Rad2Deg*angle;
        }
    }

    public float GetAlphaFrom(Vector3 toCalculateAngle)
    {
        float ret;
        if (toCalculateAngle.x == 0f) ret = 90;
        else if (toCalculateAngle.y == 0f) ret = 0;
        else
        {
            var ratio = toCalculateAngle.y/toCalculateAngle.x;
            ret = Mathf.Atan(ratio) * Mathf.Rad2Deg;
        }
        return ret;
    }
}
