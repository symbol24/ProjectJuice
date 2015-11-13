using System;
using UnityEngine;
using System.Collections;

public class MeleeAttackVibration : VibrationBase<MeleeAttack>
{
    [HideInInspector] public bool _activarChoqueCuerpoACuerpo;
    [HideInInspector] public VibrationSettings _choqueCuerpoACuerpoParametros;
    [HideInInspector]
    public bool _enableDaggerMelee = false;
    [HideInInspector]
    public VibrationSettings _daggerMeleeSettings;
    [HideInInspector]
    public bool _enableAxeMelee = false;
    [HideInInspector]
    public VibrationSettings _axeMeleeSettings;

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
