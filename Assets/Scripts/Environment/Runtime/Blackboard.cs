using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Env.Runtime
{
    struct RefCounter<T> where T: class
    {
        public T value;
        public int refs;

        public RefCounter(int refs) 
        {
            this.value = null;
            this.refs = refs;
        }

        public static RefCounter<T>[] make(List<int> usages) 
        {
            Debug.Assert(usages != null);
            RefCounter<T>[] counters = new RefCounter<T>[usages.Count];
            for (int i = 0; i < usages.Count; i++)
            {
                counters[i] = new RefCounter<T>(usages[i]);
            }
            return counters;
        }

        internal T pop()
        {
            Debug.Assert(value != null);
            T output = value;
            refs--;
            if (refs <= 0)
            {
                value = null;
            }
            return output;
        }
    }
    public class Blackboard
    {
        private readonly RefCounter<int[]>[] intArrays;
        private readonly RefCounter<float[]>[] floatArrays;
        private readonly RefCounter<int2[]>[] int2Arrays;
        private readonly RefCounter<float2[]>[] float2Arrays;
        private readonly RefCounter<int3[]>[] int3Arrays;
        private readonly RefCounter<float3[]>[] float3Arrays;
        private readonly RefCounter<ProcMesh>[] procMeshes;
        private readonly RefCounter<ProcInstanceSet>[] procInstanceSets;
        private readonly RefCounter<InstanceableObject>[] objArrays;
        private readonly RefCounter<Color[]>[] colorArrays;
        
        public readonly int resX, resZ;
        public readonly float size;
        public readonly float3 offset;

        public int returnedInstances, returnedLandscape;
        public Texture2D returnedTerrainWeights;

        public Blackboard(int resX, int resZ, float size, float3 offset,
            List<int> intArraysCount,
            List<int> floatArraysCount,
            List<int> int2ArraysCount,
            List<int> float2ArraysCount,
            List<int> int3ArraysCount,
            List<int> float3ArraysCount,
            List<int> procMeshesCount,
            List<int> procInstanceSetsCount,
            List<int> objectCount,
            List<int> colorCount
            )
        {
            this.intArrays = RefCounter<int[]>.make(intArraysCount);
            this.floatArrays = RefCounter<float[]>.make(floatArraysCount);
            this.int2Arrays = RefCounter<int2[]>.make(int2ArraysCount);
            this.float2Arrays = RefCounter<float2[]>.make(float2ArraysCount);
            this.int3Arrays = RefCounter<int3[]>.make(int3ArraysCount);
            this.float3Arrays = RefCounter<float3[]>.make(float3ArraysCount);
            this.procMeshes = RefCounter<ProcMesh>.make(procMeshesCount);
            this.procInstanceSets = RefCounter<ProcInstanceSet>.make(procInstanceSetsCount);
            this.objArrays = RefCounter<InstanceableObject>.make(objectCount);
            this.colorArrays = RefCounter<Color[]>.make(colorCount);
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
            return returnedInstances<0 ? null : getInsatnceSet(returnedInstances);
        }
        public ProcMesh setMesh(int idx, ProcMesh m)
        {
            if (idx < 0) return null;
            Debug.Assert(procMeshes[idx].value == null);
            return procMeshes[idx].value = m;
        }
        public ProcInstanceSet setInsatnceSet(int idx, ProcInstances m)
        {
            return setInsatnceSet(idx, new ProcInstanceSet(m));
        }
        public ProcInstanceSet setInsatnceSet(int idx, ProcInstanceSet m)
        {
            if (idx < 0) return null;
            Debug.Assert(procInstanceSets[idx].value == null);
            return procInstanceSets[idx].value = m;
        }
        public float3[] setFloat3(int idx, float3[] array)
        {
            if (idx < 0) return null;
            Debug.Assert(float3Arrays[idx].value == null);
            return float3Arrays[idx].value = array;
        }
        public float2[] setFloat2(int idx, float2[] array)
        {
            if (idx < 0) return null;
            Debug.Assert(float2Arrays[idx].value == null);
            return float2Arrays[idx].value = array;
        }
        public float[] setFloat(int idx, float[] array)
        {
            if (idx < 0) return null;
            Debug.Assert(floatArrays[idx].value == null);
            return floatArrays[idx].value = array;
        }
        public int3[] setInt3(int idx, int3[] array)
        {
            if (idx < 0) return null;
            Debug.Assert(int3Arrays[idx].value == null);
            return int3Arrays[idx].value = array;
        }
        public int2[] setInt2(int idx, int2[] array)
        {
            if (idx < 0) return null;
            Debug.Assert(int2Arrays[idx].value == null);
            return int2Arrays[idx].value = array;
        }
        public int[] setInt(int idx, int[] array)
        {
            if (idx < 0) return null;
            Debug.Assert(intArrays[idx].value == null);
            return intArrays[idx].value = array;
        }

        public InstanceableObject setObject(int idx, InstanceableObject o)
        {
            if (idx < 0) return null;
            Debug.Assert(objArrays[idx].value == null);
            return objArrays[idx].value = o;
        }
        public Color[] setColor(Color[] o, int idx)
        {
            if (idx < 0) return null;
            Debug.Assert(colorArrays[idx].value == null);
            return colorArrays[idx].value = o;
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
            Debug.Assert(idx >= 0);
            ProcMesh e = procMeshes[idx].pop();
            return e;
        }
        public ProcInstanceSet getInsatnceSet(int idx)
        {
            Debug.Assert(idx >= 0);
            ProcInstanceSet e = procInstanceSets[idx].pop();
            return e;
        }
        public float3[] getFloat3(int idx)
        {
            Debug.Assert(idx >= 0);
            float3[] e = float3Arrays[idx].pop();
            return e;
        }
        public float2[] getFloat2(int idx)
        {
            Debug.Assert(idx >= 0);
            float2[] e = float2Arrays[idx].pop();
            return e;
        }
        public float[] getFloat(int idx)
        {
            Debug.Assert(idx >= 0);
            float[] e = floatArrays[idx].pop();
            return e;
        }
        public float[] getMutableFloat(int idx)
        {

            float[] e = getFloat(idx);
            if (floatArrays[idx].value != null)
            {
                float[] copy = new float[e.Length];
                Array.Copy(e, copy, e.Length);
                return copy;
            }
            return e;
        }
        public int3[] getInt3(int idx)
        {
            Debug.Assert(idx >= 0);
            int3[] e = int3Arrays[idx].pop();
            return e;
        }
        public int2[] getInt2(int idx)
        {
            Debug.Assert(idx >= 0);
            int2[] e = int2Arrays[idx].pop();
            return e;
        }
        public int[] getInt(int idx)
        {
            Debug.Assert(idx >= 0);
            int[] e = intArrays[idx].pop();
            return e;
        }
        public Color[] getColor(int idx)
        {
            Debug.Assert(idx >= 0);
            Color[] e = colorArrays[idx].pop();
            return e;
        }
        public InstanceableObject getObject(int idx)
        {
            Debug.Assert(idx >= 0);
            InstanceableObject e = objArrays[idx].pop();
            return e;
        }

    }
}