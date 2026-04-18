using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR

namespace Env.Runtime
{
    [CustomEditor(typeof(StaticGrid))]
    public class StaticGridInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Refresh"))
            {
                ((StaticGrid)target).Clear();
                ((StaticGrid)target).Refresh();
            }
            if (GUILayout.Button("Clear"))
            {
                ((StaticGrid)target).Clear();
            }
        }
    }
}
#endif