using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Bullet))]
public class BulletEditor : Editor {

    string[] _listOfClips;

    int[] _choice = new int[3];

    public override void OnInspectorGUI()
    {
        _listOfClips = EditorUtilities.GetListOfAudioClips();
        DrawDefaultInspector();

        var bullet = target as Bullet;

        _choice[0] = EditorUtilities.GetSelectedClip(_listOfClips, bullet.GroundImpact);
        _choice[1] = EditorUtilities.GetSelectedClip(_listOfClips, bullet.RobotBulletImpact);
        _choice[2] = EditorUtilities.GetSelectedClip(_listOfClips, bullet.WeakpointBulletImpact);

        _choice[0] = EditorGUILayout.Popup("Ground Impact SFX", _choice[0], _listOfClips);
        _choice[1] = EditorGUILayout.Popup("Robot Impact SFX", _choice[1], _listOfClips);
        _choice[2] = EditorGUILayout.Popup("Weak Point Impact SFX", _choice[2], _listOfClips);

        bullet.GroundImpact = _listOfClips[_choice[0]];
        bullet.RobotBulletImpact = _listOfClips[_choice[1]];
        bullet.WeakpointBulletImpact = _listOfClips[_choice[2]];

        EditorUtility.SetDirty(target);
    }
}
