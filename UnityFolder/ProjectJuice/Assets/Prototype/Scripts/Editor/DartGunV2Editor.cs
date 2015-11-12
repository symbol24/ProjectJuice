using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(DartGunV2))]
//[CanEditMultipleObjects]
public class DartGunV2Editor : ExtendedEditor
{
    public override void OnInspectorGUI()
    {
        var leDartGun = (DartGunV2) target;
        DrawDefaultInspector();

        AddTitle("Dart");
        leDartGun.Settings.DartForce = EditorGUILayout.FloatField("Initial Force", leDartGun.Settings.DartForce);
        leDartGun.Settings.DartMaxDistanceTravelled = EditorGUILayout.FloatField("Max Distance to Travel", leDartGun.Settings.DartMaxDistanceTravelled);
        leDartGun.Settings.HpSuckedPerSecond = EditorGUILayout.FloatField("Hp per second", leDartGun.Settings.HpSuckedPerSecond);

        AddTitle("Cross Section");
        leDartGun.Settings.HoseLength = EditorGUILayout.FloatField("Length (to spawn)", leDartGun.Settings.HoseLength);
        leDartGun.Settings.JointLength = EditorGUILayout.FloatField("Length (dist joint)", leDartGun.Settings.JointLength);
        leDartGun.Settings.HoseLengthTolerance = EditorGUILayout.FloatField("Length to destroy", leDartGun.Settings.HoseLengthTolerance);
        leDartGun.Settings.HoseApplyForceThresholdCorrection = EditorGUILayout.FloatField("Threashold for Force Correction", leDartGun.Settings.HoseApplyForceThresholdCorrection);
        leDartGun.Settings.HoseDistancePow = EditorGUILayout.FloatField("Exponential to Distance", leDartGun.Settings.HoseDistancePow);
        leDartGun.Settings.HoseFlatForceMultiplier = EditorGUILayout.FloatField("Flat SwingMultiplier", leDartGun.Settings.HoseFlatForceMultiplier);
        leDartGun.Settings.HoseMitigator = EditorGUILayout.FloatField("Cascade Mitigation", leDartGun.Settings.HoseMitigator);
        leDartGun.Settings.HoseOriginalDirectionWeight = EditorGUILayout.FloatField("Cascade OrigDirec Weight", leDartGun.Settings.HoseOriginalDirectionWeight);
        leDartGun.Settings.HosePowBaseCorrection = EditorGUILayout.FloatField("Base Exponential Adder", leDartGun.Settings.HosePowBaseCorrection);
        leDartGun.Settings.HoseTimerToActivateTolerance = EditorGUILayout.FloatField("TimerAfterCollision for Tolerance", leDartGun.Settings.HoseTimerToActivateTolerance);
        leDartGun.Settings.MinimumCrossSectionsToSpawn = EditorGUILayout.FloatField("Minimum CrossSections per shot", leDartGun.Settings.HoseTimerToActivateTolerance);

        AddTitle("Dart Gun");
        leDartGun.Settings.m_transferSoundDelay = EditorGUILayout.FloatField("Transfer sound delay", leDartGun.Settings.m_transferSoundDelay);
        leDartGun.Settings._shootDelay = EditorGUILayout.FloatField("Shoot Delay", leDartGun.Settings._shootDelay);

        AddTitle("DEBUG");
        leDartGun.Settings.HoseBrakeOnCollision = EditorGUILayout.Toggle("Break on Collision (Hose)", leDartGun.Settings.HoseBrakeOnCollision);
        leDartGun.Settings.DartInvencible = EditorGUILayout.Toggle("Fire on click", leDartGun.Settings.DartInvencible);

    }
}
