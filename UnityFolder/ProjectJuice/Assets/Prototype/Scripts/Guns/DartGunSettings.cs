using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class DartGunSettings
{
    public float HoseLength;
    public float HoseLengthTolerance;
    public float m_transferSoundDelay = 0.4f;
    public float _shootDelay = 0.5f;
    public float DartMaxDistanceTravelled;
    public float HpSuckedPerSecond;
    public float DartForce;
    public float JointLength;
    public float _dartDestroyTimer;
    public float HoseTimerToActivateTolerance;
    public float HoseWeightAtcollision = 0.1f;
    public float HoseFlatForceMultiplier;
    public float HoseMitigator;
    public float HoseDistancePow;
    public float HoseApplyForceThresholdCorrection;
    public float HosePowBaseCorrection;
    public float HoseOriginalDirectionWeight;
    public bool HoseBrakeOnCollision;
    public bool DartInvencible;
    public float MinimumCrossSectionsToSpawn;
}
