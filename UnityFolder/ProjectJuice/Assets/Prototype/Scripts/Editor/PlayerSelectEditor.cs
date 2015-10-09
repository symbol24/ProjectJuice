using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(PlayerSelect))]
public class PlayerSelectEditor : Editor
{

    string[] ListOfScenes;
    int _sceneChoice = 0;

    string[] _listOfClips;
    int _startChoice = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var playerSelect = target as PlayerSelect;

        ListOfScenes = EditorUtilities.GetSceneNames();
        _listOfClips = EditorUtilities.GetListOfAudioClips();

        _sceneChoice = playerSelect.NextScene;
        _startChoice = EditorUtilities.GetSelectedClip(_listOfClips, playerSelect.StartSound);

        _sceneChoice = EditorGUILayout.Popup("Next Scene", _sceneChoice, ListOfScenes);
        _startChoice = EditorGUILayout.Popup("Audio on Start", _startChoice, _listOfClips);

        playerSelect.NextScene = _sceneChoice;
        playerSelect.StartSound = _listOfClips[_startChoice];

        EditorUtility.SetDirty(target);
    }
}
