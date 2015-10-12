using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(ExplosiveObject))]
public class ExplosiveObjectEditor : Editor {

    string[] _listOfClips;

    int[] _choice = new int[4];

    public override void OnInspectorGUI()
    {
        _listOfClips = EditorUtilities.GetListOfAudioClips();
        DrawDefaultInspector();

        var explosif = target as ExplosiveObject;

        _choice[0] = EditorUtilities.GetSelectedClip(_listOfClips, explosif.Pushing);
        _choice[1] = EditorUtilities.GetSelectedClip(_listOfClips, explosif.GroundImpact);
        _choice[2] = EditorUtilities.GetSelectedClip(_listOfClips, explosif.BulletImpact);
        _choice[3] = EditorUtilities.GetSelectedClip(_listOfClips, explosif.Explosion);

        _choice[0] = EditorGUILayout.Popup("Being Pushed SFX", _choice[0], _listOfClips);
        _choice[1] = EditorGUILayout.Popup("Ground Impact SFX", _choice[1], _listOfClips);
        _choice[2] = EditorGUILayout.Popup("Damaging SFX", _choice[2], _listOfClips);
        _choice[3] = EditorGUILayout.Popup("Explosion SFX", _choice[3], _listOfClips);

        explosif.Pushing = _listOfClips[_choice[0]];
        explosif.GroundImpact = _listOfClips[_choice[1]];
        explosif.BulletImpact = _listOfClips[_choice[2]];
        explosif.Explosion = _listOfClips[_choice[3]];

        EditorUtility.SetDirty(target);
    }
}
