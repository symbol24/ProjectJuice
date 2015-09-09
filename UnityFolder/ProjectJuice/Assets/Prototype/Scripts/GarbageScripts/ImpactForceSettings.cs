using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class ImpactForceSettings
{
    [SerializeField]
    private float _timeToApplyForce;
    [SerializeField]
    private float _timeToApplyForceAfterMainForce;
    [SerializeField]
    private Vector2 _impactForce;
    [Range(0, 1)]
    [SerializeField]
    private float _duringImpactSpeedMitigator = 1;
    [Range(0,1)]
    [SerializeField]
    private float _afterImpactSpeedMitigator = 1;
    [SerializeField]
    private bool _zeroVelocityAtEnd;

    public float TimeToApplyForce
    {
        get { return _timeToApplyForce; }
        set { _timeToApplyForce = value; }
    }

    public float TimeToApplyForceAfterMainForce
    {
        get { return _timeToApplyForceAfterMainForce; }
        set { _timeToApplyForceAfterMainForce = value; }
    }

    public Vector2 ImpactForce
    {
        get { return _impactForce; }
        set { _impactForce = value; }
    }

    public float DuringImpactSpeedMitigator
    {
        get { return _duringImpactSpeedMitigator; }
        set { _duringImpactSpeedMitigator = value; }
    }

    public float AfterImpactSpeedMitigator
    {
        get { return _afterImpactSpeedMitigator; }
        set { _afterImpactSpeedMitigator = value; }
    }

    public bool ZeroVelocityAtEnd
    {
        get { return _zeroVelocityAtEnd; }
        set { _zeroVelocityAtEnd = value; }
    }
}
