using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(HPScript))]
public class HPScriptEditor : Editor {

    string[] _listOFParticles;

    int _choice = 0;

    public override void OnInspectorGUI()
    {
        _listOFParticles = EditorUtilities.GetListOfParticles();
        DrawDefaultInspector();

        var hpscript = target as HPScript;

        _choice = EditorUtilities.GetSelectedParticle(hpscript._deathFlashes);

        _choice = EditorGUILayout.Popup("Death Crowd Flashes", _choice, _listOFParticles);

        hpscript._deathFlashes = Database.instance.Particles[_choice];

        EditorUtility.SetDirty(target);
    }
}
