using System;
using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

public class PlayerJumpVibration : VibrationBase<PlatformerCharacter2D>
{
    [HideInInspector] 
    public bool _enableLandingVibration = true;
    [HideInInspector]
    public VibrationSettings _landingVibrationSettings;
    [HideInInspector]
    public bool _enableJumpingVibration = false;
    [HideInInspector]
    public VibrationSettings _jumpingVibrationSettings;

	// Use this for initialization
	protected override void Start ()
	{
	    base.Start();
        _componentToListen.GroundedChanged += ComponentToListenOnGroundedChanged;
	}

    private void ComponentToListenOnGroundedChanged(object sender, BoolEventArgs boolEventArgs)
    {
        if (boolEventArgs.NewBoolValue && _enableLandingVibration) //Detected landing
        {
            _playerVibrator.Vibrate(_landingVibrationSettings);
        }
        if (!boolEventArgs.NewBoolValue && _enableJumpingVibration)
        {
            _playerVibrator.Vibrate(_jumpingVibrationSettings);
        }
    }
}
