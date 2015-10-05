using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(PlayerSelect))]
public class PlayerSelectEditor : Editor
{

    string[] ListOfScenes;
    int _sceneChoice = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var playerSelect = target as PlayerSelect;

        ListOfScenes = EditorUtilities.GetSceneNames();

        _sceneChoice = playerSelect.NextScene;

        _sceneChoice = EditorGUILayout.Popup("Next Scene", _sceneChoice, ListOfScenes);

        playerSelect.NextScene = _sceneChoice;

        EditorUtility.SetDirty(target);
    }
}
