using UnityEditor;
using UnityEngine;

namespace Env.Runtime
{
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
}