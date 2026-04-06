using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Env.Runtime
{

    [Serializable]
    public class EnvCompiledGraph : ScriptableObject
    {
       
        [SerializeReference]
        public EnvCompiledFunction[] functions;
        public List<int> intArraysCount = new List<int>();
        public List<int> floatArraysCount = new List<int>();
        public List<int> int2ArraysCount = new List<int>();
        public List<int> float2ArraysCount = new List<int>();
        public List<int> int3ArraysCount = new List<int>();
        public List<int> float3ArraysCount = new List<int>();
        public List<int> procMeshesCount = new List<int>();
        public List<int> procInstanceSetsCount = new List<int>();
        public List<int> objectCount = new List<int>();
        public List<int> colorCount = new List<int>();
        public int returnedLandscape;
        public int returnedInstances;





        Blackboard makeNewBB(int resX, int resZ, float size, float3 offset)
        {
            return new Blackboard(resX, resZ, size, offset,
                intArraysCount: intArraysCount,
                floatArraysCount: floatArraysCount,
                int2ArraysCount: int2ArraysCount,
                float2ArraysCount: float2ArraysCount,
                int3ArraysCount: int3ArraysCount,
                float3ArraysCount: float3ArraysCount,
                procMeshesCount: procMeshesCount,
                procInstanceSetsCount: procInstanceSetsCount,
                objectCount: objectCount,
                colorCount: colorCount
            );
        }
        public Blackboard Run(int resX, int resZ, float size, float3 offset)
        {
            Blackboard bb = makeNewBB(resX, resZ, size, offset);    
            Run(bb);
            bb.returnedInstances = returnedInstances;
            bb.returnedLandscape = returnedLandscape;
            return bb;
        }
        void Run(Blackboard bb)
        {
            foreach (EnvCompiledFunction f in functions)
            {
                f.run(bb);
            }
        }

        internal Section SpawnSection(int id)
        {
            throw new NotImplementedException();
        }
    }
}