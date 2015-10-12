using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(RoundMenu))]
public class RoundMenuEditor : Editor {
    string[] _listOfClips;

    int[] _soundsID = new int[7];

    public override void OnInspectorGUI()
    {
        _listOfClips = EditorUtilities.GetListOfAudioClips();
        DrawDefaultInspector();

        var roundMenu = target as RoundMenu;

        _soundsID[0] = EditorUtilities.GetSelectedClip(_listOfClips, roundMenu.MenuOpen);
        _soundsID[1] = EditorUtilities.GetSelectedClip(_listOfClips, roundMenu.MenuClose);
        _soundsID[2] = EditorUtilities.GetSelectedClip(_listOfClips, roundMenu.ReturnToCS);
        _soundsID[3] = EditorUtilities.GetSelectedClip(_listOfClips, roundMenu.MedalAppear);
        _soundsID[4] = EditorUtilities.GetSelectedClip(_listOfClips, roundMenu.MedalCrowd);
        _soundsID[5] = EditorUtilities.GetSelectedClip(_listOfClips, roundMenu.RoundPhaseOut);
        _soundsID[6] = EditorUtilities.GetSelectedClip(_listOfClips, roundMenu.MatchPhaseIn);

        _soundsID[0] = EditorGUILayout.Popup("OnOpen SFX", _soundsID[0], _listOfClips);
        _soundsID[1] = EditorGUILayout.Popup("OnClose SFX", _soundsID[1], _listOfClips);
        _soundsID[2] = EditorGUILayout.Popup("Return to CharSel SFX", _soundsID[2], _listOfClips);
        _soundsID[3] = EditorGUILayout.Popup("Medal Appear SFX", _soundsID[3], _listOfClips);
        _soundsID[4] = EditorGUILayout.Popup("Medal Crowd SFX", _soundsID[4], _listOfClips);
        _soundsID[5] = EditorGUILayout.Popup("Round Phase Out SFX", _soundsID[5], _listOfClips);
        _soundsID[6] = EditorGUILayout.Popup("Match Phase In SFX", _soundsID[6], _listOfClips);

        roundMenu.MenuOpen = _listOfClips[_soundsID[0]];
        roundMenu.MenuClose = _listOfClips[_soundsID[1]];
        roundMenu.ReturnToCS = _listOfClips[_soundsID[2]];
        roundMenu.MedalAppear = _listOfClips[_soundsID[3]];
        roundMenu.MedalCrowd = _listOfClips[_soundsID[4]];
        roundMenu.RoundPhaseOut = _listOfClips[_soundsID[5]];
        roundMenu.MatchPhaseIn = _listOfClips[_soundsID[6]];

        EditorUtility.SetDirty(target);
    }

}
