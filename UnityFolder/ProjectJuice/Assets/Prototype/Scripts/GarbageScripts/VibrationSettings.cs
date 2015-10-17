using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class VibrationSettings
{

    [SerializeField]
    private bool _averageBothSides = true;
    [Range(0, 1)]
    [SerializeField]
    private float _leftSideVibration = 0.5f;
    [Range(0,1)]
    [SerializeField]
    private float _rightSideVibration = 0.5f;
    [Range(0, 1)]
    [SerializeField]
    private float _timeToVibrate = 0.3f;

    public float LeftSideVibration
    {
        get
        {
            var ret = _averageBothSides ? (_leftSideVibration + _rightSideVibration) / 2 : _leftSideVibration;
            return ret;
        }
    }
    public float RightSideVibration
    {
        get
        {
            var ret = _averageBothSides ? (_leftSideVibration + _rightSideVibration) / 2 : _rightSideVibration;
            return ret;
        }
    }
    public float TimeToVibrate { get { return _timeToVibrate; } }



}
