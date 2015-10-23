using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Gun))]
public class GunEditor : Editor {

    string[] _listOfClips;

    int[] _choice = new int[2];

    string[] _listOFParticles;

    int[] _particle = new int[2];

    public override void OnInspectorGUI()
    {
        _listOfClips = EditorUtilities.GetListOfAudioClips();
        _listOFParticles = EditorUtilities.GetListOfParticles();

        DrawDefaultInspector();

        var gun = target as Gun;

        _choice[0] = EditorUtilities.GetSelectedClip(_listOfClips, gun.GunShot);
        _choice[1] = EditorUtilities.GetSelectedClip(_listOfClips, gun.GunReloaded);
        _particle[0] = EditorUtilities.GetSelectedParticle(gun.m_MuzzleFlash);
        _particle[1] = EditorUtilities.GetSelectedParticle(gun.m_MuzzleSmoke);

        _choice[0] = EditorGUILayout.Popup("Gun Shot SFX", _choice[0], _listOfClips);
        _choice[1] = EditorGUILayout.Popup("Gun Reloaded SFX", _choice[1], _listOfClips);
        _particle[0] = EditorGUILayout.Popup("Muzzle Flash", _particle[0], _listOFParticles);
        _particle[1] = EditorGUILayout.Popup("Muzzle SMoke", _particle[1], _listOFParticles);

        gun.GunShot = _listOfClips[_choice[0]];
        gun.GunReloaded = _listOfClips[_choice[1]];
        gun.m_MuzzleFlash = Database.instance.Particles[_particle[0]];
        gun.m_MuzzleSmoke = Database.instance.Particles[_particle[1]];

        EditorUtility.SetDirty(target);
    }
}
