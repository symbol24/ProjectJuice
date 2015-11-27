using System;
using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;
using System.Linq;

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
    [HideInInspector]
    public bool _enableAxeHitGround;
    [HideInInspector]
    public VibrationSettings _axeHitGroundSettings;
    [HideInInspector]
    public bool _enableDaggerHitPlayerVibration;
    [HideInInspector]
    public VibrationSettings _DaggerhitPlayerVibrations;
    [HideInInspector]
    public bool _enableAxeHitPlayerVibration;
    [HideInInspector]
    public VibrationSettings _AxehitPlayerVibrations;
    [HideInInspector]
    public bool _enableAxeHitGroundForOthers;
    [HideInInspector]
    public VibrationSettings _axeHitGroundForOthersSettings;

    private MeleeAttack _daggerAttack;
    private MeleeAttack _axeAttack;


    // Use this for initialization
    protected override void Start ()
	{
        base.Start();
        var meleeAttacks = GetComponents<MeleeAttack>();
        _daggerAttack = meleeAttacks.First(c => !c.isAbility);
        _axeAttack = meleeAttacks.First(c => c.isAbility);
        _daggerAttack.MeleeClashed += ComponentToListenOnMeleeClashed;
        _axeAttack.MeleeClashed += ComponentToListenOnMeleeClashed;
        _daggerAttack.MeleeConsumed += _componentToListen_MeleeConsumed;
        _axeAttack.MeleeConsumed += _componentToListen_MeleeConsumed;
        _daggerAttack.MeleeSpecialHitGround += _componentToListen_MeleeSpecialHitGround;
        _axeAttack.MeleeSpecialHitGround += _componentToListen_MeleeSpecialHitGround;
        _daggerAttack.MeleeStarted += _componentToListen_MeleeStarted;
        _axeAttack.MeleeStarted += _componentToListen_MeleeStarted;
    }

    private void _componentToListen_MeleeStarted(object sender, EventArgs e)
    {
        var meleeScript = sender as MeleeAttack;
        if (meleeScript.isAbility)
        {
            if (_enableAxeMelee) _playerVibrator.Vibrate(_axeMeleeSettings);
        }
        else
        {
            if (_enableDaggerMelee) _playerVibrator.Vibrate(_daggerMeleeSettings);
        }
    }

    private void _componentToListen_MeleeSpecialHitGround(object sender, EventArgs e)
    {
        var meleeScript = sender as MeleeAttack;
        if(meleeScript.isAbility)
        {
            if (_enableAxeHitGround) _playerVibrator.Vibrate(_axeHitGroundSettings);
            if (_enableAxeHitGroundForOthers)
            {
                var vibrators = FindObjectsOfType<PlayerVibrator>();
                foreach (var vibrator in vibrators)
                {
                    if (vibrator.GetInstanceID() != _playerVibrator.GetInstanceID())
                    {
                        var checkIfGrounded = vibrator.GetComponent<PlatformerCharacter2D>();
                        if (checkIfGrounded.IsGrounded) vibrator.Vibrate(_axeHitGroundForOthersSettings);
                    }
                }
            }
        }
    }

    private void _componentToListen_MeleeConsumed(object sender, ConsumingObjectEventArgs e)
    {
        var meleeScript = sender as MeleeAttack;
        var checkPlayer = e.AttemptingToConsume as HPScript;
        if (checkPlayer != null)
        {
            if (meleeScript.isAbility)
            {
                if (_enableAxeHitPlayerVibration) _playerVibrator.Vibrate(_AxehitPlayerVibrations);
            }
            else
            {
                if (_enableDaggerHitPlayerVibration) _playerVibrator.Vibrate(_DaggerhitPlayerVibrations);
            }
        }
    }

    private void ComponentToListenOnMeleeClashed(object sender, EventArgs eventArgs)
    {
        if (_activarChoqueCuerpoACuerpo) _playerVibrator.Vibrate(_choqueCuerpoACuerpoParametros);
    }
}
