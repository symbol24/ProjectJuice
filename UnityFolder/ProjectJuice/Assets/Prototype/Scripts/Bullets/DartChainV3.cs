using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof (CrossSectionAngleCorrection), typeof(DistanceJoint2D))]

public class DartChainV3 : MonoBehaviour
{
    public CrossSectionAngleCorrection _crossSectionAngleCorrection;
    public DistanceJoint2D _distanceJoint;
    public Rigidbody2D _mainRigidBody;
    public bool _isStaticChain = false;
    private DartChainV3 _previousChain;
    private DartChainV3 _nextChain;
    private DartGunSettings _settings;
    private IDart _dart;
    private DartGunV2 _dartGun;
    private GameObject _owner;
    private bool _collided = false;
    private bool _instantCollided = false;
    

    // Use this for initialization
    private void Start()
    {
        if(_crossSectionAngleCorrection == null) Debug.LogError("Please assign CrossSectionAngleCorrection");
        if(_distanceJoint == null) Debug.LogError("PleaseAssignJoint 2d");
        if (_mainRigidBody == null) Debug.LogError("Please assign MainRigidBody2d");
        CorrectPosition();
    }
    private void Update()
    {
        if (_isStaticChain) return;

        if (!_collided && !_instantCollided)
        {
            CorrectPosition();
        }
    }
    void FixedUpdate()
    {
        if (_collided)
        {
            CascadeForce();
            if (Vector3.Distance(_crossSectionAngleCorrection._nextGameObject.transform.position, transform.position) > _settings.HoseLengthTolerance)
            {
                OnBrokenOnTolerance();
            }
        }
    }

    private void CorrectPosition()
    {
        if (!_crossSectionAngleCorrection.IsEdge)
        {
            var checkDistance = Vector3.Distance(transform.position, _crossSectionAngleCorrection._nextGameObject.transform.position);
            if (checkDistance >= _settings.HoseLength)
            {
                var correctedPosition = Vector3.MoveTowards(transform.position, _crossSectionAngleCorrection._nextGameObject.transform.position,
                    checkDistance - _settings.HoseLength);
                transform.position = (correctedPosition);
            }
        }
    }

    public void SubscribeToDart(IDart leDart, DartGunV2 dartGun)
    {
        _dart = leDart;
        _dartGun = dartGun;
        leDart.DartDestroyed += LeDartOnDartDestroyed;
        leDart.DartCollision += LeDart_DartCollision;
    }

    private void LeDart_DartCollision(object sender, EventArgs e)
    {
        _instantCollided = true;
        if (_mainRigidBody != null)
        {
            _mainRigidBody.mass = _settings.HoseWeightAtcollision;
            StartCoroutine(DelayAction(_settings.HoseTimerToActivateTolerance, new Action(() => { _collided = true; })));
        }
        else Debug.LogWarning("Missing _mainRigidbody");
    }

    internal void Track(DartChainV3 _kinematicChain)
    {
        _mainRigidBody.isKinematic = true;
        StartCoroutine(TrackCoroutine(_kinematicChain));
    }

    private IEnumerator TrackCoroutine(DartChainV3 _kinematicChain)
    {
        while(true)
        {
            transform.position = _kinematicChain.transform.position;
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator DelayAction(float toDelay, Action action)
    {
        yield return new WaitForSeconds(toDelay);
        action.Invoke();
    }

    private void LeDartOnDartDestroyed(object sender, EventArgs eventArgs)
    {
        //Debug.Log("DestroyingHose");
        Destroy(gameObject);
    }

    public void AddNext(DartChainV3 toAdd, DartGunSettings settings)
    {
        _settings = settings;
        _nextChain = toAdd;
        _crossSectionAngleCorrection._nextGameObject = toAdd.gameObject;
        _distanceJoint.connectedBody = toAdd._mainRigidBody;
        toAdd._crossSectionAngleCorrection._previousGameObject = gameObject;
        _nextChain._previousChain = this;
    }

    public event EventHandler BrokenOnTolerance;
    protected virtual void OnBrokenOnTolerance()
    {
        if (BrokenOnTolerance != null) BrokenOnTolerance(this, EventArgs.Empty);
    }
    public event EventHandler BrokenOnGround;
    protected virtual void OnBrokenOnGround()
    {
//        Debug.Log("OnBrokenOnGround");
        if (BrokenOnGround != null) BrokenOnGround(this, EventArgs.Empty);
    }


    private void CascadeForce()
    {
        if (_nextChain != null && _previousChain != null)
        {
            if (_nextChain._crossSectionAngleCorrection.IsEdge)
            {
                CascadeToNeighbour(_dart.gameObject, _previousChain);
            }
            else if (_previousChain._crossSectionAngleCorrection.IsEdge)
            {
                CascadeToNeighbour(_dartGun._kinematicChain.gameObject, _nextChain);
            }
        }
    }

    private void CascadeToNeighbour(GameObject measureDistanceTo, DartChainV3 targettocascade)
    {
        if (!_crossSectionAngleCorrection.IsEdge)
        {
            var distancetochain = Vector3.Distance(measureDistanceTo.transform.position, transform.position);
            if (distancetochain >= _settings.HoseLength + _settings.HoseApplyForceThresholdCorrection)
            {
                var direction = measureDistanceTo.transform.position - transform.position;
                float distanceToMultiply;
                distancetochain += _settings.HosePowBaseCorrection;
                if (distancetochain < 1)
                {
                    distancetochain += 1;
                    distanceToMultiply = Mathf.Pow(distancetochain, 1 / _settings.HoseDistancePow);
                }
                else distanceToMultiply = Mathf.Pow(distancetochain, _settings.HoseDistancePow);
                var applying = direction.normalized * _settings.HoseFlatForceMultiplier * distanceToMultiply;
                //Debug.Log(applying);
                _mainRigidBody.AddForce(applying);
                targettocascade.CascadeForce(this, _settings.HoseFlatForceMultiplier * _settings.HoseMitigator * distancetochain, direction);
            }
        }
    }
    private void CascadeForce(DartChainV3 source, float forcetoAdd, Vector3 previousDirection)
    {
        if (!_crossSectionAngleCorrection.IsEdge)
        {
            var direction = source.transform.position - transform.position;
            direction = (direction + _settings.HoseOriginalDirectionWeight * previousDirection).normalized;
            _mainRigidBody.AddForce(direction.normalized * forcetoAdd);
            if (source == _previousChain)
            {
                _nextChain.CascadeForce(this, forcetoAdd * _settings.HoseMitigator, direction);
            }
            else if (source == _nextChain)
            {
                _previousChain.CascadeForce(this, forcetoAdd * _settings.HoseMitigator, direction);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        //Debug.Log(collider.name);
        var ground = collider.gameObject.GetComponent<Ground>();
        if(_settings.HoseBrakeOnCollision && (ground == null || !ground.IsPassThrough) && IgnoreHpScripts(collider.gameObject))
        {
            OnBrokenOnGround();
        }
    }

    private bool IgnoreHpScripts(GameObject gameOb)
    {
        var topObject = GetTopOfLine(gameOb);
        var hpScript = topObject.GetComponent<HPScript>();
        return hpScript == null;
    }

    private GameObject GetTopOfLine(GameObject gameOb)
    {
        var ret = gameOb;
        if(ret.transform.parent != null)
        {
            ret = GetTopOfLine(ret.transform.parent.gameObject);
        }
        return ret;
    }
}
