using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(SplashScreen))]
public class SplashScreenEditor : Editor {
    string[] ListOfScenes;
    int _sceneChoice = 0;
    int _crowdChoice = 0;
    int _pressChoice = 0;
    string[] ListOfAudioClips;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var splashScreen = target as SplashScreen;

        ListOfScenes = EditorUtilities.GetSceneNames();
        ListOfAudioClips = EditorUtilities.GetListOfAudioClips();

        _sceneChoice = splashScreen.m_NextLevel;
        _crowdChoice = EditorUtilities.GetSelectedClip(ListOfAudioClips, splashScreen.CrowdClip);
        _pressChoice = EditorUtilities.GetSelectedClip(ListOfAudioClips, splashScreen.PressClip);

        _sceneChoice = EditorGUILayout.Popup("Next Scene", _sceneChoice, ListOfScenes);
        _crowdChoice = EditorGUILayout.Popup("Crowd Audio", _crowdChoice, ListOfAudioClips);
        _pressChoice = EditorGUILayout.Popup("Press Start Audio", _pressChoice, ListOfAudioClips);

        splashScreen.m_NextLevel = _sceneChoice;
        splashScreen.CrowdClip = SoundManager.Instance.storedSFXs[_crowdChoice].name;
        splashScreen.PressClip = SoundManager.Instance.storedSFXs[_pressChoice].name;

        EditorUtility.SetDirty(target);
    }
}
