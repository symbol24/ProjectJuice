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
    // Use this for initialization
	void Start ()
	{
	    

	}
	
	// Update is called once per frame
	void FixedUpdate () 
    {
	    if ((_lastPosition == transform.position) && !_triggered)
	    {
            TriggerHpRecov();
	        _triggered = true;
	    }
	    _lastPosition = transform.position;
    }


    private void TriggerHpRecov()
    {
        if (Random.value <= ProbabilityOfHealthPickup)
        {
            var recov = gameObject.AddComponent<HPRecovery>();
            recov.HPRecov = HpRecovered;
        }
        else
        {
            var timer = gameObject.AddComponent<DestroyOnTimer>();
            timer.Timeout = 0;
        }
    }

}
