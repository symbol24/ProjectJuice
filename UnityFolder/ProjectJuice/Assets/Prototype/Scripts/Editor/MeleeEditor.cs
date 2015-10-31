using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(MeleeAttack))]
public class MeleeEditor : Editor {

    string[] _listOfClips;

    int[] _choice = new int[8];

    string[] _listOFParticles;

    int[] _particle = new int[1];

    public override void OnInspectorGUI()
    {
        _listOfClips = EditorUtilities.GetListOfAudioClips();
        _listOFParticles = EditorUtilities.GetListOfParticles();



        var melee = target as MeleeAttack;

        if (melee.isAbility)
            EditorGUILayout.HelpBox("This is the BATTLE AXE", MessageType.Info);


        DrawDefaultInspector();

        _choice[0] = EditorUtilities.GetSelectedClip(_listOfClips, melee.PlayerImpact);
        _choice[1] = EditorUtilities.GetSelectedClip(_listOfClips, melee.Swipe);
        _choice[2] = EditorUtilities.GetSelectedClip(_listOfClips, melee.Sheath);
        _choice[3] = EditorUtilities.GetSelectedClip(_listOfClips, melee.Clash);
        _choice[4] = EditorUtilities.GetSelectedClip(_listOfClips, melee.ClashCrowd);
        _choice[5] = EditorUtilities.GetSelectedClip(_listOfClips, melee.ClashAftermath);

        if (melee.Trail == null)
            _particle[0] = 0;
        else
            _particle[0] = EditorUtilities.GetSelectedParticle(melee.Trail);


        _choice[0] = EditorGUILayout.Popup("Player Impact SFX", _choice[0], _listOfClips);
        _choice[1] = EditorGUILayout.Popup("Swipe SFX", _choice[1], _listOfClips);
        _choice[2] = EditorGUILayout.Popup("Sheath SFX", _choice[2], _listOfClips);
        _choice[3] = EditorGUILayout.Popup("Clash SFX", _choice[3], _listOfClips);
        _choice[4] = EditorGUILayout.Popup("Clash Crowd SFX", _choice[4], _listOfClips);
        _choice[5] = EditorGUILayout.Popup("Clash Aftermath SFX", _choice[5], _listOfClips);


        if (melee.isAbility)
        {

            _choice[6] = EditorUtilities.GetSelectedClip(_listOfClips, melee.AbilitySecondSound);
            _choice[7] = EditorUtilities.GetSelectedClip(_listOfClips, melee.AbilityAerial);

            _choice[6] = EditorGUILayout.Popup("Ability Secondary SFX", _choice[6], _listOfClips);
            _choice[7] = EditorGUILayout.Popup("Ability Aerial SFX", _choice[7], _listOfClips);

            melee.AbilitySecondSound = _listOfClips[_choice[6]];
            melee.AbilityAerial = _listOfClips[_choice[7]];
        }

        _particle[0] = EditorGUILayout.Popup("Melee Trail FX", _particle[0], _listOFParticles);

        melee.PlayerImpact = _listOfClips[_choice[0]];
        melee.Swipe = _listOfClips[_choice[1]];
        melee.Sheath = _listOfClips[_choice[2]];
        melee.Clash = _listOfClips[_choice[3]];
        melee.ClashCrowd = _listOfClips[_choice[4]];
        melee.ClashAftermath = _listOfClips[_choice[5]];

        melee.Trail = Database.instance.Particles[_particle[0]];

        EditorUtility.SetDirty(target);
    }
}
