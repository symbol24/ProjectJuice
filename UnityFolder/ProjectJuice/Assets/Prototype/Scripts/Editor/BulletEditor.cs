using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Bullet))]
public class BulletEditor : Editor {

    string[] _listOfClips;

    int[] _choice = new int[3];

    string[] _listOFParticles;

    int[] _particle = new int[1];

    public override void OnInspectorGUI()
    {
        _listOfClips = EditorUtilities.GetListOfAudioClips();
        _listOFParticles = EditorUtilities.GetListOfParticles();
        DrawDefaultInspector();

        var bullet = target as Bullet;

        _choice[0] = EditorUtilities.GetSelectedClip(_listOfClips, bullet.GroundImpact);
        _choice[1] = EditorUtilities.GetSelectedClip(_listOfClips, bullet.RobotBulletImpact);
        _choice[2] = EditorUtilities.GetSelectedClip(_listOfClips, bullet.WeakpointBulletImpact);

        _particle[0] = EditorUtilities.GetSelectedParticle(bullet.m_particleImpact);

        _choice[0] = EditorGUILayout.Popup("Ground Impact SFX", _choice[0], _listOfClips);
        _choice[1] = EditorGUILayout.Popup("Robot Impact SFX", _choice[1], _listOfClips);
        _choice[2] = EditorGUILayout.Popup("Weak Point Impact SFX", _choice[2], _listOfClips);

        _particle[0] = EditorGUILayout.Popup("Impact Particle", _particle[0], _listOFParticles);

        bullet.GroundImpact = _listOfClips[_choice[0]];
        bullet.RobotBulletImpact = _listOfClips[_choice[1]];
        bullet.WeakpointBulletImpact = _listOfClips[_choice[2]];

        bullet.m_particleImpact = Database.instance.Particles[_particle[0]];

        EditorUtility.SetDirty(target);
    }
}
