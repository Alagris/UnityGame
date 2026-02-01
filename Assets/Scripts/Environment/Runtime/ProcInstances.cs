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
        public InstanceableObject Object;

        public void Add(float3 position, Quaternion rotation)
        {
            Transforms.Add(Matrix4x4.TRS(position, rotation, Vector3.one));
        }
    }
}