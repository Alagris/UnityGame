using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace Env.Runtime
{
   
    
    public interface EnvCompiledFunction
    {
        public void run(Blackboard bb);
    }

    [Serializable]
    public abstract class AbstractNoiseGradCompiled : EnvCompiledFunction
    {
        [SerializeField]
        int positionXArg, positionZArg, outputGradientsArg;
        public AbstractNoiseGradCompiled( int positionXArg, int positionZArg, int outputGradientsArg)
        {
            this.positionXArg = positionXArg;
            this.positionZArg = positionZArg;
            this.outputGradientsArg = outputGradientsArg;
        }
        internal abstract float3 EvalNoise(float2 position);
        public void run(Blackboard bb)
        {
            if(positionXArg==-1 || positionZArg==-1)
            {
                float3[] o = ProcMesh.forEachPosition2D(bb.offset.xz, bb.resX, bb.resZ, bb.size, bb.size, (idx, pos)=> EvalNoise(pos));
                bb.setFloat3(outputGradientsArg, o); 
            }
            else
            {
                float3[] posX = bb.getFloat3(positionXArg);
                float3[] posZ = bb.getFloat3(positionZArg);
                float3[] o = bb.makeFloat3(outputGradientsArg, Mathf.Min(posX.Length, posZ.Length));
                for (int i = 0; i < o.Length; i++)
                {
                    // let X, Z be the global coordinates of the world.
                    // Define the following functions:
                    // f                := EvalNoise
                    // gX               := positionXArg
                    // gZ               := positionZArg
                    // We have the following derivatives:
                    // x = gX           := posX.z
                    // d x / d X        := posX.x
                    // d x / d Z        := posX.y
                    // z = gZ           := posZ.z
                    // d z / d X        := posZ.x
                    // d z / d Z        := posZ.y
                    // f(x,z) =         := posY.z
                    // d f(x, z) / d x  := posY.x
                    // d f(x, z) / d z  := posY.y

                    // consider derivative of f(x,z) wrt X. Then Z is held constant
                    // d f(x,z) / d X =  
                    //   = d f(x, z) / d x * d x / d X + d f(x, z) / d z * d z / d X
                    //   = posY.x          * posX.x    + posY.y          * posZ.x
                    // 
                    // similarly:
                    // d f(x,z) / d Z =  
                    //   = d f(x, z) / d x * d x / d Z + d f(x, z) / d z * d z / d Z
                    //   = posY.x          * posX.y    + posY.y          * posZ.y
                    float3 posY = EvalNoise(new float2(posX[i].z, posZ[i].z));
                    float der_EvalNoise_wrt_X = posY.x * posX[i].x + posY.y * posZ[i].x;
                    float der_EvalNoise_wrt_Z = posY.x * posX[i].y + posY.y * posZ[i].y;
                    o[i] = new float3(der_EvalNoise_wrt_X, der_EvalNoise_wrt_Z, posY.z);
                }
            }
            
            
        }
    }

    [Serializable]
    public abstract class AbstractInstanceNoiseCompiled : EnvCompiledFunction
    {
        [SerializeField]
        int instanceArg, outputHeightsArg;
        public AbstractInstanceNoiseCompiled(int instanceArg, int outputHeightsArg)
        {
            this.instanceArg = instanceArg;
            this.outputHeightsArg = outputHeightsArg;
        }
        internal abstract float EvalNoise(float3 position);
        public void run(Blackboard bb)
        {
            
            ProcInstanceSet instanceSets = bb.getInsatnceSet(instanceArg);
            if (instanceSets.Count == 0)
            {
                bb.makeFloat(outputHeightsArg, 0);
                return;
            }
            else if (instanceSets.Count != 1)
            {
                Debug.LogError("Warning! Attempting to generate noise for instances containing mix of different instanced objects. Only the first one will be generated!");
            }
            ProcInstances instances = instanceSets[0];
            float[] o = bb.makeFloat(outputHeightsArg, instances.Transforms.Count);
            for (int i = 0; i < o.Length; i++)
            {
                float posY = EvalNoise(instances.Transforms[i].Location);
                o[i] = posY;
            }
        }
    }
    [Serializable]
    public abstract class AbstractNoiseCompiled : EnvCompiledFunction
    {
        [SerializeField]
        int positionXArg, positionZArg, outputHeightsArg;
        public AbstractNoiseCompiled(int positionXArg, int positionZArg, int outputHeightsArg)
        {
            this.positionXArg = positionXArg;
            this.positionZArg = positionZArg;
            this.outputHeightsArg = outputHeightsArg;
        }
        internal abstract float EvalNoise(float2 position);
        public void run(Blackboard bb)
        {
            if (positionXArg == -1 || positionZArg == -1)
            {
                float[] o = ProcMesh.forEachPosition2D(bb.offset.xz, bb.resX, bb.resZ, 1, bb.size, bb.size, (idx, pos) => EvalNoise(pos));
                bb.setFloat(outputHeightsArg, o);
            }
            else
            {
                float[] posX = bb.getFloat(positionXArg);
                float[] posZ = bb.getFloat(positionZArg);
                float[] o = bb.makeFloat(outputHeightsArg, Mathf.Min(posX.Length, posZ.Length));
                for (int i = 0; i < o.Length; i++)
                {  
                    float posY = EvalNoise(new float2(posX[i], posZ[i]));
                    o[i] = posY;
                }
            }
        }
    }
    [Serializable]
    public class PerlinFbmCompiled : AbstractNoiseCompiled
    {
        [SerializeField]
        float scale, heightPowerBase, scalePowerBase, height;
        [SerializeField]
        int iterations;
        public PerlinFbmCompiled(float scale, float heightPowerBase, float scalePowerBase, int iterations, float height, int positionXArg, int positionZArg, int outputGradientsArg): base(positionXArg, positionZArg, outputGradientsArg)
        {
            this.scale = scale;
            this.heightPowerBase = heightPowerBase;
            this.scalePowerBase = scalePowerBase;
            this.iterations = iterations;
            this.height = height;

        }
        internal override float EvalNoise(float2 pos)
        {
            return Noise.perlin_fbm(pos, scale, height, heightPowerBase, scalePowerBase, iterations);
        }
    }
    [Serializable]
    public class PerlinFbmInstanceCompiled : AbstractInstanceNoiseCompiled
    {
        [SerializeField]
        float scale, heightPowerBase, scalePowerBase, height;
        [SerializeField]
        int iterations;
        public PerlinFbmInstanceCompiled(float scale, float heightPowerBase, float scalePowerBase, int iterations, float height, int instanceArg, int outputGradientsArg) : base(instanceArg, outputGradientsArg)
        {
            this.scale = scale;
            this.heightPowerBase = heightPowerBase;
            this.scalePowerBase = scalePowerBase;
            this.iterations = iterations;
            this.height = height;

        }
        internal override float EvalNoise(float3 pos)
        {
            return Noise.perlin_fbm(new float2(pos.x, pos.z), scale, height, heightPowerBase, scalePowerBase, iterations);
        }
    }
    [Serializable]
    public class PerlinCompiled : AbstractNoiseCompiled
    {
        [SerializeField]
        float scale, height;
        public PerlinCompiled(float scale, float height, int positionXArg, int positionZArg, int outputGradientsArg): base(positionXArg, positionZArg, outputGradientsArg)
        {
            this.scale = scale;
            this.height = height;
        }
        internal override float EvalNoise(float2 pos)
        {
            return Noise.perlin_noise(pos, scale) * height;
        }
    }

    [Serializable]
    public class PerlinInstanceCompiled : AbstractInstanceNoiseCompiled
    {
        [SerializeField]
        float scale, height;
        public PerlinInstanceCompiled(float scale, float height, int instanceArg, int outputGradientsArg) : base(instanceArg, outputGradientsArg)
        {
            this.scale = scale;
            this.height = height;
        }
        internal override float EvalNoise(float3 pos)
        {
            return Noise.perlin_noise(new float2(pos.x, pos.z), scale) * height;
        }
    }
    [Serializable]
    public class WhiteNoiseCompiled : AbstractNoiseCompiled
    {
        [SerializeField]
        float min, range;
        public WhiteNoiseCompiled(float min, float max, int positionXArg, int positionZArg, int outputGradientsArg) : base(positionXArg, positionZArg, outputGradientsArg)
        {
            this.min = min;
            this.range = max-min;
        }
        internal override float EvalNoise(float2 pos)
        {
            return Noise.hash_float_to_float(pos) * range + min;
        }
    }
    [Serializable]
    public class WhiteNoiseInstanceCompiled : AbstractInstanceNoiseCompiled
    {
        [SerializeField]
        float min, range;
        public WhiteNoiseInstanceCompiled(float min, float max, int instanceArg, int outputGradientsArg) : base(instanceArg, outputGradientsArg)
        {
            this.min = min;
            this.range = max - min;
        }
        internal override float EvalNoise(float3 pos)
        {
            return Noise.hash_float_to_float(pos) * range + min;
        }
    }
    [Serializable]
    public class LinearGradCompiled : AbstractNoiseGradCompiled
    {
        [SerializeField]
        float2 scale, height;
        public LinearGradCompiled(Vector2 scale, Vector2 height, int positionXArg, int positionZArg, int outputGradientsArg) : base(positionXArg, positionZArg, outputGradientsArg)
        {
            this.scale = new float2(scale);
            this.height = new float2(height);
        }
        internal override float3 EvalNoise(float2 pos)
        {
            Vector2 y = pos * scale + height;
            return new float3(scale, y.x + y.y);
        }
    }
    [Serializable]
    public class LinearCompiled : AbstractNoiseCompiled
    {
        [SerializeField]
        float2 scale, height;
        public LinearCompiled(Vector2 scale, Vector2 height, int positionXArg, int positionZArg, int outputGradientsArg) : base(positionXArg, positionZArg, outputGradientsArg)
        {
            this.scale = new float2(scale);
            this.height = new float2(height);
        }
        internal override float EvalNoise(float2 pos)
        {
            Vector2 y = pos * scale + height;
            return y.x+y.y;
        }
    }

    [Serializable]
    public abstract class AbstractMutableBinaryCompiled : EnvCompiledFunction
    {
        [SerializeField]
        int aArg, bArg, outputArg;
        public AbstractMutableBinaryCompiled(int aArg, int bArg, int outputArg)
        {
            this.aArg = aArg;
            this.bArg = bArg;
            this.outputArg = outputArg;
        }
        public abstract void Eval(float[] a, float[] b);
        public void run(Blackboard bb)
        {

            float[] a = bb.getMutableFloat(aArg);
            float[] b = bb.getFloat(bArg);
            Eval(a, b);
            bb.setFloat(outputArg, a);

        }
    }

    [Serializable]
    public abstract class AbstractBinaryCompiled : EnvCompiledFunction
    {
        [SerializeField]
        int aArg, bArg, outputArg;
        public AbstractBinaryCompiled(int aArg, int bArg, int outputArg) 
        {
            this.aArg = aArg;
            this.bArg = bArg;
            this.outputArg = outputArg;
        }
        public abstract void Eval(float[] a, float[] b, float[] c);
        public void run(Blackboard bb)
        {
            
             float[] a = bb.getFloat(aArg);
             float[] b = bb.getFloat(bArg);
             float[] o = bb.makeFloat(outputArg, Mathf.Min(a.Length, b.Length));
             Eval(a, b, o);
            
        }
    }
    [Serializable]
    public class MultiplyCompiled : AbstractMutableBinaryCompiled
    {
        
        public MultiplyCompiled(int aArg, int bArg, int outputArg) : base(aArg, bArg, outputArg) { }
        
       

        public override void Eval(float[] a, float[] b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                a[i]*=b[i];
            }
        }
    }
    [Serializable]
    public class AddCompiled : AbstractMutableBinaryCompiled
    {

        public AddCompiled(int aArg, int bArg, int outputArg) : base(aArg, bArg, outputArg) { }



        public override void Eval(float[] a, float[] b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                a[i] += b[i];
            }
        }
    }
    [Serializable]
    public class SubCompiled : AbstractMutableBinaryCompiled
    {

        public SubCompiled(int aArg, int bArg, int outputArg) : base(aArg, bArg, outputArg) { }



        public override void Eval(float[] a, float[] b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                a[i] -= b[i];
            }
        }
    }
    
    
    [Serializable]
    public class ErosionNoiseCompiled : AbstractNoiseCompiled
    {
        [SerializeField]
        float scale, pointiness, scalingPowerBase, height;
        [SerializeField]
        int iterations;
        public ErosionNoiseCompiled(float scale, float pointiness, float scalingPowerBase, int iterations, float height, int positionXArg, int positionZArg, int outputGradientsArg) : base(positionXArg, positionZArg, outputGradientsArg)
        {
            this.scale = scale;
            this.pointiness = pointiness;
            this.scalingPowerBase = scalingPowerBase;
            this.iterations = iterations;
            this.height = height;
        }
        internal override float EvalNoise(float2 position)
        {
            return Noise.morenoise(position, scale, pointiness, scalingPowerBase, iterations) * height;
        }
    }
    public class ErosionNoiseInstanceCompiled : AbstractInstanceNoiseCompiled
    {
        [SerializeField]
        float scale, pointiness, scalingPowerBase, height;
        [SerializeField]
        int iterations;
        public ErosionNoiseInstanceCompiled(float scale, float pointiness, float scalingPowerBase, int iterations, float height, int instanceArg, int outputGradientsArg) : base(instanceArg, outputGradientsArg)
        {
            this.scale = scale;
            this.pointiness = pointiness;
            this.scalingPowerBase = scalingPowerBase;
            this.iterations = iterations;
            this.height = height;
        }
        internal override float EvalNoise(float3 position)
        {
            return Noise.morenoise(new float2(position.x, position.z), scale, pointiness, scalingPowerBase, iterations) * height;
        }
    }
    [Serializable]
    public enum UVMode
    {
        NONE, REPEATING, REPEATING_PADDED, REPEATING_UNNORMALIZED, REPEATING_PADDED_UNNORMALIZED, GLOBAL
    }
    [Serializable]
    public class LandscapeCompiled : EnvCompiledFunction
    {
        [SerializeField]
        bool shadeFlat;
        [SerializeField]
        float uvScaling;
        [SerializeField]
        int heightArg, outputLandscapeArg;
        [SerializeField]
        UVMode uvMode;
        [SerializeField]
        int layer;
        public LandscapeCompiled(bool shadeFlat, UVMode uvMode, float uvScaling, int heightArg, int outputLandscapeArg, int layer)
        {
            this.uvScaling = uvScaling;
            this.uvMode = uvMode;
            this.shadeFlat = shadeFlat;
            this.heightArg = heightArg;
            this.outputLandscapeArg = outputLandscapeArg;
            this.layer = layer;
        }
        public void run(Blackboard bb)
        {
            float[] heights = bb.getFloat(heightArg);
            ProcMesh m = ProcMesh.gridPrecomputed(bb.offset, bb.resX, bb.resZ, bb.size, bb.size, heights, uvScaling, uvMode);
            
            if (shadeFlat)
            {
                m = m.ShadeFlat();
            }
            m.Layer = layer;
            bb.setMesh(outputLandscapeArg, m);
        }
    }
    [Serializable]
    public abstract class SamplePointsCompiled<T> : EnvCompiledFunction
    {
        [SerializeField]
        protected int inIdx, inAttrIdx, outAttrIdx;
        public SamplePointsCompiled(int inIdx, int inAttrIdx, int outAttrIdx)
        {
            this.inIdx = inIdx;
            this.inAttrIdx = inAttrIdx;
            this.outAttrIdx = outAttrIdx;
        }
        protected abstract T[] readAttrArray(Blackboard bb);
        protected abstract T[] writeAttrArray(Blackboard bb, int count);
        protected abstract T interpolate(T bottomLeft, T bottomRight, T topLeft, T topRight, float x, float z);
        public void run(Blackboard bb)
        {
            T[] inAttrs = readAttrArray(bb);
            T[] outAttrs;
            const int padding = 1;
            int resXPadded = bb.resX + 2 * padding;
            Debug.Assert(resXPadded  * (bb.resZ + 2 * padding) == inAttrs.Length); 
            ProcInstanceSet instanceSets = bb.getInsatnceSet(inIdx);
            if (instanceSets.Count == 0)
            {
                outAttrs = writeAttrArray(bb, 0);
                return;
            }
            else if (instanceSets.Count != 1)
            {
                Debug.LogError("Warning! Attempting to sample points of instances containing mix of different instanced objects. Only the first one will be sampled!");
            }
            ProcInstances instances = instanceSets[0];
            outAttrs = writeAttrArray(bb, instances.Transforms.Count);
            for (int i = 0; i<instances.Transforms.Count; i++)
            {
                float3 position = (instances.Transforms[i].Location - bb.offset)/bb.size;
                float cellX = position.x * bb.resX;
                float cellZ = position.z * bb.resZ;
                int cellIntX = (int)cellX + padding;
                int cellIntZ = (int)cellZ + padding;
                int bottomLeft = cellIntZ * resXPadded + cellIntX;
                int bottomRight = cellIntZ * resXPadded + cellIntX + 1;
                int topLeft = (cellIntZ + 1)* resXPadded + cellIntX;
                int topRight = (cellIntZ + 1)* resXPadded + cellIntX + 1;
                float remainderX = cellX - (float)cellIntX;
                float remainderZ = cellZ - (float)cellIntZ;
                T interpolated = interpolate(inAttrs[bottomLeft], inAttrs[bottomRight], inAttrs[topLeft], inAttrs[topRight], remainderX, remainderZ);
                outAttrs[i] = interpolated;
            }
        }
    }
    [Serializable]
    public class SamplePointsCompiledFloat : SamplePointsCompiled<float>
    {
        public SamplePointsCompiledFloat(int inIdx, int inAttrIdx, int outAttrIdx) : base(inIdx, inAttrIdx, outAttrIdx)
        {
        }

        protected override float interpolate(float bottomLeft, float bottomRight, float topLeft, float topRight, float x, float z)
        {
            return Noise.mix(bottomLeft, bottomRight, topLeft, topRight, x, z);
        }

        protected override float[] readAttrArray(Blackboard bb)
        {
            return bb.getFloat(inAttrIdx);
        }

        protected override float[] writeAttrArray(Blackboard bb, int count)
        {
            return bb.makeFloat(inAttrIdx, count);
        }
    }

    [Serializable]
    public class SamplePointsCompiledFloat2 : SamplePointsCompiled<float2>
    {
        public SamplePointsCompiledFloat2(int inIdx, int inAttrIdx, int outAttrIdx) : base(inIdx, inAttrIdx, outAttrIdx)
        {
        }

        protected override float2 interpolate(float2 bottomLeft, float2 bottomRight, float2 topLeft, float2 topRight, float x, float z)
        {
            return Noise.mix(bottomLeft, bottomRight, topLeft, topRight, x, z);
        }

        protected override float2[] readAttrArray(Blackboard bb)
        {
            return bb.getFloat2(inAttrIdx);
        }

        protected override float2[] writeAttrArray(Blackboard bb, int count)
        {
            return bb.makeFloat2(inAttrIdx, count);
        }
    }
    [Serializable]
    public class SamplePointsCompiledFloat3 : SamplePointsCompiled<float3>
    {
        public SamplePointsCompiledFloat3(int inIdx, int inAttrIdx, int outAttrIdx) : base(inIdx, inAttrIdx, outAttrIdx)
        {
        }

        protected override float3 interpolate(float3 bottomLeft, float3 bottomRight, float3 topLeft, float3 topRight, float x, float z)
        {
            return Noise.mix(bottomLeft, bottomRight, topLeft, topRight, x, z);
        }

        protected override float3[] readAttrArray(Blackboard bb)
        {
            return bb.getFloat3(inAttrIdx);
        }

        protected override float3[] writeAttrArray(Blackboard bb, int count)
        {
            return bb.makeFloat3(inAttrIdx, count);
        }
    }
    [Serializable]
    public class SamplePointsCompiledFloat4 : SamplePointsCompiled<float4>
    {
        public SamplePointsCompiledFloat4(int inIdx, int inAttrIdx, int outAttrIdx) : base(inIdx, inAttrIdx, outAttrIdx)
        {
        }

        protected override float4 interpolate(float4 bottomLeft, float4 bottomRight, float4 topLeft, float4 topRight, float x, float z)
        {
            return Noise.mix(bottomLeft, bottomRight, topLeft, topRight, x, z);
        }

        protected override float4[] readAttrArray(Blackboard bb)
        {
            return bb.getFloat4(inAttrIdx);
        }

        protected override float4[] writeAttrArray(Blackboard bb, int count)
        {
            return bb.makeFloat4(inAttrIdx, count);
        }
    }
    [Serializable]
    public class SamplePointsCompiledInt : SamplePointsCompiled<int>
    {
        public SamplePointsCompiledInt(int inIdx, int inAttrIdx, int outAttrIdx) : base(inIdx, inAttrIdx, outAttrIdx)
        {
        }

        protected override int interpolate(int bottomLeft, int bottomRight, int topLeft, int topRight, float x, float z)
        {
            return Noise.nearest(bottomLeft, bottomRight, topLeft, topRight, x, z);
        }

        protected override int[] readAttrArray(Blackboard bb)
        {
            return bb.getInt(inAttrIdx);
        }

        protected override int[] writeAttrArray(Blackboard bb, int count)
        {
            return bb.makeInt(inAttrIdx, count);
        }
    }
    [Serializable]
    public class SamplePointsCompiledInt2 : SamplePointsCompiled<int2>
    {
        public SamplePointsCompiledInt2(int inIdx, int inAttrIdx, int outAttrIdx) : base(inIdx, inAttrIdx, outAttrIdx)
        {
        }

        protected override int2 interpolate(int2 bottomLeft, int2 bottomRight, int2 topLeft, int2 topRight, float x, float z)
        {
            return Noise.nearest(bottomLeft, bottomRight, topLeft, topRight, x, z);
        }

        protected override int2[] readAttrArray(Blackboard bb)
        {
            return bb.getInt2(inAttrIdx);
        }

        protected override int2[] writeAttrArray(Blackboard bb, int count)
        {
            return bb.makeInt2(inAttrIdx, count);
        }
    }
    [Serializable]
    public class SamplePointsCompiledInt3 : SamplePointsCompiled<int3>
    {
        public SamplePointsCompiledInt3(int inIdx, int inAttrIdx, int outAttrIdx) : base(inIdx, inAttrIdx, outAttrIdx)
        {
        }

        protected override int3 interpolate(int3 bottomLeft, int3 bottomRight, int3 topLeft, int3 topRight, float x, float z)
        {
            return Noise.nearest(bottomLeft, bottomRight, topLeft, topRight, x, z);
        }

        protected override int3[] readAttrArray(Blackboard bb)
        {
            return bb.getInt3(inAttrIdx);
        }

        protected override int3[] writeAttrArray(Blackboard bb, int count)
        {
            return bb.makeInt3(inAttrIdx, count);
        }
    }
    [Serializable]
    public class SamplePointsCompiledInt4 : SamplePointsCompiled<int4>
    {
        public SamplePointsCompiledInt4(int inIdx, int inAttrIdx, int outAttrIdx) : base(inIdx, inAttrIdx, outAttrIdx)
        {
        }

        protected override int4 interpolate(int4 bottomLeft, int4 bottomRight, int4 topLeft, int4 topRight, float x, float z)
        {
            return Noise.nearest(bottomLeft, bottomRight, topLeft, topRight, x, z);
        }

        protected override int4[] readAttrArray(Blackboard bb)
        {
            return bb.getInt4(inAttrIdx);
        }

        protected override int4[] writeAttrArray(Blackboard bb, int count)
        {
            return bb.makeInt4(inAttrIdx, count);
        }
    }
    [Serializable]
    public abstract class SetAttributeCompiled<T> : EnvCompiledFunction
    {
        [SerializeField]
        private string attrName;
        [SerializeField]
        protected int inIdx,attrIdx,outIdx;
        public SetAttributeCompiled(string attrName, int inIdx, int attrIdx, int outIdx)
        {
            this.attrName = attrName;
            this.inIdx = inIdx;
            this.attrIdx = attrIdx;
            this.outIdx = outIdx;
        }
        protected abstract T[] readArray(Blackboard bb);
        protected abstract Attribute toAttr(T[] array);
        public void run(Blackboard bb)
        {
            ProcInstanceSet instanceSets = bb.getMutableInsatnceSet(inIdx);
            T[] attrs = readArray(bb);
            if (instanceSets.Count > 0)
            {
                Debug.Assert(instanceSets[0].Transforms.Count == attrs.Length);
                instanceSets[0].Attributes[attrName] = toAttr(attrs);
            }
        }
    }
    [Serializable]
    public class SetAttributeCompiledFloat : SetAttributeCompiled<float>
    {
        public SetAttributeCompiledFloat(string attrName, int inIdx, int attrIdx, int outIdx):base(attrName, inIdx, attrIdx, outIdx){}

        protected override float[] readArray(Blackboard bb) =>bb.getFloat(attrIdx);

        protected override Attribute toAttr(float[] array) =>new AttributeFloat(array);
    }
    [Serializable]
    public class SetAttributeCompiledFloat2 : SetAttributeCompiled<float2>
    {
        public SetAttributeCompiledFloat2(string attrName, int inIdx, int attrIdx, int outIdx) : base(attrName, inIdx, attrIdx, outIdx) { }

        protected override float2[] readArray(Blackboard bb) => bb.getFloat2(attrIdx);

        protected override Attribute toAttr(float2[] array) => new AttributeFloat2(array);
    }
    [Serializable]
    public class SetAttributeCompiledFloat3 : SetAttributeCompiled<float3>
    {
        public SetAttributeCompiledFloat3(string attrName, int inIdx, int attrIdx, int outIdx) : base(attrName, inIdx, attrIdx, outIdx) { }

        protected override float3[] readArray(Blackboard bb) => bb.getFloat3(attrIdx);

        protected override Attribute toAttr(float3[] array) => new AttributeFloat3(array);
    }
    [Serializable]
    public class SetAttributeCompiledFloat4 : SetAttributeCompiled<float4>
    {
        public SetAttributeCompiledFloat4(string attrName, int inIdx, int attrIdx, int outIdx) : base(attrName, inIdx, attrIdx, outIdx) { }

        protected override float4[] readArray(Blackboard bb) => bb.getFloat4(attrIdx);

        protected override Attribute toAttr(float4[] array) => new AttributeFloat4(array);
    }


    [Serializable]
    public class SetAttributeCompiledInt : SetAttributeCompiled<int>
    {
        public SetAttributeCompiledInt(string attrName, int inIdx, int attrIdx, int outIdx) : base(attrName, inIdx, attrIdx, outIdx) { }

        protected override int[] readArray(Blackboard bb) => bb.getInt(attrIdx);

        protected override Attribute toAttr(int[] array) => new AttributeInt(array);
    }
    [Serializable]
    public class SetAttributeCompiledInt2 : SetAttributeCompiled<int2>
    {
        public SetAttributeCompiledInt2(string attrName, int inIdx, int attrIdx, int outIdx) : base(attrName, inIdx, attrIdx, outIdx) { }

        protected override int2[] readArray(Blackboard bb) => bb.getInt2(attrIdx);

        protected override Attribute toAttr(int2[] array) => new AttributeInt2(array);
    }
    [Serializable]
    public class SetAttributeCompiledInt3 : SetAttributeCompiled<int3>
    {
        public SetAttributeCompiledInt3(string attrName, int inIdx, int attrIdx, int outIdx) : base(attrName, inIdx, attrIdx, outIdx) { }

        protected override int3[] readArray(Blackboard bb) => bb.getInt3(attrIdx);

        protected override Attribute toAttr(int3[] array) => new AttributeInt3(array);
    }
    [Serializable]
    public class SetAttributeCompiledInt4 : SetAttributeCompiled<int4>
    {
        public SetAttributeCompiledInt4(string attrName, int inIdx, int attrIdx, int outIdx) : base(attrName, inIdx, attrIdx, outIdx) { }

        protected override int4[] readArray(Blackboard bb) => bb.getInt4(attrIdx);

        protected override Attribute toAttr(int4[] array) => new AttributeInt4(array);
    }



    [Serializable]
    public class SetAttributeCompiledUInt : SetAttributeCompiled<int>
    {
        public SetAttributeCompiledUInt(string attrName, int inIdx, int attrIdx, int outIdx) : base(attrName, inIdx, attrIdx, outIdx) { }

        protected override int[] readArray(Blackboard bb) => bb.getInt(attrIdx);

        protected override Attribute toAttr(int[] array) => new AttributeUint(array);
    }
    [Serializable]
    public class SetAttributeCompiledUInt2 : SetAttributeCompiled<int2>
    {
        public SetAttributeCompiledUInt2(string attrName, int inIdx, int attrIdx, int outIdx) : base(attrName, inIdx, attrIdx, outIdx) { }

        protected override int2[] readArray(Blackboard bb) => bb.getInt2(attrIdx);

        protected override Attribute toAttr(int2[] array) => new AttributeUint2(array);
    }
    [Serializable]
    public class SetAttributeCompiledUInt3 : SetAttributeCompiled<int3>
    {
        public SetAttributeCompiledUInt3(string attrName, int inIdx, int attrIdx, int outIdx) : base(attrName, inIdx, attrIdx, outIdx) { }

        protected override int3[] readArray(Blackboard bb) => bb.getInt3(attrIdx);

        protected override Attribute toAttr(int3[] array) => new AttributeUint3(array);
    }
    [Serializable]
    public class SetAttributeCompiledUInt4 : SetAttributeCompiled<int4>
    {
        public SetAttributeCompiledUInt4(string attrName, int inIdx, int attrIdx, int outIdx) : base(attrName, inIdx, attrIdx, outIdx) { }

        protected override int4[] readArray(Blackboard bb) => bb.getInt4(attrIdx);

        protected override Attribute toAttr(int4[] array) => new AttributeUint4(array);
    }
    [Serializable]
    public class NormalizeCompiled : EnvCompiledFunction
    {
        [SerializeField]
        int[] inputArgs;
        [SerializeField]
        int[] outputArgs;

        public NormalizeCompiled(int[] outputArgs, int[] inputArgs)
        {
            this.outputArgs = outputArgs;
            this.inputArgs = inputArgs;
        }

        public void run(Blackboard bb)
        {
            int Count = inputArgs.Length;
            float[][] weights = new float[Count][];
            for (int i = 0; i < Count; i++)
            {
                int idx = inputArgs[i];
                weights[i] = idx < 0 ? null : bb.getMutableFloat(idx);
            }
            if (Count > 0)
            {
                int len = weights[0].Length;
                float[] sum = new float[len];
                int firstWithoutWeights = -1;
                for (int j = 0; j < Count; j++)
                {
                    float[] weight = weights[j];
                    if (weight == null)
                    {
                        if (firstWithoutWeights == -1)
                        {
                            firstWithoutWeights = j;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < len; i++)
                        {
                            sum[i] += weight[i];
                        }
                    }
                }
                if (firstWithoutWeights >= 0)
                {
                    if (Count == 1)
                    {
                        float[] missingWeight = new float[len];
                        Array.Fill(missingWeight, 1);
                        weights[firstWithoutWeights] = missingWeight;
                    }
                    else
                    {
                        float[] missingWeight = new float[len];
                        weights[firstWithoutWeights] = missingWeight;
                        float max = sum.Max();
                        for (int i = 0; i < len; i++)
                        {
                            missingWeight[i] = max - sum[i];
                        }
                        for (int j = 0; j < Count; j++)
                        {
                            float[] weight = weights[j];
                            if (weight != null)
                            {
                                for (int i = 0; i < len; i++)
                                {
                                    weight[i] /= max;
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < Count; j++)
                    {
                        float[] weight = weights[j];
                        if (weight != null)
                        {
                            for (int i = 0; i < len; i++)
                            {
                                weight[i] /= sum[i];
                            }
                        }
                    }
                }
                
            }
            for (int j = 0; j < Count; j++)
            {
                bb.setFloat(outputArgs[j], weights[j]);
            }
                
        }
    }
    
    [Serializable]
    public abstract class DistributeBaseCompiled : EnvCompiledFunction
    {
        [SerializeField]
        protected uint seed;
        [SerializeField]
        protected bool alignToNormal;
        [SerializeField]
        protected float minTilt;
        [SerializeField]
        protected float maxTilt;
        [SerializeField]
        protected float3 minScale;
        [SerializeField]
        protected float3 maxScale;
        [SerializeField]
        protected bool scaleUniformly;
        [SerializeField]
        protected int objectArg;
        [SerializeField]
        protected int inputLandscapeArg, outputInstancesArg;
        public DistributeBaseCompiled(uint seed, bool alignToNormal, float minTilt, float maxTilt, Vector3 minScale, Vector3 maxScale, bool scaleUniformly, int objectArg, int inputLandscapeArg, int outputInstancesArg)
        {
            this.seed = seed;
            this.alignToNormal = alignToNormal;
            this.minTilt = minTilt;
            this.maxTilt = maxTilt;
            this.minScale = minScale;
            this.maxScale = maxScale;
            this.scaleUniformly = scaleUniformly;
            this.objectArg = objectArg; // Object = InstanceableObject.From(Object);
            this.inputLandscapeArg = inputLandscapeArg;
            this.outputInstancesArg = outputInstancesArg;
        }
        protected abstract ProcInstances run(Blackboard bb, ProcInstances m, ProcMesh landscape, uint seed);
        public void run(Blackboard bb)
        {
            ProcMesh landscape = bb.getMesh(inputLandscapeArg);
            InstanceableObject o = bb.getObject(objectArg);
            ProcInstances m = new ProcInstances(o);
            uint s = Noise.hash_float(new float2(bb.offset.x, bb.offset.z), seed);
            run(bb, m, landscape, s);
            bb.setInsatnceSet(outputInstancesArg, m);
        }
    }
    [Serializable]
    public class DistributeCompiled : DistributeBaseCompiled
    {
        
        [SerializeField]
        int inputDensityArg;
        public DistributeCompiled(uint seed, bool alignToNormal, float minTilt, float maxTilt, Vector3 minScale, Vector3 maxScale, bool scaleUniformly, int objectArg, int inputLandscapeArg, int inputDensityArg, int outputInstancesArg):base(seed,alignToNormal,minTilt,maxTilt,minScale,maxScale, scaleUniformly,objectArg, inputLandscapeArg,outputInstancesArg)
        {
            this.inputDensityArg = inputDensityArg;
            
        }
        protected override ProcInstances run(Blackboard bb, ProcInstances m,ProcMesh landscape, uint seed)
        {
            float[] density = bb.getFloat(inputDensityArg);
            
            return landscape.distributePoints(m, density, seed,alignToNormal,minTilt,maxTilt,minScale,maxScale, scaleUniformly);
        }
    }
    [Serializable]
    public class DistributeUniformCompiled : DistributeBaseCompiled
    {
        [SerializeField]
        float desnityUniform;
        public DistributeUniformCompiled(uint seed, bool alignToNormal, float minTilt, float maxTilt, Vector3 minScale, Vector3 maxScale, bool scaleUniformly, int objectArg, int inputLandscapeArg, float desnityUniform, int outputInstancesArg) : base(seed, alignToNormal, minTilt, maxTilt, minScale, maxScale, scaleUniformly, objectArg, inputLandscapeArg, outputInstancesArg)
        {
            
            this.desnityUniform = desnityUniform;
            
        }
        protected override ProcInstances run(Blackboard bb, ProcInstances m, ProcMesh landscape, uint seed)
        {
            return landscape.distributePointsUniform(m, desnityUniform, seed, alignToNormal, minTilt, maxTilt,minScale,maxScale, scaleUniformly);
        }
    }
    [Serializable]
    public class JoinInstancesCompiled : EnvCompiledFunction
    {
        [SerializeField]
        int[] inputArgs;
        [SerializeField]
        int outArg;
        public JoinInstancesCompiled(int[] inputArgs, int outArg)
        {
            this.inputArgs = inputArgs;
            this.outArg = outArg;
        }
        public void run(Blackboard bb)
        {
            if (inputArgs.Length == 1)
            {
                bb.setInsatnceSet(outArg, bb.getInsatnceSet(inputArgs[0]));
            }
            else
            {
                ProcInstanceSet output = new ProcInstanceSet();
                foreach (int i in inputArgs)
                {
                    ProcInstanceSet input = bb.getInsatnceSet(i);
                    output.AddRange(input);
                }
                bb.setInsatnceSet(outArg, output);
            }
        }
    }
    [Serializable]
    public class OverrideMaterialsCompiled : EnvCompiledFunction
    {
        [SerializeField]
        List<Material> mats;
        [SerializeField]
        int inArg;
        [SerializeField]
        int outArg;
        public OverrideMaterialsCompiled(int inArg, int outArg, List<Material> mats)
        {
            this.inArg = inArg;
            this.mats = mats;
            this.outArg = outArg;
        }
        public void run(Blackboard bb)
        {
            ProcInstanceSet a = bb.getInsatnceSet(inArg);
            bb.setInsatnceSet(outArg, a.SetMaterials(mats));
        }
    }
    
    [Serializable]
    public class LoadInstanceableObjectAssetCompiled : EnvCompiledFunction
    {
        
        [SerializeField]
        InstanceableObjectAsset asset;
        [SerializeField]
        int outArg;
        public LoadInstanceableObjectAssetCompiled(int outArg, InstanceableObjectAsset asset)
        {
            this.asset = asset;
            this.outArg = outArg;
        }
        public void run(Blackboard bb)
        {
            InstanceableObject o = new InstanceableObject(asset);
            bb.setObject(outArg, o);
        }
    }
    [Serializable]
    public class LoadLODGroupCompiled : EnvCompiledFunction
    {
        [SerializeField]
        List<Material> mats;
        [SerializeField]
        LODGroup mesh;
        [SerializeField]
        int outArg;
        public LoadLODGroupCompiled(int outArg, List<Material> mats, LODGroup mesh)
        {
            this.mesh = mesh;
            this.mats = mats;
            this.outArg = outArg;
        }
        public void run(Blackboard bb)
        {
            InstanceableObject o = new InstanceableObject(mesh);
            o.SetMaterials(mats);
            bb.setObject(outArg, o);
        }
    }

    [Serializable]
    public class LoadStaticMeshCompiled : EnvCompiledFunction
    {
        [SerializeField]
        List<Material> mats;
        [SerializeField]
        MeshRenderer mesh;
        [SerializeField]
        int outArg;
        public LoadStaticMeshCompiled(int outArg, List<Material> mats, MeshRenderer mesh)
        {
            this.mesh = mesh;
            this.mats = mats;
            this.outArg = outArg;
        }
        public void run(Blackboard bb)
        {
            InstanceableObject o = new InstanceableObject(mesh);
            o.SetMaterials(mats);
            bb.setObject(outArg, o);
        }
    }


    [Serializable]
    public class ReturnCompiled : EnvCompiledFunction
    {
        [SerializeField]
        int[] weightArgs;
        [SerializeField]
        string matWeightsParam;
        [SerializeField]
        Material landscapeMaterial;
        public ReturnCompiled(int[] weightArgs, string matWeightsParam, Material landscapeMaterial)
        {
            this.weightArgs = weightArgs;
            this.matWeightsParam = matWeightsParam;
            this.landscapeMaterial = landscapeMaterial;
        }
        public void run(Blackboard bb)
        {
            Material sectionMaterial = landscapeMaterial;
            if (landscapeMaterial != null) {
                List<float[]> weights = new List<float[]>(weightArgs.Length);
                List<int> idxMapping = new List<int>(weightArgs.Length);
                const int padding = 1;
                for (int i = 0; i < weightArgs.Length; i++)
                {
                    if (weightArgs[i] >= 0)
                    {
                        idxMapping.Add(i);
                        weights.Add(bb.getFloat(weightArgs[i]));
                        Debug.Assert((bb.resX + 2 * padding) * (bb.resZ + 2 * padding) == weights[i].Length);
                    }
                }

                
                if (weights.Count > 1)
                {
                    
                    Texture2D terrainWeights = new Texture2D(bb.resX, bb.resZ, TextureFormat.RGBA32, false,  true, true);


                    for (int z = 0, j = 0; z < bb.resZ + 2; z++)
                    {
                        for (int x = 0; x < bb.resZ + 2; j++, x++)
                        {

                            int maxIdx, secondMaxIdx;
                            if (weights[0][j] > weights[1][j])
                            {
                                maxIdx = 0;
                                secondMaxIdx = 1;
                            }
                            else
                            {
                                maxIdx = 1;
                                secondMaxIdx = 0;
                            }
                            for (int i = 2; i < weights.Count; i++)
                            {
                                if (weights[i][j] > weights[secondMaxIdx][j])
                                {
                                    if (weights[i][j] > weights[maxIdx][j])
                                    {
                                        secondMaxIdx = maxIdx;
                                        maxIdx = i;
                                    }
                                    else
                                    {
                                        secondMaxIdx = i;
                                    }
                                }
                            }
                            float maxValue = weights[maxIdx][j];
                            float secondMaxValue = weights[secondMaxIdx][j];
                            float sum = maxValue + secondMaxValue;
                            maxValue = maxValue / sum;
                            secondMaxValue = secondMaxValue / sum;
                            byte maxI = (byte)idxMapping[maxIdx];
                            byte secondMaxI = (byte)idxMapping[secondMaxIdx];
                            byte maxValueI = (byte)(255 * maxValue);
                            byte secondMaxValueI = (byte)(255 * secondMaxValue);
                            terrainWeights.SetPixel(x, z, new Color32(maxI, secondMaxI, maxValueI, secondMaxValueI));
                        }
                    }
                    sectionMaterial = new Material(landscapeMaterial);
                    
                    sectionMaterial.SetTexture(matWeightsParam, terrainWeights);
                }
            }
            bb.returnedTerrainMaterial = sectionMaterial;
        }
    }
    
}
