using UnityEngine;
using System.Collections;

public class GunVibration : VibrationBase<Gun>
{
    [HideInInspector]
    public bool _enableFireVibration = true;
    [HideInInspector]
    public VibrationSettings _fireVibrationSettings;


	// Use this for initialization
	protected override void Start ()
    {
        base.Start();
        if (_enableFireVibration) _componentToListen.BulletFired += ComponentToListenBulletFired;
	}

    private void ComponentToListenBulletFired(object sender, BulletFiredEventArgs e)
    {
        _playerVibrator.Vibrate(_fireVibrationSettings);
    }

    // Update is called once per frame
    void Update () {
	
	}
}
