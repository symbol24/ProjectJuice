using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(SelectorMenu))]
public class SelectorMenuEditor : Editor {

    int[] _choices = new int[10];
    int _vertChoice = 0;

    string[] _strings;
    string[] _listOfClips;


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var selectorMenu = target as SelectorMenu;

        _strings = EditorUtilities.ConvertToArray(Database.instance.GameTexts);
        _listOfClips = EditorUtilities.GetListOfAudioClips();

        _choices[0] = selectorMenu.m_headerPreSelection;
        _choices[1] = selectorMenu.m_headerSelection;
        _choices[2] = selectorMenu.m_headerConfirmation;
        _choices[3] = selectorMenu.m_headerConfirmed;
        _choices[4] = selectorMenu.m_ledgerSelection;
        _choices[5] = selectorMenu.m_ledgerConfirmation;
        _choices[6] = selectorMenu.m_ledgerConfirmedWaiting;
        _choices[7] = selectorMenu.m_ledgerConfirmedCanStart;
        _choices[8] = selectorMenu.m_ConfirmTextConfirmation;
        _choices[9] = selectorMenu.m_ConfirmTextConfirmed;
        _vertChoice = EditorUtilities.GetSelectedClip(_listOfClips, selectorMenu.m_VerticalSlide);

        _choices[0] = EditorGUILayout.Popup("Header PreSelection", _choices[0], _strings);
        _choices[1] = EditorGUILayout.Popup("Header Selection", _choices[1], _strings);
        _choices[2] = EditorGUILayout.Popup("Header Confirmation", _choices[2], _strings);
        _choices[3] = EditorGUILayout.Popup("Header Confirmed", _choices[3], _strings);
        _choices[4] = EditorGUILayout.Popup("Ledger Selection", _choices[4], _strings);
        _choices[5] = EditorGUILayout.Popup("Ledger Confirmation", _choices[5], _strings);
        _choices[6] = EditorGUILayout.Popup("Ledger Confirm Waiting", _choices[6], _strings);
        _choices[7] = EditorGUILayout.Popup("Ledger Confirm CanStart", _choices[7], _strings);
        _choices[8] = EditorGUILayout.Popup("Popup Confirmation", _choices[8], _strings);
        _choices[9] = EditorGUILayout.Popup("Popup Confirmed", _choices[9], _strings);
        _vertChoice = EditorGUILayout.Popup("Menu Vertical Slide Sound", _vertChoice, _listOfClips);

        selectorMenu.m_headerPreSelection = _choices[0];
        selectorMenu.m_headerSelection = _choices[1];
        selectorMenu.m_headerConfirmation = _choices[2];
        selectorMenu.m_headerConfirmed = _choices[3];
        selectorMenu.m_ledgerSelection = _choices[4];
        selectorMenu.m_ledgerConfirmation = _choices[5];
        selectorMenu.m_ledgerConfirmedWaiting = _choices[6];
        selectorMenu.m_ledgerConfirmedCanStart = _choices[7];
        selectorMenu.m_ConfirmTextConfirmation = _choices[8];
        selectorMenu.m_ConfirmTextConfirmed = _choices[9];
        selectorMenu.m_VerticalSlide = SoundManager.Instance.storedSFXs[_vertChoice].name;

        EditorUtility.SetDirty(target);
    }
}
