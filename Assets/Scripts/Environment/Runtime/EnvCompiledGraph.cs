using System;
using Unity.Mathematics;
using UnityEngine;

namespace Env.Runtime
{

    [Serializable]
    public class EnvCompiledGraph : ScriptableObject
    {
        [SerializeReference]
        public EnvCompiledFunction[] functions;
        public int intArraysCount;
        public int floatArraysCount;
        public int int2ArraysCount;
        public int float2ArraysCount;
        public int int3ArraysCount;
        public int float3ArraysCount;
        public int procMeshesCount;
        public int procInstanceSetsCount;
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
                procInstanceSetsCount: procInstanceSetsCount
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