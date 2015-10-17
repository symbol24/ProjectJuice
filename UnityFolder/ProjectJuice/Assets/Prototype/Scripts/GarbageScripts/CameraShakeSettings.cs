using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class CameraShakeSettings
{
    [SerializeField]
    float _shakeIntensity = 1.5f;
    [SerializeField]
    float _shakeTime = 0.5f;
    [SerializeField]
    float _mitigationOnShakeAddition = 0.2f;
    [SerializeField]
    float _shakeFlatConstant = 2f;

    public float ShakeIntensity { get { return _shakeIntensity; } }
    public float ShakeTime { get { return _shakeTime; } }
    public float MitigationOnShakeAddition { get { return _mitigationOnShakeAddition; } }
    public float ShakeFlatConstant { get { return _shakeFlatConstant; } }

}
