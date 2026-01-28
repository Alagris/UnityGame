using TMPro;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StaticGrid))] 
public class StaticGridInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Refresh"))
        {
            ((StaticGrid)target).Refresh();
        }
    }
}
