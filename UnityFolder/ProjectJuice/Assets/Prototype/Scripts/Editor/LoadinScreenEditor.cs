using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(LoadingScreen))]
public class LoadinScreenEditor : Editor {
    string[] ListOfScenes;
    int _sceneChoice = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var loading = target as LoadingScreen;

        ListOfScenes = EditorUtilities.GetSceneNames();

        _sceneChoice = loading.m_LevelToLoad;

        _sceneChoice = EditorGUILayout.Popup("Current scene", _sceneChoice, ListOfScenes);

        loading.m_LevelToLoad = _sceneChoice;

        EditorUtility.SetDirty(target);
    }

}
