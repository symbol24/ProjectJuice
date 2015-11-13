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
    [HideInInspector]
    public bool _enableDashOnGroundVibration = false;
    [HideInInspector]
    public VibrationSettings _dashOnGroundVibrationSettings;
    [HideInInspector]
    public bool _enableDashInAirVibration = false;
    [HideInInspector]
    public VibrationSettings _dashInAirVibrationSettings;

	// Use this for initialization
	protected override void Start ()
	{
	    base.Start();
        _componentToListen.GroundedChanged += ComponentToListenOnGroundedChanged;
        _componentToListen.PlayerDashed += _componentToListen_PlayerDashed;
	}

    private void _componentToListen_PlayerDashed(object sender, EventArgs e)
    {
        if(_componentToListen.IsGrounded && _enableDashOnGroundVibration)
        {
            _playerVibrator.Vibrate(_dashOnGroundVibrationSettings);
        }
        if(!_componentToListen.IsGrounded && _enableDashInAirVibration)
        {
            _playerVibrator.Vibrate(_dashInAirVibrationSettings);
        }
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
