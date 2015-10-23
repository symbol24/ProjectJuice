using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(MovementSparking))]
public class MovementSparkingEditor : Editor
{
    string[] _listOFParticles;

    int[] _particle = new int[1];


    public override void OnInspectorGUI()
    {
        _listOFParticles = EditorUtilities.GetListOfParticles();
        DrawDefaultInspector();

        var msEditor = target as MovementSparking;

        _particle[0] = EditorUtilities.GetSelectedParticle(msEditor._effectsToSpawn);

        _particle[0] = EditorGUILayout.Popup("Sparking Particle", _particle[0], _listOFParticles);

        msEditor._effectsToSpawn = Database.instance.Particles[_particle[0]];

        EditorUtility.SetDirty(target);
    }
}
