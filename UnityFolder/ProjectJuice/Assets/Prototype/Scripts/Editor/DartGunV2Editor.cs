using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(DartGunV2))]
//[CanEditMultipleObjects]
public class DartGunV2Editor : ExtendedEditor
{
    string[] _listOfClips;

    int[] _choice = new int[4];

    string[] _listOFParticles;

    int[] _particle = new int[1];

    public override void OnInspectorGUI()
    {
        var leDartGun = (DartGunV2) target;
        DrawDefaultInspector();
        _listOfClips = EditorUtilities.GetListOfAudioClips();
        _listOFParticles = EditorUtilities.GetListOfParticles();

        _choice[0] = EditorUtilities.GetSelectedClip(_listOfClips, leDartGun.Fire);
        _choice[1] = EditorUtilities.GetSelectedClip(_listOfClips, leDartGun.Transfering);
        _choice[2] = EditorUtilities.GetSelectedClip(_listOfClips, leDartGun.CoolDown);

        _particle[0] = EditorUtilities.GetSelectedParticle(leDartGun.m_firingParticle);

        


        AddTitle("Dart");
        leDartGun.Settings.DartForce = EditorGUILayout.FloatField("Initial Force", leDartGun.Settings.DartForce);
        leDartGun.Settings.DartMaxDistanceTravelled = EditorGUILayout.FloatField("Max Distance to Travel", leDartGun.Settings.DartMaxDistanceTravelled);
        leDartGun.Settings.HpSuckedPerSecond = EditorGUILayout.FloatField("Hp per second", leDartGun.Settings.HpSuckedPerSecond);
        leDartGun.Settings._dartDestroyTimer = EditorGUILayout.FloatField("DestroyOnImpactTimer", leDartGun.Settings._dartDestroyTimer);

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
        leDartGun.Settings.MinimumCrossSectionsToSpawn = EditorGUILayout.FloatField("Minimum CrossSections per shot", leDartGun.Settings.MinimumCrossSectionsToSpawn);

        AddTitle("Dart Gun");
        leDartGun.Settings.m_transferSoundDelay = EditorGUILayout.FloatField("Transfer sound delay", leDartGun.Settings.m_transferSoundDelay);
        leDartGun.Settings._shootDelay = EditorGUILayout.FloatField("Cooldown", leDartGun.Settings._shootDelay);
        
        AddTitle("SoundFX");
        _choice[0] = EditorGUILayout.Popup("Fire Dart", _choice[0], _listOfClips);
        _choice[1] = EditorGUILayout.Popup("Transfering Juice", _choice[1], _listOfClips);
        _choice[2] = EditorGUILayout.Popup("Cooldown", _choice[2], _listOfClips);
        _choice[3] = EditorGUILayout.Popup("Wire Snap", _choice[3], _listOfClips);
        leDartGun.Settings.LoopCooldown = EditorGUILayout.Toggle("Loop Cooldown", leDartGun.Settings.LoopCooldown);
        leDartGun.Fire = _listOfClips[_choice[0]];
        leDartGun.Transfering = _listOfClips[_choice[1]];
        leDartGun.CoolDown = _listOfClips[_choice[2]];
        leDartGun.WireSnapping = _listOfClips[_choice[3]];

        AddTitle("Particles");
        _particle[0] = EditorGUILayout.Popup("Firing", _particle[0], _listOFParticles);
        leDartGun.m_firingParticle = Database.instance.Particles[_particle[0]];
        
        AddTitle("DEBUG");
        leDartGun.Settings.HoseBrakeOnCollision = EditorGUILayout.Toggle("Break on Collision (Hose)", leDartGun.Settings.HoseBrakeOnCollision);
        leDartGun.Settings.DartInvencible = EditorGUILayout.Toggle("Fire on click", leDartGun.Settings.DartInvencible);

    }
}
