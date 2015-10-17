using UnityEngine;
using System.Collections;

public class HpScriptVibration : MonoBehaviour
{
    [SerializeField]
    private HPScript _hpScript;
    [SerializeField]
    private PlayerVibrator _playerVibrator;
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
    void Start()
    {
        if (_hpScript == null) _hpScript = GetComponent<HPScript>();
        if (_hpScript == null) Debug.LogWarning("HpScript not found!");
        if (_playerVibrator == null) _playerVibrator = GetComponent<PlayerVibrator>();
        if (_playerVibrator == null) Debug.LogWarning("NoPlayer Vibrator FOUND! :$ >.<");

        if (_enableDeadVibration) _hpScript.Dead += _hpScript_Dead;
        if (_enableHpChangedVibration) _hpScript.HpChanged += _hpScript_HpChanged;
        if (_enableImpactVibration) _hpScript.HpImpactReceived += _hpScript_HpImpactReceived;
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
