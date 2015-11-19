using UnityEngine;
using System.Collections;

public class ExplosiveObjectDetectorVibrations : VibrationBase<ExplosiveObjectDetector>
{
    [HideInInspector]
    public bool _enableExplosiveVibration;
    [HideInInspector]
    public VibrationSettings _explosiveVibrationSettings;
    [HideInInspector]
    public bool _enableNonExplosiveVibration;
    [HideInInspector]
    public VibrationSettings _nonExplosiveVibrationSettings;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        _componentToListen.MovingNonExplosiveObject += _componentToListen_MovingNonExplosiveObject;
        _componentToListen.MovingExplosiveObject += _componentToListen_MovingExplosiveObject;
    }

    private void _componentToListen_MovingExplosiveObject(object sender, System.EventArgs e)
    {
        if (_enableExplosiveVibration) _playerVibrator.Vibrate(_explosiveVibrationSettings);
    }

    private void _componentToListen_MovingNonExplosiveObject(object sender, System.EventArgs e)
    {
        if (_enableNonExplosiveVibration) _playerVibrator.Vibrate(_nonExplosiveVibrationSettings);
    }
}
