using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(SplashScreen))]
public class SplashScreenEditor : Editor {
    string[] ListOfScenes;
    int _sceneChoice = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var splashScreen = target as SplashScreen;

        ListOfScenes = EditorUtilities.GetSceneNames();

        _sceneChoice = splashScreen.m_NextLevel;

        _sceneChoice = EditorGUILayout.Popup("Next Scene", _sceneChoice, ListOfScenes);

        splashScreen.m_NextLevel = _sceneChoice;

        EditorUtility.SetDirty(target);
    }
}
