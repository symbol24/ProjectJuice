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

    int[] _soundsID = new int[10];

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
        _soundsID[4] = EditorUtilities.GetSelectedClip(_listOfClips, database.RobotDeath);
        _soundsID[5] = EditorUtilities.GetSelectedClip(_listOfClips, database.RobotDeathCrowd);
        _soundsID[6] = EditorUtilities.GetSelectedClip(_listOfClips, database.Jump);
        _soundsID[7] = EditorUtilities.GetSelectedClip(_listOfClips, database.Landing);
        _soundsID[8] = EditorUtilities.GetSelectedClip(_listOfClips, database.Dash);
        _soundsID[9] = EditorUtilities.GetSelectedClip(_listOfClips, database.DashMetalGrind);

        _sceneChoice = EditorGUILayout.Popup("Main Menu scene", _sceneChoice, ListOfScenes);
        _soundsID[0] = EditorGUILayout.Popup("Menu Slide SFX", _soundsID[0], _listOfClips);
        _soundsID[1] = EditorGUILayout.Popup("Menu Click SFX", _soundsID[1], _listOfClips);
        _soundsID[2] = EditorGUILayout.Popup("Menu Cancel SFX", _soundsID[2], _listOfClips);
        _soundsID[3] = EditorGUILayout.Popup("Menu Error SFX", _soundsID[3], _listOfClips);
        _soundsID[4] = EditorGUILayout.Popup("Robot Death SFX", _soundsID[4], _listOfClips);
        _soundsID[5] = EditorGUILayout.Popup("Robot Death Crowd SFX", _soundsID[5], _listOfClips);
        _soundsID[6] = EditorGUILayout.Popup("Jump SFX", _soundsID[6], _listOfClips);
        _soundsID[7] = EditorGUILayout.Popup("Land SFX", _soundsID[7], _listOfClips);
        _soundsID[8] = EditorGUILayout.Popup("Dash SFX", _soundsID[8], _listOfClips);
        _soundsID[9] = EditorGUILayout.Popup("Ground Dash Metal SFX", _soundsID[9], _listOfClips);


        database.MainMenuID = _sceneChoice;
        database.MenuSlideName = _listOfClips[_soundsID[0]];
        database.MenuClickName = _listOfClips[_soundsID[1]];
        database.MenuCancelName = _listOfClips[_soundsID[2]];
        database.MenuErrorName = _listOfClips[_soundsID[3]];
        database.RobotDeath = _listOfClips[_soundsID[4]];
        database.RobotDeathCrowd = _listOfClips[_soundsID[5]];
        database.Jump = _listOfClips[_soundsID[6]];
        database.Landing = _listOfClips[_soundsID[7]];
        database.Dash = _listOfClips[_soundsID[8]];
        database.DashMetalGrind = _listOfClips[_soundsID[9]];

        EditorUtility.SetDirty(target);
    }

}
