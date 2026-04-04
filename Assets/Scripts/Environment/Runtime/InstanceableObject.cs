using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.GraphToolkit.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;


namespace Env.Runtime
{
    [Serializable]
    public struct InstanceableLOD
    {
        [SerializeField]
        public Mesh StaticMesh;

        [SerializeField]
        public Material[] Materials;

        [SerializeField]
        public float chunkDistance;
        
        public InstanceableLOD(MeshRenderer o)
        {
            MeshFilter f = o.gameObject.GetComponent<MeshFilter>();
            StaticMesh = f.sharedMesh;
            Materials = o.sharedMaterials;
            chunkDistance = 100000;
        }
        public InstanceableLOD(Mesh StaticMesh, Material[] Materials)
        {
            this.StaticMesh = StaticMesh;
            this.Materials = Materials;
            chunkDistance = 100000;
        }

        internal void SetMaterials(List<Material> mats)
        {
            int s = Mathf.Min(StaticMesh.subMeshCount, mats.Count);
            Materials = new Material[s];
            for(int i = 0; i < s; i++)
            {
                Materials[i] = mats[i];
            }
        }
    }
    [Serializable]
    public class InstanceableObject : List<InstanceableLOD>
    {
        public InstanceableObject() { }
        public InstanceableObject(Mesh mesh) : this(mesh, null) { }
        public InstanceableObject(InstanceableObjectAsset asset) : base(asset.LODs){} 
        public InstanceableObject(IEnumerable<InstanceableLOD> lods) : base(lods) { } 
        public InstanceableObject(Mesh StaticMesh, Material[] Materials) : this(new InstanceableLOD(StaticMesh, Materials)) { }
        public InstanceableObject(InstanceableLOD lod)
        {
            Add(lod);
        }
        public InstanceableObject(MeshRenderer o)
        {
            if (o != null) { 
                Add(new InstanceableLOD(o));
            }
        }
        public InstanceableObject(LODGroup o)
        {
            
            if (o != null)
            {
                LOD[] lods = o.GetLODs();
                foreach (LOD lod in lods)
                {
                    if (lod.renderers.Length > 0 && lod.renderers[0] is MeshRenderer)
                    {
                        MeshRenderer p = (MeshRenderer)lod.renderers[0];
                        InstanceableLOD ilod = new InstanceableLOD(p);
                        ilod.chunkDistance = lod.screenRelativeTransitionHeight;
                        Add(ilod);
                    }
                    
                }
            }
            
        }

        internal void SetMaterials(List<Material> mats)
        {
            if (mats != null && mats.Count>0)
            {
                for (int j = 0; j < Count; j++)
                {
                    this[j].SetMaterials(mats);
                }
            }
        }
    }

    
    

    [CreateAssetMenu(fileName = "InstancedMesh", menuName = "Proc Env/Instanced Mesh")]
    public class InstanceableObjectAsset : ScriptableObject
    {
        [SerializeField]
        public List<InstanceableLOD> LODs;

        [MenuItem("Assets/Create/Proc Env/Instanced Mesh From Selected Asset")]
        static void CreateAssetFile()
        {
            InstanceableObjectAsset asset = ScriptableObject.CreateInstance<InstanceableObjectAsset>();

            
            if (Selection.activeObject is Mesh)
            {
                Mesh m = (Mesh)Selection.activeObject;
                asset.LODs = new InstanceableObject(m);
            }
            else if (Selection.activeObject is GameObject)
            {
                GameObject go = (GameObject)Selection.activeObject;
                MeshRenderer mesh = go.GetComponent<MeshRenderer>();
                if (mesh != null)
                {
                    asset.LODs = new InstanceableObject(mesh);
                }
                else
                {
                    LODGroup lods = go.GetComponent<LODGroup>();
                    if (lods != null)
                    {
                        asset.LODs = new InstanceableObject(lods);
                    }
                }

            }

            String path = AssetDatabase.GetAssetPath(Selection.activeObject);
            String assetPath = Path.GetFileNameWithoutExtension(path) + " instanced.asset";

            ProjectWindowUtil.CreateAsset(asset, assetPath);
        }
    }
}