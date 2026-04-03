using Env.Runtime;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
namespace Env.Runtime
{
  

    public class ProcInstances
    {
        public List<Matrix4x4> Transforms;
        public InstanceableObject Mesh;
        public ProcInstances(ProcInstances copy) : this(new InstanceableObject(copy.Mesh), copy.Transforms)
        {

        }

        public ProcInstances(Mesh StaticMesh, Material[] Materials):this(StaticMesh,Materials, new List<Matrix4x4>())
        {
            
        }
        public ProcInstances(Mesh StaticMesh, Material[] Materials, List<Matrix4x4> Transforms):this(new InstanceableObject(StaticMesh, Materials), Transforms)
        {
            
        }
        public ProcInstances(InstanceableObject Mesh):this(Mesh, new List<Matrix4x4>())
        {
        }
        public ProcInstances(InstanceableObject Mesh, List<Matrix4x4> Transforms)
        {
            this.Transforms = Transforms;
            this.Mesh = Mesh;
        }
        internal static ProcInstances From(MeshRenderer o)
        {
            if (o == null)
            {
                return null;
            }
            else
            {
                return new ProcInstances(new InstanceableObject(o));
            }
        }

        public void Add(float3 position, Quaternion rotation, Vector3 scale)
        {
            Transforms.Add(Matrix4x4.TRS(position, rotation, scale));
        }

        public void SetMaterials(List<Material> mats)
        {
            Mesh.SetMaterials(mats);
        }
    }
}