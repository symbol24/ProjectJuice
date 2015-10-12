using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(shield))]
public class ShieldEditor : Editor {
    string[] _listOfClips;

    int[] _choice = new int[7];

    public override void OnInspectorGUI()
    {
        _listOfClips = EditorUtilities.GetListOfAudioClips();
        DrawDefaultInspector();

        var leShield = target as shield;

        _choice[0] = EditorUtilities.GetSelectedClip(_listOfClips, leShield.Activate);
        _choice[1] = EditorUtilities.GetSelectedClip(_listOfClips, leShield.AbsorbBullet);
        _choice[2] = EditorUtilities.GetSelectedClip(_listOfClips, leShield.ShootingBullets);
        _choice[3] = EditorUtilities.GetSelectedClip(_listOfClips, leShield.FullCharge);
        _choice[4] = EditorUtilities.GetSelectedClip(_listOfClips, leShield.Ricochet);
        _choice[5] = EditorUtilities.GetSelectedClip(_listOfClips, leShield.CoolDown);
        _choice[6] = EditorUtilities.GetSelectedClip(_listOfClips, leShield.AbrosbExplosion);

        _choice[0] = EditorGUILayout.Popup("Activate Shield SFX", _choice[0], _listOfClips);
        _choice[1] = EditorGUILayout.Popup("Absorb Bullet SFX", _choice[1], _listOfClips);
        _choice[2] = EditorGUILayout.Popup("Shoot Bullets SFX", _choice[2], _listOfClips);
        _choice[3] = EditorGUILayout.Popup("Full Charge SFX", _choice[3], _listOfClips);
        _choice[4] = EditorGUILayout.Popup("Ricochet? SFX", _choice[4], _listOfClips);
        _choice[5] = EditorGUILayout.Popup("Cooldown SFX", _choice[5], _listOfClips);
        _choice[6] = EditorGUILayout.Popup("Abesorb Explosion SFX", _choice[6], _listOfClips);

        leShield.Activate = _listOfClips[_choice[0]];
        leShield.AbsorbBullet = _listOfClips[_choice[1]];
        leShield.ShootingBullets = _listOfClips[_choice[2]];
        leShield.FullCharge = _listOfClips[_choice[3]];
        leShield.Ricochet = _listOfClips[_choice[4]];
        leShield.CoolDown = _listOfClips[_choice[5]];
        leShield.AbrosbExplosion = _listOfClips[_choice[6]];

        EditorUtility.SetDirty(target);
    }

}
