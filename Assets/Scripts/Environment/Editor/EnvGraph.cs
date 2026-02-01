using System;
using UnityEngine;
using Unity.GraphToolkit.Editor;
using UnityEditor;

namespace Env.Editor
{
    [Serializable]
    [Graph(AssetExtension)]
    public class EnvGraph : Graph
    {
        internal const string AssetExtension = "enveditor";

        [MenuItem("Assets/Create/Proc Env/Graph")]
        static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<EnvGraph>("Procedural environemnt graph");
        }

        public override void OnGraphChanged(GraphLogger infos)
        {
            base.OnGraphChanged(infos);
        }
    }
}
