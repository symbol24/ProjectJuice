using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class ImpactForceSettings
{
    [Range(0,10)][SerializeField]
    private int _impactDrag = 5;
    [SerializeField]
    private float _impactForce = 250f;
    [SerializeField]
    private Vector2 _impactAngle = new Vector2(1, 1);
    [Range(0,1)][SerializeField] private float _impactDragTimer = 0.1f;


    [Range(0, 1)]
    [SerializeField]
    private float _firstCycleSpeedMitigator = 0.95f;
    [Range(0,1)]
    [SerializeField]
    private float _secondCycleSpeedMitigator = 0.9f;
    [SerializeField]
    private bool _zeroVelocityAtEnd;

    [SerializeField] private bool _isFadeDmgOnDistance = false;
    [SerializeField]private float _fadeMaxDmgDistance = 50f;


    public int ImpactDrag
    {
        get { return _impactDrag; }
        set { _impactDrag = value; }
    }

    public float ImpactForce
    {
        get { return _impactForce; }
        set { _impactForce = value; }
    }

    public Vector2 ImpactAngle
    {
        get { return _impactAngle; }
        set { _impactAngle = value; }
    }

    public float ImpactDragTimer { get { return _impactDragTimer; } set { _impactDragTimer = value; } }

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
    public bool IsFadeDmgOnDistance { get { return _isFadeDmgOnDistance; } set { _isFadeDmgOnDistance = value; } }

    public float FadeMaxDmgDistance
    {
        get { return _fadeMaxDmgDistance; }
        set { _fadeMaxDmgDistance = value; }
    }
}
