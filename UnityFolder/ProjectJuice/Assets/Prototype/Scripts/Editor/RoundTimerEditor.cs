using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(RoundStartTimer))]
public class RoundTimerEditor : Editor {
    int _choice = 0;
    string[] _listOfClips;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var roundTimer = target as RoundStartTimer;

        _listOfClips = EditorUtilities.GetListOfAudioClipsFromGroup("Logo");
        
        _choice = roundTimer.clipChoice;

        _choice = EditorGUILayout.Popup("Audio Clip on Logo", _choice, _listOfClips);
        
        roundTimer.clipChoice = _choice;
        roundTimer.clipName = SoundManager.Instance.storedSFXs[_choice].name;

        EditorUtility.SetDirty(target);
    }
}
