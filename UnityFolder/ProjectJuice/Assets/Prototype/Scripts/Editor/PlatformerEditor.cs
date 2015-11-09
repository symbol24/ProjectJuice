using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets._2D;

[CustomEditor(typeof(PlatformerCharacter2D))]
public class PlatformerEditor : Editor {
    string[] _listOFParticles;

    int[] _choice = new int[6];

    public override void OnInspectorGUI()
    {
        _listOFParticles = EditorUtilities.GetListOfParticles();
        DrawDefaultInspector();

        var player = target as PlatformerCharacter2D;

        _choice[0] = EditorUtilities.GetSelectedParticle(player.m_JumpParticle);
        _choice[1] = EditorUtilities.GetSelectedParticle(player.m_LandingParticle);
        _choice[2] = EditorUtilities.GetSelectedParticle(player.m_DashBodyThrusters);
        _choice[3] = EditorUtilities.GetSelectedParticle(player.m_GroundDashGrinding);
        _choice[4] = EditorUtilities.GetSelectedParticle(player.m_DashChromaticAberation);
        _choice[5] = EditorUtilities.GetSelectedParticle(player.m_DashParticle);

        _choice[0] = EditorGUILayout.Popup("Jump Particle", _choice[0], _listOFParticles);
        _choice[1] = EditorGUILayout.Popup("Landing Particle", _choice[1], _listOFParticles);
        _choice[2] = EditorGUILayout.Popup("Dash Body Thrusters Particle", _choice[2], _listOFParticles);
        _choice[3] = EditorGUILayout.Popup("Ground Dash Grinding Particle", _choice[3], _listOFParticles);
        _choice[4] = EditorGUILayout.Popup("Chromatic Aberation Particle", _choice[4], _listOFParticles);
        _choice[5] = EditorGUILayout.Popup("Dash Trail Particle", _choice[5], _listOFParticles);

        player.m_JumpParticle = Database.instance.Particles[_choice[0]];
        player.m_LandingParticle = Database.instance.Particles[_choice[1]];
        player.m_DashBodyThrusters = Database.instance.Particles[_choice[2]];
        player.m_GroundDashGrinding = Database.instance.Particles[_choice[3]];
        player.m_DashChromaticAberation = Database.instance.Particles[_choice[4]];
        player.m_DashParticle = Database.instance.Particles[_choice[5]];

        EditorUtility.SetDirty(target);
    }
}
