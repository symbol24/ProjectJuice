using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Gun))]
public class GunEditor : Editor {

    string[] _listOfClips;

    int _choice = 0;

    public override void OnInspectorGUI()
    {
        _listOfClips = EditorUtilities.GetListOfAudioClips();
        DrawDefaultInspector();

        var gun = target as Gun;

        _choice = EditorUtilities.GetSelectedClip(_listOfClips, gun.GunShot);

        _choice = EditorGUILayout.Popup("Gun Shot SFX", _choice, _listOfClips);

        gun.GunShot = _listOfClips[_choice];

        EditorUtility.SetDirty(target);
    }
}
