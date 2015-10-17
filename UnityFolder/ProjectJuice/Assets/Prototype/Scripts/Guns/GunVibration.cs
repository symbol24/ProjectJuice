using UnityEngine;
using System.Collections;

public class GunVibration : MonoBehaviour {

    [SerializeField]
    private Gun _gun;
    [SerializeField]
    private PlayerVibrator _playerVibrator;
    [SerializeField]
    private bool _enableFireVibration = true;
    [SerializeField]
    private VibrationSettings _fireVibrationSettings;


	// Use this for initialization
	void Start ()
    {
        if (_gun == null) _gun = GetComponent<Gun>();
        if (_gun == null) Debug.LogWarning("GunNotFound! Vibrator looking ;)");
        if (_playerVibrator == null) _playerVibrator = GetComponent<PlayerVibrator>();
        if (_playerVibrator == null) Debug.LogWarning("PlayerVibrator not found. Gun is sad :(");

        if (_enableFireVibration) _gun.BulletFired += _gun_BulletFired;

	}

    private void _gun_BulletFired(object sender, BulletFiredEventArgs e)
    {
        _playerVibrator.Vibrate(_fireVibrationSettings);
    }

    // Update is called once per frame
    void Update () {
	
	}
}
