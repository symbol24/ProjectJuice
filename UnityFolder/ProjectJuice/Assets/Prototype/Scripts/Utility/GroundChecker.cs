using System;
using System.Linq;
using UnityEngine;
using System.Collections;

public class GroundChecker : ExtendedMonobehaviour {

    public GameObject PointOfCollision { get; private set; }
    [SerializeField] private float _radius = 1.1f;
    
    [SerializeField]
    private bool _enableDebug = false;

    // Use this for initialization
    private void Start()
    {
        PointOfCollision = new GameObject(Guid.NewGuid().ToString());
        PointOfCollision.transform.parent = transform;
    }

    // Update is called once per frame
	void Update ()
	{
        DetectIfGrounded();
	}


    private void DetectIfGrounded()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, _radius);
        IsGrounded = colliders.Any(c => c.GetComponent<Ground>() != null);
        if(_enableDebug) print("CheckingIsGrounded " + IsGrounded);
        
    }

    private bool _isGrounded;
    public bool IsGrounded
    {
        get { return _isGrounded; }
        set
        {
            if (_isGrounded != value)
            {
                _isGrounded = value;
                OnGroundChanged(new GroundedChangedEventArgs {Grounded = value});
            }
        }
    }

    public EventHandler<GroundedChangedEventArgs> GroundChanged;
    public void OnGroundChanged(GroundedChangedEventArgs e)
    {
        if (GroundChanged != null) GroundChanged(this, e);
    }
}

public class GroundedChangedEventArgs : EventArgs
{
    public bool Grounded { get; set; }
}
