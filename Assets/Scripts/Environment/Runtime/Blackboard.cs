using Unity.Mathematics;
using UnityEngine;

namespace Env.Runtime
{

    public class Blackboard
    {
        private readonly int[][] intArrays;
        private readonly float[][] floatArrays;
        private readonly int2[][] int2Arrays;
        private readonly float2[][] float2Arrays;
        private readonly int3[][] int3Arrays;
        private readonly float3[][] float3Arrays;
        private readonly ProcMesh[] procMeshes;
        private readonly ProcInstanceSet[] procInstanceSets;
        public readonly int resX, resZ;
        public readonly float size;
        public readonly float3 offset;

        public int returnedInstances, returnedLandscape;

        public Blackboard(int resX, int resZ, float size, float3 offset,
            int intArraysCount,
            int floatArraysCount,
            int int2ArraysCount,
            int float2ArraysCount,
            int int3ArraysCount,
            int float3ArraysCount,
            int procMeshesCount,
            int procInstanceSetsCount
            )
        {
            this.intArrays = new int[intArraysCount][];
            this.floatArrays = new float[floatArraysCount][];
            this.int2Arrays = new int2[int2ArraysCount][];
            this.float2Arrays = new float2[float2ArraysCount][];
            this.int3Arrays = new int3[int3ArraysCount][];
            this.float3Arrays = new float3[float3ArraysCount][];
            this.procMeshes = new ProcMesh[procMeshesCount];
            this.procInstanceSets = new ProcInstanceSet[procInstanceSetsCount];
            this.resX = resX;
            this.resZ = resZ;
            this.size = size;
            this.offset = offset;
        }
        public ProcMesh GetReturnedMesh()
        {
            return getMesh(returnedLandscape);
        }
        public ProcInstanceSet GetReturnedInstances()
        {
            return getInsatnceSet(returnedInstances);
        }
        public ProcMesh setMesh(int idx, ProcMesh m)
        {
            if (idx < 0) return null;
            Debug.Assert(procMeshes[idx] == null);
            return procMeshes[idx] = m;
        }
        public ProcInstanceSet setInsatnceSet(int idx, ProcInstances m)
        {
            return setInsatnceSet(idx, new ProcInstanceSet(m));
        }
        public ProcInstanceSet setInsatnceSet(int idx, ProcInstanceSet m)
        {
            if (idx < 0) return null;
            Debug.Assert(procInstanceSets[idx] == null);
            return procInstanceSets[idx] = m;
        }
        public float3[] setFloat3(int idx, float3[] array)
        {
            if (idx < 0) return null;
            Debug.Assert(float3Arrays[idx] == null);
            return float3Arrays[idx] = array;
        }
        public float2[] setFloat2(int idx, float2[] array)
        {
            if (idx < 0) return null;
            Debug.Assert(float2Arrays[idx] == null);
            return float2Arrays[idx] = array;
        }
        public float[] setFloat(int idx, float[] array)
        {
            if (idx < 0) return null;
            Debug.Assert(floatArrays[idx] == null);
            return floatArrays[idx] = array;
        }
        public int3[] setInt3(int idx, int3[] array)
        {
            if (idx < 0) return null;
            Debug.Assert(int3Arrays[idx] == null);
            return int3Arrays[idx] = array;
        }
        public int2[] setInt2(int idx, int2[] array)
        {
            if (idx < 0) return null;
            Debug.Assert(int2Arrays[idx] == null);
            return int2Arrays[idx] = array;
        }
        public int[] setInt(int idx, int[] array)
        {
            if (idx < 0) return null;
            Debug.Assert(intArrays[idx] == null);
            return intArrays[idx] = array;
        }

        public float3[] makeFloat3(int idx, int length)
        {
            return setFloat3(idx, new float3[length]);
        }
        public float2[] makeFloat2(int idx, int length)
        {
            return setFloat2(idx, new float2[length]);
        }
        public float[] makeFloat(int idx, int length)
        {
            return setFloat(idx, new float[length]);
        }
        public int3[] makeInt3(int idx, int length)
        {
            return setInt3(idx, new int3[length]);
        }
        public int2[] makeInt2(int idx, int length)
        {
            return setInt2(idx, new int2[length]);
        }
        public int[] makeInt(int idx, int length)
        {
            return setInt(idx, new int[length]);
        }
        public ProcMesh getMesh(int idx)
        {
            ProcMesh e = procMeshes[idx];
            Debug.Assert(e != null);
            return e;
        }
        public ProcInstanceSet getInsatnceSet(int idx)
        {
            ProcInstanceSet e = procInstanceSets[idx];
            Debug.Assert(e != null);
            return e;
        }
        public float3[] getFloat3(int idx)
        {
            float3[] e = float3Arrays[idx];
            Debug.Assert(e != null);
            return e;
        }
        public float2[] getFloat2(int idx)
        {
            float2[] e = float2Arrays[idx];
            Debug.Assert(e != null);
            return e;
        }
        public float[] getFloat(int idx)
        {
            float[] e = floatArrays[idx];
            Debug.Assert(e != null);
            return e;
        }
        public int3[] getInt3(int idx)
        {
            int3[] e = int3Arrays[idx];
            Debug.Assert(e != null);
            return e;
        }
        public int2[] getInt2(int idx)
        {
            int2[] e = int2Arrays[idx];
            Debug.Assert(e != null);
            return e;
        }
        public int[] getInt(int idx)
        {
            int[] e = intArrays[idx];
            Debug.Assert(e != null);
            return e;
        }
    }
}