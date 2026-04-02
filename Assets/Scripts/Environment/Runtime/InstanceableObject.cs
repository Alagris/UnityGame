using System;
using UnityEngine;


namespace Env.Runtime
{
    [Serializable]
    public class InstanceableObject
    {
        [SerializeField]
        Mesh StaticMesh;
        [SerializeField]
        Material Material;

        
        public InstanceableObject(Mesh StaticMesh, Material Material) {
            this.StaticMesh = StaticMesh;
            this.Material = Material;
        }

        internal static InstanceableObject From(MeshRenderer o)
        {
            if (o == null)
            {
                return null;
            }
            else
            {
                MeshFilter f = o.gameObject.GetComponent<MeshFilter>();
                return new InstanceableObject(f.sharedMesh, o.sharedMaterial);
            }
        }
    }
}