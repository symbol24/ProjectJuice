using UnityEngine;
using System.Collections;

public class HpScriptCameraShake : CameraShakeBase<HPScript>
{
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
    protected override void Start()
    {
        base.Start();
        if (_enableImpactShake && _cameraShaker != null && _componentToListenTo != null) _componentToListenTo.HpImpactReceived += ComponentToListenToHpImpactReceived;
        if (_enableHpChangedShake && _cameraShaker != null && _componentToListenTo != null) _componentToListenTo.HpChanged += ComponentToListenToHpChanged;
        if (_enableDeadShake && _cameraShaker != null && _componentToListenTo != null) _componentToListenTo.Dead += ComponentToListenToDead;
    }

    private void ComponentToListenToDead(object sender, System.EventArgs e)
    {
        _cameraShaker.DoShake(_deadShakeSettings);
    }

    private void ComponentToListenToHpChanged(object sender, HpChangedEventArgs e)
    {
        if (e.NewHp < e.PreviousHp) _cameraShaker.DoShake(_hpChangedShakeSettings);
    }

    private void ComponentToListenToHpImpactReceived(object sender, ImpactEventArgs e)
    {
        _cameraShaker.DoShake(_impactShakeSettings);
    }
}
