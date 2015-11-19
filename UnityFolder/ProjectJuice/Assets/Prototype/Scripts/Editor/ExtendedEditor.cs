using UnityEngine;
using System.Collections;
using UnityEditor;

public abstract class ExtendedEditor : Editor {

    protected void AddTitle(string message, int fontSize = 15)
    {
        EditorGUILayout.Separator();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(string.Format("*** {0} ***", message), new GUIStyle { fontSize = fontSize, fontStyle = FontStyle.Bold });
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
}
