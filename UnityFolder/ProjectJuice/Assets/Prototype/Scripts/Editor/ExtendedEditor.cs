using UnityEngine;
using System.Collections;
using UnityEditor;

public abstract class ExtendedEditor : Editor {

    protected void AddTitle(string message)
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(string.Format("*** {0} ***", message), new GUIStyle { fontSize = 15, fontStyle = FontStyle.Bold });
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
}
