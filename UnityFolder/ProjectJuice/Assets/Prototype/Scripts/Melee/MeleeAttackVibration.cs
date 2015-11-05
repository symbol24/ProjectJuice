using System;
using UnityEngine;
using System.Collections;

public class MeleeAttackVibration : VibrationBase<MeleeAttack>
{
    [HideInInspector] public bool _activarChoqueCuerpoACuerpo;
    [HideInInspector] public VibrationSettings _choqueCuerpoACuerpoParametros;

	// Use this for initialization
	protected override void Start ()
	{
        base.Start();
	    _componentToListen.MeleeClashed += ComponentToListenOnMeleeClashed;
	}

    private void ComponentToListenOnMeleeClashed(object sender, EventArgs eventArgs)
    {
        if (_activarChoqueCuerpoACuerpo) _playerVibrator.Vibrate(_choqueCuerpoACuerpoParametros);
    }
}
