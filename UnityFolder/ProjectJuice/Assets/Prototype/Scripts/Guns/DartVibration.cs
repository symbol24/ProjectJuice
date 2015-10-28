using System;
using UnityEngine;
using System.Collections;

public class DartVibration : VibrationBase<SappingDartGun>
{
    [SerializeField] private bool _enableFireVibration;
    [SerializeField] private VibrationSettings _fireVibrationSettings;
    [SerializeField] private bool _enableDartCollisionVibration;
    [SerializeField] private VibrationSettings _dartCollisionVibrationSettings;
    [SerializeField] private bool _enableDartJuiceSuckedVibration;
    [SerializeField] private VibrationSettings _dartJuiceSuckedVibrationSettings;
    [SerializeField] private bool _enableDartDestroyedVibration;
    [SerializeField] private VibrationSettings _dartDestroyedVibrationSettings;

    protected override void Start()
    {
        base.Start();
        _componentToListen.DartFired += ComponentToListenOnDartFired;
    }

    private void ComponentToListenOnDartFired(object sender, DartFiredEventArgs dartFiredEventArgs)
    {
        dartFiredEventArgs.Dart.DartCollision += DartOnDartCollision;
        dartFiredEventArgs.Dart.JuiceSucked += DartOnJuiceSucked;
        dartFiredEventArgs.Dart.DartDestroyed += DartOnDartDestroyed;
        if (_enableFireVibration) _playerVibrator.Vibrate(_fireVibrationSettings);

    }

    private void DartOnDartDestroyed(object sender, EventArgs eventArgs)
    {
        if(_enableDartDestroyedVibration) _playerVibrator.Vibrate(_dartDestroyedVibrationSettings);
        else _playerVibrator.StopVibration();
    }

    private void DartOnJuiceSucked(object sender, JuiceSuckedEventArgs juiceSuckedEventArgs)
    {
        if(_enableDartJuiceSuckedVibration) _playerVibrator.Vibrate(_dartJuiceSuckedVibrationSettings);
    }

    private void DartOnDartCollision(object sender, EventArgs eventArgs)
    {
        if (_enableDartCollisionVibration) _playerVibrator.Vibrate(_dartCollisionVibrationSettings);
        else _playerVibrator.StopVibration();
    }
}
