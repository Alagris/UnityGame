using TMPro;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(FloatingIslandGenerator))]
public class ProcTerrainGeneratorEditor : Editor
{
    public VisualTreeAsset InspectorUXML;
    public override VisualElement CreateInspectorGUI()
    {
        // Create a new VisualElement to be the root of our Inspector UI.
        VisualElement myInspector = new VisualElement();

        // Add a simple label.
        myInspector.Add(new Label("This is a custom Inspector"));
        if(InspectorUXML == null)
        {
            InspectorUXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Environment/ProcTerrainGenerator.uxml");


        }
        
        // Load the UXML file and clone its tree into the inspector.
        if (InspectorUXML != null)
        {
            VisualElement uxmlContent = InspectorUXML.CloneTree();
            Button refreshBtn = uxmlContent.Q<Button>("refresh-button");
            refreshBtn.clickable.clicked += () =>
            {
                ((FloatingIslandGenerator)target).Refresh();
            };
            myInspector.Add(uxmlContent);
        }

        // Return the finished Inspector UI.
        return myInspector;
    }

    
    public override void OnInspectorGUI() {
        //Called whenever the inspector is drawn for this object.
        //    DrawDefaultInspector();
        //This draws the default screen.  You don't need this if you want
        //to start from scratch, but I use this when I'm just adding a button or
        //some small addition and don't feel like recreating the whole inspector.

        //  if (GUILayout.Button("Refresh")) { 
        //     ().Refresh();
        //}
        /*
        if (GUI.changed)
        {
            FloatingIslandGenerator t=(FloatingIslandGenerator)target;
            EditorUtility.SetDirty(t);
            EditorSceneManager.MarkSceneDirty(t.gameObject.scene);
        }
        */
    }
    
}