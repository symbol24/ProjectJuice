using System;
using UnityEngine;
using System.Collections;

public class ShieldVibration : VibrationBase<shield>
{
    [HideInInspector]
    public bool _enabledShieldFiredVibration;
    [HideInInspector]
    public VibrationSettings _shieldFiredVibrationSettings;
    [HideInInspector]
    public bool _enableShieldChargedVibration;
    [HideInInspector]
    public VibrationSettings _shieldChargedVibrationSettings;
    [HideInInspector]
    public bool _activarEscudoAbsorveVibraciones;
    [HideInInspector]
    public VibrationSettings _escudoAbsorventeParametros;

    // Use this for initialization
	protected override void Start ()
	{
        base.Start();
        _componentToListen.ShieldFired += ComponentToListenOnComponentToListenFired;
        _componentToListen.BulletAbsorbed += ComponentToListenOnBulletAbsorbed;
	}

    private void ComponentToListenOnBulletAbsorbed(object sender, EventArgs eventArgs)
    {
        if(_activarEscudoAbsorveVibraciones) _playerVibrator.Vibrate(_escudoAbsorventeParametros);
    }

    private void ComponentToListenOnComponentToListenFired(object sender, EventArgs eventArgs)
    {
        if(_enabledShieldFiredVibration) _playerVibrator.Vibrate(_shieldFiredVibrationSettings);
        else _playerVibrator.StopVibration();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_componentToListen.m_CanShootBack && _enableShieldChargedVibration)
        {
            _playerVibrator.Vibrate(_shieldChargedVibrationSettings);
        }
    }
}
