using UnityEngine;
using System.Collections;
using System;

public class ExplosiveObjectDetector : ExtendedMonobehaviour
{
    private IPlatformer2DUserControl _inputManager;
    public GameObject _refForPushing;
    public float _rayCastLength = 1;

    // Use this for initialization
    void Start()
    {
        _inputManager = GetComponent<IPlatformer2DUserControl>();
    }

    // Update is called once per frame
    void Update()
    {
        var direction = _inputManager.m_FacingRight ? transform.right : -transform.right;
        Debug.DrawRay(_refForPushing.transform.position, direction);
        var hit = Physics2D.Raycast(_refForPushing.transform.position, direction, _rayCastLength);
        if(hit.collider != null)
        {
            DetectedCollider(hit.collider.gameObject);
            //print(hit.collider.gameObject.name);
        }
        else
        {
            StopCollider();            
        }
    }

    private void StopCollider()
    {
        if (_groundMover != null)
        {
            _groundMover.X_PositionChanged -= GroundMoverOnPositionChanged;
        }
        _groundMover = null;
        _subscribedCollider = null;
    }

    private ExplosiveObject _subscribedCollider;
    private MovementChecker _groundMover;
    private void DetectedCollider(GameObject gameObject)
    {
        var testExplosive = gameObject.GetComponentInParent<ExplosiveObject>();
        //print(test);
        if(testExplosive != null)
        {
            var getGroundMover = gameObject.GetComponentInParent<MovementChecker>();
            _subscribedCollider = testExplosive;
            _groundMover = getGroundMover;
            _groundMover.X_PositionChanged += GroundMoverOnPositionChanged;
        }
    }

    private void GroundMoverOnPositionChanged(object sender, PositionChangedEventArgs e)
    {
        if (_subscribedCollider != null)
        {
            if (_subscribedCollider.IsExplosive) OnMovingExplosiveObject();
            else OnMovingNonExplosiveObject();
        }
    }

    public event EventHandler MovingExplosiveObject;
    protected virtual void OnMovingExplosiveObject()
    {
        if (MovingExplosiveObject != null) MovingExplosiveObject(this, EventArgs.Empty);
    }
    public event EventHandler MovingNonExplosiveObject;
    protected virtual void OnMovingNonExplosiveObject()
    {
        if (MovingNonExplosiveObject != null) MovingNonExplosiveObject(this, EventArgs.Empty);
    }
}
