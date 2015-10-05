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

    int[] _soundsID = new int[3];

    public override void OnInspectorGUI()
    {
        _listOfClips = EditorUtilities.GetListOfAudioClipsFromGroup("Logo");
        DrawDefaultInspector();

        var database = target as Database;

        ListOfScenes = EditorUtilities.GetSceneNames();

        _sceneChoice = database.MainMenuID;
        _soundsID[0] = database.MenuSlideID;
        _soundsID[1] = database.MenuClickID;
        _soundsID[2] = database.MenuCanceID;

        _sceneChoice = EditorGUILayout.Popup("Main Menu scene", _sceneChoice, ListOfScenes);
        _soundsID[0] = EditorGUILayout.Popup("Menu Slide Audio Clip", _soundsID[0], _listOfClips);
        _soundsID[1] = EditorGUILayout.Popup("Menu Click Audio Clip", _soundsID[1], _listOfClips);
        _soundsID[2] = EditorGUILayout.Popup("Menu Cancel Audio Clip", _soundsID[2], _listOfClips);


        database.MainMenuID = _sceneChoice;

        database.MenuSlideID = _soundsID[0];
        database.MenuClickID = _soundsID[1];
        database.MenuCanceID = _soundsID[2];
        database.MenuSlideName = SoundManager.Instance.storedSFXs[_soundsID[0]].name;
        database.MenuClickName = SoundManager.Instance.storedSFXs[_soundsID[1]].name;
        database.MenuCancelName = SoundManager.Instance.storedSFXs[_soundsID[2]].name;

        EditorUtility.SetDirty(target);
    }

}
