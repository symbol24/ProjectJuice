using System;
using UnityEngine;
using System.Collections;

public class MeleeAttackCameraShake : CameraShakeBase<MeleeAttack>
{
    [SerializeField] private bool _activarChoqueMovimientoCamara;
    [SerializeField] private CameraShakeSettings _movimientoChoqueCamaraParametros;

    protected override void Start()
    {
        base.Start();
        _componentToListenTo.MeleeClashed += ComponentToListenToOnMeleeClashed;
    }

    private void ComponentToListenToOnMeleeClashed(object sender, EventArgs eventArgs)
    {
        if(_activarChoqueMovimientoCamara) _cameraShaker.DoShake(_movimientoChoqueCamaraParametros);
    }
}
