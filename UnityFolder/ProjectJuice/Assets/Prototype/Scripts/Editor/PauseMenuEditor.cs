using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(PauseMenu))]
public class PauseMenuEditor : Editor {
    string[] _listOfClips;

    int[] _soundsID = new int[2];

    public override void OnInspectorGUI()
    {
        _listOfClips = EditorUtilities.GetListOfAudioClips();
        DrawDefaultInspector();

        var pauseMenu = target as PauseMenu;

        _soundsID[0] = EditorUtilities.GetSelectedClip(_listOfClips, pauseMenu.MenuOpenName);
        _soundsID[1] = EditorUtilities.GetSelectedClip(_listOfClips, pauseMenu.MenuCloseName);

        _soundsID[0] = EditorGUILayout.Popup("OnOpen Audio Clip", _soundsID[0], _listOfClips);
        _soundsID[1] = EditorGUILayout.Popup("OnClose Audio Clip", _soundsID[1], _listOfClips);

        pauseMenu.MenuOpenName = SoundManager.Instance.storedSFXs[_soundsID[0]].name;
        pauseMenu.MenuCloseName = SoundManager.Instance.storedSFXs[_soundsID[1]].name;

        EditorUtility.SetDirty(target);
    }
}
