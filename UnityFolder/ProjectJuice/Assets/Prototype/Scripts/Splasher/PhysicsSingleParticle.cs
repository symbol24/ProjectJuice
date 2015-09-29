using System.Linq;
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

    public float ProbablityOfHealthStayOnGround
    {
        get { return _probablityOfHealthStayOnGround; }
        set { _probablityOfHealthStayOnGround = value; }
    }

    public float HpRecovered
    {
        get { return _hpRecovered; }
        set { _hpRecovered = value; }
    }

    private Vector3 _lastPosition = Vector3.zero;
    private bool _triggered = false;
    [Range(0,1)][SerializeField] private float _probabilityOfHealthPickup = 0.1f;
    [Range(0,1)][SerializeField] private float _probablityOfHealthStayOnGround = 0.2f;
    [SerializeField] private float _hpRecovered = 5;
    [SerializeField] private bool _arePickupsToDisapear = false;
    [Range(0,5)][SerializeField] private float _timeoutBeforeDisapearingForPickups = 2f;

    [Range(0, 5)] public float _minDestroyTime = 1f;
    [Range(0, 5)] public float _masDestroyTime = 5f;
    [SerializeField] private float circleRadius;
    [SerializeField] private LayerMask _whatIsPlayers;
    private bool _waitToExitCollider = false;

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

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, circleRadius, _whatIsPlayers);
        if(colliders.Any(c => c.GetComponent<HPScript>() != null))
        {
            _waitToExitCollider = true;
        }
        else
	    {
            TriggerHpRecov();
	    }
	}

    void Update()
    {
        if (_waitToExitCollider)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, circleRadius, _whatIsPlayers);
            if (colliders.All(c => c.gameObject.GetComponent<HPScript>() == null))
            {
                _waitToExitCollider = false;
                TriggerHpRecov();
            }
        }
    }

    private void TriggerHpRecov()
    {
        if (Random.value <= ProbabilityOfHealthPickup)
        {
            var recov = gameObject.AddComponent<HPRecovery>();
            recov.HPRecov = HpRecovered;
            if (_arePickupsToDisapear && Random.value > _probablityOfHealthStayOnGround)
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
