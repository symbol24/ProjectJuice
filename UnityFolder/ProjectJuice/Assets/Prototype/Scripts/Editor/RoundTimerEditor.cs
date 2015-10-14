using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(RoundStartTimer))]
public class RoundTimerEditor : Editor {
    int _choice = 0;
    string[] _listOfClips;

    int _goChoice = 0;

    int _GetReadyGo = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var roundTimer = target as RoundStartTimer;

        _listOfClips = EditorUtilities.GetListOfAudioClips();
        
        _choice = EditorUtilities.GetSelectedClip(_listOfClips, roundTimer.NumberClipName);
        _goChoice = EditorUtilities.GetSelectedClip(_listOfClips, roundTimer.GoClipName);
        _GetReadyGo = EditorUtilities.GetSelectedClip(_listOfClips, roundTimer.GetReadyClipName);

        _choice = EditorGUILayout.Popup("Audio Clip on Number", _choice, _listOfClips);
        _goChoice = EditorGUILayout.Popup("Audio Clip on Go", _goChoice, _listOfClips);
        _GetReadyGo = EditorGUILayout.Popup("Audio Clip on Get Ready", _GetReadyGo, _listOfClips);

        roundTimer.NumberClipName = SoundManager.Instance.storedSFXs[_choice].name;
        roundTimer.GoClipName = SoundManager.Instance.storedSFXs[_goChoice].name;
        roundTimer.GetReadyClipName = SoundManager.Instance.storedSFXs[_GetReadyGo].name;

        EditorUtility.SetDirty(target);
    }
}
