using System;
using UnityEngine;
using System.Collections;

public class DartChainV2 : MonoBehaviour {
    [SerializeField]
    private Rigidbody2D _mainRigidbody;
    [SerializeField]
    private HingeJoint2D _mainHingeJoint;
    [SerializeField]
    private bool _isStaticChain = false;

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
    public HingeJoint2D MainHingeJoint
    {
        get { return _mainHingeJoint ?? (_mainHingeJoint = GetComponent<HingeJoint2D>()); }
    }

    public EventHandler HitFloor;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    private void Update()
    {
        if (!_isStaticChain)
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
    }



    private void CurrentDartOnDartDestroyed(object sender, EventArgs eventArgs)
    {
        try {
            Destroy(gameObject);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
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
