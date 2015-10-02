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
                _currentDart.DartDestroyed += CurrentDartOnDartDestroyed;
                _currentDart.DartCollision += CurrentDart_DartCollision;
            }

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
    private bool _collidedWithSomething = false;
    

    void CurrentDart_DartCollision(object sender, EventArgs e)
    {
        _collidedWithSomething = true;
    }

    // Update is called once per frame
    private void Update()
    {

        if (!_isStaticChain && !_collidedWithSomething && _enableScriptTranslation)
        {
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
            var distanceToDart = Vector3.Distance(transform.position, CurrentDart.transform.position);
            var distanceToGun = Vector3.Distance(transform.position, CurrentGun.transform.position);
            if (distanceToDart < distanceToGun)
            {
                
                var direction = CurrentDart.transform.position - transform.position;
                MainRigidbody.AddForce(direction.normalized * Mathf.Sqrt(distanceToGun) * Mathf.Pow(_maxForce / (distanceToDart + distanceToGun), 3));
                /*if (NextChain != null)
                {
                    var direction = NextChain.transform.position - transform.position;
                    MainRigidbody.AddForce(direction.normalized*distanceToGun*
                                           (_maxForce/(distanceToDart + distanceToGun)));
                }*/
            }
            else
            {
                var direction = CurrentGun.transform.position - transform.position;
                MainRigidbody.AddForce(direction.normalized * Mathf.Sqrt(distanceToDart) * Mathf.Pow(_maxForce / (distanceToDart + distanceToGun), 3));
                /*if (PreviousChain != null)
                {
                    var direction = PreviousChain.transform.position - transform.position;
                    MainRigidbody.AddForce(direction.normalized*distanceToDart*
                                           (_maxForce/(distanceToDart + distanceToGun)));
                }*/

            }

            if (!IgnoreFloor && Vector3.Distance(transform.position, NextChain.transform.position) >
                (CurrentGun.HoseCrossSectionLength*4))
            {
                //Debug.Break();
                Debug.Log(transform.position);
                Debug.Log("distance is " + Vector3.Distance(transform.position, NextChain.transform.position));
                //if (HitFloor != null) HitFloor(this, EventArgs.Empty);
            }
        }
    }



    private void CurrentDartOnDartDestroyed(object sender, EventArgs eventArgs)
    {
<<<<<<< HEAD
        Destroy(gameObject);
=======
        try {
            Destroy(gameObject);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
>>>>>>> 0ca7e737836ec74b29f1fd5229b541a391e9c889
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        var groundCheck = collider.GetComponent<Ground>();
        if (groundCheck != null && !groundCheck.IsPassThrough)
        {
            if (HitFloor != null) HitFloor(this, EventArgs.Empty);
        }
    }

}
