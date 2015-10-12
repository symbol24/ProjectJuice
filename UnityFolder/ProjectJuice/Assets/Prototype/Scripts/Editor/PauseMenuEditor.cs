using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(PauseMenu))]
public class PauseMenuEditor : Editor {
    string[] _listOfClips;

    int[] _soundsID = new int[3];

    public override void OnInspectorGUI()
    {
        _listOfClips = EditorUtilities.GetListOfAudioClips();
        DrawDefaultInspector();

        var pauseMenu = target as PauseMenu;

        _soundsID[0] = EditorUtilities.GetSelectedClip(_listOfClips, pauseMenu.MenuOpenName);
        _soundsID[1] = EditorUtilities.GetSelectedClip(_listOfClips, pauseMenu.MenuCloseName);
        _soundsID[2] = EditorUtilities.GetSelectedClip(_listOfClips, pauseMenu.ReturnToCS);

        _soundsID[0] = EditorGUILayout.Popup("OnOpen SFX", _soundsID[0], _listOfClips);
        _soundsID[1] = EditorGUILayout.Popup("OnClose SFX", _soundsID[1], _listOfClips);
        _soundsID[2] = EditorGUILayout.Popup("Return to CharSel SFX", _soundsID[2], _listOfClips);

        pauseMenu.MenuOpenName = _listOfClips[_soundsID[0]];
        pauseMenu.MenuCloseName = _listOfClips[_soundsID[1]];
        pauseMenu.ReturnToCS = _listOfClips[_soundsID[2]];

        EditorUtility.SetDirty(target);
    }
}
