using UnityEngine;
using System.Collections;

public class HpScriptVibration : VibrationBase<HPScript>
{
    [SerializeField]
    private bool _enableImpactVibration;
    [SerializeField]
    private VibrationSettings _impactVibrationSettings;
    [SerializeField]
    private bool _enableDeadVibration = true;
    [SerializeField]
    private VibrationSettings _deadVibrationSettings;
    [SerializeField]
    private bool _enableHpChangedVibration;
    [SerializeField]
    private VibrationSettings _hpChangedVibrationSettings;


    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        if (_enableDeadVibration) _componentToListen.Dead += _hpScript_Dead;
        if (_enableHpChangedVibration) _componentToListen.HpChanged += _hpScript_HpChanged;
        if (_enableImpactVibration) _componentToListen.HpImpactReceived += _hpScript_HpImpactReceived;
    }

    private void _hpScript_HpImpactReceived(object sender, ImpactEventArgs e)
    {
        _playerVibrator.Vibrate(_impactVibrationSettings);
    }

    private void _hpScript_HpChanged(object sender, HpChangedEventArgs e)
    {
        if (e.NewHp < e.PreviousHp) _playerVibrator.Vibrate(_hpChangedVibrationSettings);
    }

    private void _hpScript_Dead(object sender, System.EventArgs e)
    {
        _playerVibrator.Vibrate(_deadVibrationSettings, true);
    }
}
