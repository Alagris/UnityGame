using Env.Runtime;
using log4net.Util;
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


        public static void AssignTransform(UnityEngine.Transform destination, Matrix4x4 source)
        {

            destination.localScale = ExtractScale(source);
            destination.rotation = ExtractRotation(source);
            destination.position = ExtractPosition(source);
        }
        public static Quaternion ExtractRotation( Matrix4x4 m)
        {
            Quaternion q = new Quaternion();
            q.w = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2;
            q.x = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2;
            q.y = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) / 2;
            q.z = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) / 2;
            q.x *= Mathf.Sign(q.x * (m[2, 1] - m[1, 2]));
            q.y *= Mathf.Sign(q.y * (m[0, 2] - m[2, 0]));
            q.z *= Mathf.Sign(q.z * (m[1, 0] - m[0, 1]));
            return q;
        }

        public static Vector3 ExtractPosition( Matrix4x4 matrix)
        {
            Vector3 position;
            position.x = matrix.m03;
            position.y = matrix.m13;
            position.z = matrix.m23;
            return position;
        }

        public static Vector3 ExtractScale( Matrix4x4 matrix)
        {
            Vector3 scale;
            scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
            scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
            scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
            return scale;
        }
    }
}