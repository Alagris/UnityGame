using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MovingGrid))] 
public class MovingGridInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Refresh"))
        {
            ((MovingGrid)target).Refresh();
        }
    }
}
