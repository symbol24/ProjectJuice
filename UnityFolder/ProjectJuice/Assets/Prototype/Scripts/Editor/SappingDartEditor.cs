using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(SappingDartGun))]
public class SappingDartEditor : Editor {

    string[] _listOfClips;

    int[] _choice = new int[3];

    public override void OnInspectorGUI()
    {
        _listOfClips = EditorUtilities.GetListOfAudioClips();
        DrawDefaultInspector();

        var dart = target as SappingDartGun;

        _choice[0] = EditorUtilities.GetSelectedClip(_listOfClips, dart.Fire);
        _choice[1] = EditorUtilities.GetSelectedClip(_listOfClips, dart.Transfering);
        _choice[2] = EditorUtilities.GetSelectedClip(_listOfClips, dart.CoolDown);

        _choice[0] = EditorGUILayout.Popup("Fire Dart SFX", _choice[0], _listOfClips);
        _choice[1] = EditorGUILayout.Popup("Transfering Juice SFX", _choice[1], _listOfClips);
        _choice[2] = EditorGUILayout.Popup("Cooldown SFX", _choice[2], _listOfClips);

        dart.Fire = _listOfClips[_choice[0]];
        dart.Transfering = _listOfClips[_choice[1]];
        dart.CoolDown = _listOfClips[_choice[2]];

        EditorUtility.SetDirty(target);
    }
}
