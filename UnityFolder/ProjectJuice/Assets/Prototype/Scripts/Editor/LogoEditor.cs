using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(LogoIntro))]
public class LogoEditor : Editor {
    string[] ListOfScenes;
    int _sceneChoice = 0;

    string[] ListOfAudioClips;
    int _clipChoice = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var logoIntro = target as LogoIntro;
        
        ListOfScenes = EditorUtilities.GetSceneNames();
        ListOfAudioClips = EditorUtilities.GetListOfAudioClips();

        _sceneChoice = logoIntro.NextScene;
        _clipChoice =  EditorUtilities.GetSelectedClip(ListOfAudioClips, logoIntro.ClipName);

        _sceneChoice = EditorGUILayout.Popup("Next Scene", _sceneChoice, ListOfScenes);
        _clipChoice = EditorGUILayout.Popup("Audio Clip on Logo", _clipChoice, ListOfAudioClips);

        logoIntro.NextScene = _sceneChoice;
        logoIntro.ClipName = SoundManager.Instance.storedSFXs[_clipChoice].name;

        EditorUtility.SetDirty(target);
    }
}
