using UnityEngine;
using System.Collections;

public class HpScriptCameraShake : MonoBehaviour
{
    [SerializeField]
    private HPScript _hpScript;
    [SerializeField]
    private CameraShaker _cameraShaker;
    [SerializeField]
    private bool _enableImpactShake;
    [SerializeField]
    private CameraShakeSettings _impactShakeSettings;
    [SerializeField]
    private bool _enableDeadShake = true;
    [SerializeField]
    private CameraShakeSettings _deadShakeSettings;
    [SerializeField]
    private bool _enableHpChangedShake;
    [SerializeField]
    private CameraShakeSettings _hpChangedShakeSettings;


    // Use this for initialization
    void Start()
    {
        if (_cameraShaker == null) _cameraShaker = GetComponent<CameraShaker>();
        if (_cameraShaker == null) Debug.LogWarning("NoCameraShakerFound (CameraShake on MainCamera)");
        if (_hpScript == null) _hpScript = GetComponent<HPScript>();
        if (_enableImpactShake && _cameraShaker != null && _hpScript != null) _hpScript.HpImpactReceived += _hpScript_HpImpactReceived;
        if (_enableHpChangedShake && _cameraShaker != null && _hpScript != null) _hpScript.HpChanged += _hpScript_HpChanged;
        if (_enableDeadShake && _cameraShaker != null && _hpScript != null) _hpScript.Dead += _hpScript_Dead;
    }

    private void _hpScript_Dead(object sender, System.EventArgs e)
    {
        _cameraShaker.DoShake(_deadShakeSettings);
    }

    private void _hpScript_HpChanged(object sender, HpChangedEventArgs e)
    {
        if (e.NewHp < e.PreviousHp) _cameraShaker.DoShake(_hpChangedShakeSettings);
    }

    private void _hpScript_HpImpactReceived(object sender, ImpactEventArgs e)
    {
        _cameraShaker.DoShake(_impactShakeSettings);
    }
}
