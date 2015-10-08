using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Database))]
public class DatabaseEditor : Editor
{
    string[] _listOfClips;
    string[] ListOfScenes;
    int _sceneChoice = 0;

    int[] _soundsID = new int[4];

    public override void OnInspectorGUI()
    {
        _listOfClips = EditorUtilities.GetListOfAudioClips();
        DrawDefaultInspector();

        var database = target as Database;

        ListOfScenes = EditorUtilities.GetSceneNames();

        _sceneChoice = database.MainMenuID;
        _soundsID[0] = EditorUtilities.GetSelectedClip(_listOfClips, database.MenuSlideName);
        _soundsID[1] = EditorUtilities.GetSelectedClip(_listOfClips, database.MenuClickName);
        _soundsID[2] = EditorUtilities.GetSelectedClip(_listOfClips, database.MenuCancelName);
        _soundsID[3] = EditorUtilities.GetSelectedClip(_listOfClips, database.MenuErrorName);

        _sceneChoice = EditorGUILayout.Popup("Main Menu scene", _sceneChoice, ListOfScenes);
        _soundsID[0] = EditorGUILayout.Popup("Menu Slide Audio Clip", _soundsID[0], _listOfClips);
        _soundsID[1] = EditorGUILayout.Popup("Menu Click Audio Clip", _soundsID[1], _listOfClips);
        _soundsID[2] = EditorGUILayout.Popup("Menu Cancel Audio Clip", _soundsID[2], _listOfClips);
        _soundsID[3] = EditorGUILayout.Popup("Menu Error Audio Clip", _soundsID[3], _listOfClips);


        database.MainMenuID = _sceneChoice;
        database.MenuSlideName = SoundManager.Instance.storedSFXs[_soundsID[0]].name;
        database.MenuClickName = SoundManager.Instance.storedSFXs[_soundsID[1]].name;
        database.MenuCancelName = SoundManager.Instance.storedSFXs[_soundsID[2]].name;
        database.MenuErrorName = SoundManager.Instance.storedSFXs[_soundsID[3]].name;

        EditorUtility.SetDirty(target);
    }

}
