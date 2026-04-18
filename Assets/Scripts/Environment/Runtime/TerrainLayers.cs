using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Env.Runtime
{
    
    public class TerrainLayers : ScriptableObject
    {
        [SerializeField]
        public Texture2DArray diffuse;
        [SerializeField]
        public Texture2DArray normal;
        [SerializeField]
        public Texture2DArray mask;
        [SerializeField]
        public string[] names;
        [SerializeField]
        public Material material;

    }
}
