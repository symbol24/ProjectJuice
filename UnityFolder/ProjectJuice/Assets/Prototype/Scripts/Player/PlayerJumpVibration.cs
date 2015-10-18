using System;
using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

public class PlayerJumpVibration : MonoBehaviour
{

    [SerializeField] private PlatformerCharacter2D _platformerChar;
    [SerializeField] private PlayerVibrator _playerVibrator;

    [SerializeField] private bool _enableLandingVibration = true;
    [SerializeField] private VibrationSettings _landingVibrationSettings;
    [SerializeField] private bool _enableJumpingVibration = false;
    [SerializeField] private VibrationSettings _jumpingVibrationSettings;

	// Use this for initialization
	void Start ()
	{
	    if (_platformerChar == null) _platformerChar = GetComponent<PlatformerCharacter2D>();
	    if (_platformerChar == null) Debug.LogWarning("CharController not found");
	    if (_playerVibrator == null) _playerVibrator = GetComponent<PlayerVibrator>();
	    if (_playerVibrator == null) Debug.LogWarning("Where is my vibrator? :'(");
        _platformerChar.GroundedChanged += PlatformerCharOnGroundedChanged;
	}

    private void PlatformerCharOnGroundedChanged(object sender, BoolEventArgs boolEventArgs)
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
