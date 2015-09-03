using UnityEngine;
using System.Collections;

public class PhysicsSingleParticle : MonoBehaviour {
    /// <summary>
    /// ThisValue is a percentage between 0 and 1;
    /// </summary>
    public float ProbabilityOfHealthPickup
    {
        get { return _probabilityOfHealthPickup; }
        set { _probabilityOfHealthPickup = value; }
    }

    public float HpRecovered
    {
        get { return _hpRecovered; }
        set { _hpRecovered = value; }
    }

    private Vector3 _lastPosition = Vector3.zero;
    private bool _triggered = false;
    [SerializeField] private float _probabilityOfHealthPickup = 0.1f;
    [SerializeField] private float _hpRecovered = 5;
    [SerializeField] private bool _arePickupsToDisapear = false;
    [SerializeField] private float _timeoutBeforeDisapearingForPickups = 2f;

    public float _minDestroyTime = 1f;
    public float _masDestroyTime = 5f;
    private float RandomDestroyTime
    {
        get
        {
            var range = _masDestroyTime - _minDestroyTime;
            var ret = Random.value * range + _minDestroyTime;
            return ret;
        }
    }


    // Use this for initialization
    void Start ()
	{
        TriggerHpRecov();

    }
	
	// Update is called once per frame
	void FixedUpdate () 
    {
        /*
	    if ((_lastPosition == transform.position) && !_triggered)
	    {
            TriggerHpRecov();
	        _triggered = true;
	    }
	    _lastPosition = transform.position;
        */
    }


    private void TriggerHpRecov()
    {
        if (Random.value <= ProbabilityOfHealthPickup)
        {
            var recov = gameObject.AddComponent<HPRecovery>();
            recov.HPRecov = HpRecovered;
            if (_arePickupsToDisapear)
            {
                var timer = gameObject.AddComponent<DestroyOnTimer>();
                timer.Timeout = _timeoutBeforeDisapearingForPickups;
            }
        }
        else
        {
            var timer = gameObject.AddComponent<DestroyOnTimer>();
            timer.Timeout = RandomDestroyTime;
        }
    }

}
