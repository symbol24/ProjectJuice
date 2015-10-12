using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(ExplosiveObject))]
public class ExplosiveObjectEditor : Editor {

    string[] _listOfClips;

    int[] _choice = new int[4];

    string[] _listOFParticles;

    int[] _particle = new int[4];

    public override void OnInspectorGUI()
    {
        _listOfClips = EditorUtilities.GetListOfAudioClips();
        _listOFParticles = EditorUtilities.GetListOfParticles();
        DrawDefaultInspector();

        var explosif = target as ExplosiveObject;

        _choice[0] = EditorUtilities.GetSelectedClip(_listOfClips, explosif.Pushing);
        _choice[1] = EditorUtilities.GetSelectedClip(_listOfClips, explosif.GroundImpact);
        _choice[2] = EditorUtilities.GetSelectedClip(_listOfClips, explosif.BulletImpact);
        _choice[3] = EditorUtilities.GetSelectedClip(_listOfClips, explosif.Explosion);

        _particle[0] = EditorUtilities.GetSelectedParticle(explosif._groundScraping);
        _particle[1] = EditorUtilities.GetSelectedParticle(explosif._explosionFX);
        _particle[2] = EditorUtilities.GetSelectedParticle(explosif._shockwave);
        _particle[3] = EditorUtilities.GetSelectedParticle(explosif._chromaticAberation);

        _choice[0] = EditorGUILayout.Popup("Being Pushed SFX", _choice[0], _listOfClips);
        _choice[1] = EditorGUILayout.Popup("Ground Impact SFX", _choice[1], _listOfClips);
        _choice[2] = EditorGUILayout.Popup("Damaging SFX", _choice[2], _listOfClips);
        _choice[3] = EditorGUILayout.Popup("Explosion SFX", _choice[3], _listOfClips);

        _particle[0] = EditorGUILayout.Popup("Ground Scrapping Particle", _particle[0], _listOFParticles);
        _particle[1] = EditorGUILayout.Popup("Explosion Particle", _particle[1], _listOFParticles);
        _particle[2] = EditorGUILayout.Popup("Shockwave Particle", _particle[2], _listOFParticles);
        _particle[3] = EditorGUILayout.Popup("Chromatic Aberation Particle", _particle[3], _listOFParticles);

        explosif.Pushing = _listOfClips[_choice[0]];
        explosif.GroundImpact = _listOfClips[_choice[1]];
        explosif.BulletImpact = _listOfClips[_choice[2]];
        explosif.Explosion = _listOfClips[_choice[3]];

        explosif._groundScraping = Database.instance.Particles[_particle[0]];
        explosif._explosionFX = Database.instance.Particles[_particle[1]];
        explosif._shockwave = Database.instance.Particles[_particle[2]];
        explosif._chromaticAberation = Database.instance.Particles[_particle[3]];

        EditorUtility.SetDirty(target);
    }
}
