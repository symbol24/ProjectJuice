using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Dart))]
public class DartEditor : Editor {

    string[] _listOfClips;

    int[] _choice = new int[3];

    public override void OnInspectorGUI()
    {
        _listOfClips = EditorUtilities.GetListOfAudioClips();
        DrawDefaultInspector();

        var dart = target as Dart;

        _choice[0] = EditorUtilities.GetSelectedClip(_listOfClips, dart.PlayerImpact);
        _choice[1] = EditorUtilities.GetSelectedClip(_listOfClips, dart.GroundImpact);
        _choice[2] = EditorUtilities.GetSelectedClip(_listOfClips, dart.Severed);

        _choice[0] = EditorGUILayout.Popup("Player Impact SFX", _choice[0], _listOfClips);
        _choice[1] = EditorGUILayout.Popup("Ground Impact SFX", _choice[1], _listOfClips);
        _choice[2] = EditorGUILayout.Popup("Severed SFX", _choice[2], _listOfClips);

        dart.PlayerImpact = _listOfClips[_choice[0]];
        dart.GroundImpact = _listOfClips[_choice[1]];
        dart.Severed = _listOfClips[_choice[2]];

        EditorUtility.SetDirty(target);
    }
}
