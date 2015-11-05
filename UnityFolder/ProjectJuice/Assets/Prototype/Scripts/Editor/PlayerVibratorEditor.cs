using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(PlayerVibrator))]
[CanEditMultipleObjects]
public class PlayerVibratorEditor : Editor
{
    private DartVibration _dartVib;
    private GunVibration _gunVib;
    private HpScriptVibration _hpVib;
    private MeleeAttackVibration _meleeVib;
    private PlayerJumpVibration _jumpVib;
    private ShieldVibration _shieldVib;
    public void OnEnable()
    {
        var vibrator = (PlayerVibrator)target;
        _dartVib = vibrator.gameObject.GetComponent<DartVibration>();
        _gunVib = vibrator.gameObject.GetComponent<GunVibration>();
        _hpVib = vibrator.gameObject.GetComponent<HpScriptVibration>();
        _meleeVib = vibrator.gameObject.GetComponent<MeleeAttackVibration>();
        _jumpVib = vibrator.gameObject.GetComponent<PlayerJumpVibration>();
        _shieldVib = vibrator.gameObject.GetComponent<ShieldVibration>();
    }

    public override void OnInspectorGUI()
    {
        var vibrator = (PlayerVibrator) target;
        if (_dartVib != null)
        {
            AddTitle("Dart Vibration");
            AddToMenu(ref _dartVib._enableDartCollisionVibration, ref _dartVib._dartCollisionVibrationSettings, "DartCollision");
            AddToMenu(ref _dartVib._enableDartDestroyedVibration, ref _dartVib._dartDestroyedVibrationSettings, "DartDestroyed");
            AddToMenu(ref _dartVib._enableDartJuiceSuckedVibration, ref _dartVib._dartJuiceSuckedVibrationSettings, "DartJuiceSap");
            AddToMenu(ref _dartVib._enableFireVibration, ref _dartVib._fireVibrationSettings, "Disparar");
            EditorGUILayout.Separator();
        }
        if (_gunVib != null)
        {
            AddTitle("Gun Vibration");
            AddToMenu(ref _gunVib._enableFireVibration, ref _gunVib._fireVibrationSettings, "Disparar");
            EditorGUILayout.Separator();
        }
        if (_hpVib != null)
        {
            AddTitle("HP Vibration");
            AddToMenu(ref _hpVib._enableDeadVibration, ref _hpVib._deadVibrationSettings, "Death");
            AddToMenu(ref _hpVib._enableHpChangedVibration, ref _hpVib._hpChangedVibrationSettings, "HpChanged");
            AddToMenu(ref _hpVib._enableImpactVibration, ref _hpVib._impactVibrationSettings, "Impacto");
            EditorGUILayout.Separator();
        }
        if (_meleeVib != null)
        {
            AddTitle("Cuerpo a Cuerpo Vibraciones");
            AddToMenu(ref _meleeVib._activarChoqueCuerpoACuerpo, ref _meleeVib._choqueCuerpoACuerpoParametros, "Choque (clash)");
            EditorGUILayout.Separator();
        }
        if (_jumpVib != null)
        {
            AddTitle("Salto Vibraciones");
            AddToMenu(ref _jumpVib._enableJumpingVibration, ref _jumpVib._jumpingVibrationSettings, "Jump");
            AddToMenu(ref _jumpVib._enableLandingVibration, ref _jumpVib._landingVibrationSettings, "Landing");
            EditorGUILayout.Separator();
        }
        if (_shieldVib != null)
        {
            AddTitle("Shield Vibrations");
            AddToMenu(ref _shieldVib._activarEscudoAbsorveVibraciones, ref _shieldVib._escudoAbsorventeParametros, "EscudoAbsorve");
            AddToMenu(ref _shieldVib._enableShieldChargedVibration, ref _shieldVib._shieldChargedVibrationSettings, "CompletelyCharged");
            AddToMenu(ref _shieldVib._enabledShieldFiredVibration, ref _shieldVib._shieldFiredVibrationSettings, "Fire");
        }

    }

    public void AddToMenu(ref bool vibrationEnabled, ref VibrationSettings vibrationSettings, string title)
    {
        EditorGUILayout.BeginHorizontal();
        vibrationEnabled = EditorGUILayout.Toggle(title,
            vibrationEnabled);



        if (vibrationEnabled)
        {

            vibrationSettings._averageBothSides = EditorGUILayout.Toggle("Average Vibration",
                vibrationSettings._averageBothSides);
            EditorGUILayout.EndHorizontal();

            if (vibrationSettings._averageBothSides)
            {
                var slider = EditorGUILayout.Slider("Vibration",
                    vibrationSettings._leftSideVibration, 0, 1);
                vibrationSettings._leftSideVibration =
                    vibrationSettings._rightSideVibration = slider;
            }
            else
            {
                vibrationSettings._leftSideVibration = EditorGUILayout.Slider("Left Vibration",
                    vibrationSettings._leftSideVibration, 0, 1);
                vibrationSettings._rightSideVibration = EditorGUILayout.Slider("Right Vibration",
                    vibrationSettings._rightSideVibration, 0, 1);
            }
            vibrationSettings._timeToVibrate = EditorGUILayout.FloatField(
                "Time of Vibration", vibrationSettings._timeToVibrate);


        }
        else
        {
            EditorGUILayout.EndHorizontal();
        }
    }

    private void AddTitle(string message)
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(string.Format("*** {0} ***",message), new GUIStyle { fontSize = 15, fontStyle = FontStyle.Bold });
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

}
