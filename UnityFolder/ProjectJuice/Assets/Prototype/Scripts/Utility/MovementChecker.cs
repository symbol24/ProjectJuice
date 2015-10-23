using System;
using UnityEngine;
using System.Collections;

public class MovementChecker : MonoBehaviour
{
    private Vector2 _previousPosition;
    private const float ComparisonTolerance = 0.001f;
    private bool _detectingMovement = false;
	// Use this for initialization
	void Start ()
	{
	    _previousPosition = transform.position;
	}
	
	// Update is called once per frame
    private void Update()
    {
        var currentPositon = transform.position.ToVector2();

        if (Math.Abs(_previousPosition.x - currentPositon.x) > 0.01f || Math.Abs(_previousPosition.y - currentPositon.y) > 0.01f)
        {
            _detectingMovement = true;
            var e = new PositionChangedEventArgs {NewPosition = currentPositon, PreviousPosition = _previousPosition};
            if (Math.Abs(_previousPosition.x - currentPositon.x) > ComparisonTolerance && X_PositionChanged != null)
            {
                X_PositionChanged(this, e);
            }
            if (Math.Abs(_previousPosition.y - currentPositon.y) > ComparisonTolerance && Y_PositionChanged != null)
            {
                Y_PositionChanged(this, e);
            }
            if (PositionChanged != null) PositionChanged(this, e);
        }
        else if(_detectingMovement)
        {
            _detectingMovement = false;
            if (MovementStopped != null) MovementStopped(this, EventArgs.Empty);
        }
        _previousPosition = currentPositon;
    }

    public EventHandler<PositionChangedEventArgs> PositionChanged;
    public EventHandler<PositionChangedEventArgs> X_PositionChanged;
    public EventHandler<PositionChangedEventArgs> Y_PositionChanged;
    public EventHandler MovementStopped;


}

public class PositionChangedEventArgs : EventArgs
{
    public Vector2 PreviousPosition { get; set; }
    public Vector2 NewPosition { get; set; }
    public Vector2 Difference { get { return NewPosition - PreviousPosition; } }
}
