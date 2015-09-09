using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class ImpactForceSettings
{
    [SerializeField]
    private float _firstCycleTime = 0.5f;
    [SerializeField]
    private float _secondCycleTime;
    [SerializeField]
    private Vector2 _impactForce = new Vector2(20, 5);
    [Range(0, 1)]
    [SerializeField]
    private float _firstCycleSpeedMitigator = 0.95f;
    [Range(0,1)]
    [SerializeField]
    private float _secondCycleSpeedMitigator = 0.9f;
    [SerializeField]
    private bool _zeroVelocityAtEnd;

    public float FirstCycleTime
    {
        get { return _firstCycleTime; }
        set { _firstCycleTime = value; }
    }

    public float SecondCycleTime
    {
        get { return _secondCycleTime; }
        set { _secondCycleTime = value; }
    }

    public Vector2 ImpactForce
    {
        get { return _impactForce; }
        set { _impactForce = value; }
    }

    public float FirstCycleSpeedMitigator
    {
        get { return _firstCycleSpeedMitigator; }
        set { _firstCycleSpeedMitigator = value; }
    }

    public float SecondCycleSpeedMitigator
    {
        get { return _secondCycleSpeedMitigator; }
        set { _secondCycleSpeedMitigator = value; }
    }

    public bool ZeroVelocityAtEnd
    {
        get { return _zeroVelocityAtEnd; }
        set { _zeroVelocityAtEnd = value; }
    }
    public Direction2D DirectionComingForm { get; set; }
}
