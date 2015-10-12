using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(ArcShooting))]
public class ArcShootingEditor : Editor
{

    string[] _listOfClips;

    int _choice = 0;

    string[] _listOFParticles;

    int[] _particle = new int[2];

    public override void OnInspectorGUI()
    {
        _listOfClips = EditorUtilities.GetListOfAudioClips();
        _listOFParticles = EditorUtilities.GetListOfParticles();
        DrawDefaultInspector();

        var gun = target as ArcShooting;

        _choice = EditorUtilities.GetSelectedClip(_listOfClips, gun.GunShot);
        _particle[0] = EditorUtilities.GetSelectedParticle(gun.m_MuzzleFlash);
        _particle[1] = EditorUtilities.GetSelectedParticle(gun.m_MuzzleSmoke);

        _choice = EditorGUILayout.Popup("Gun Shot SFX", _choice, _listOfClips);
        _particle[0] = EditorGUILayout.Popup("Muzzle Flash", _particle[0], _listOFParticles);
        _particle[1] = EditorGUILayout.Popup("Muzzle SMoke", _particle[1], _listOFParticles);

        gun.GunShot = _listOfClips[_choice];
        gun.m_MuzzleFlash = Database.instance.Particles[_particle[0]];
        gun.m_MuzzleSmoke = Database.instance.Particles[_particle[1]];

        EditorUtility.SetDirty(target);
    }
}