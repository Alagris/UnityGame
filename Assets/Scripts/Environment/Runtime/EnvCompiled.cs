using System;
using System.Collections.Generic;
using System.Linq;
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
    [Serializable]
    public enum UVMode
    {
        NONE, REPEATING, GLOBAL
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
        public LandscapeCompiled(bool shadeFlat, UVMode uvMode, float uvScaling, int heightArg, int outputLandscapeArg)
        {
            this.uvScaling = uvScaling;
            this.uvMode = uvMode;
            this.shadeFlat = shadeFlat;
            this.heightArg = heightArg;
            this.outputLandscapeArg = outputLandscapeArg;
        }
        public void run(Blackboard bb)
        {
            float[] heights = bb.getFloat(heightArg);
            ProcMesh m = ProcMesh.gridPrecomputed(bb.offset, bb.resX, bb.resZ, bb.size, bb.size, heights, uvScaling, uvMode);
            
            if (shadeFlat)
            {
                m = m.ShadeFlat();
            }
            bb.setMesh(outputLandscapeArg, m);
        }
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
        public ReturnCompiled(int[] weightArgs)
        {
            this.weightArgs = weightArgs;
        }
        public void run(Blackboard bb)
        {
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
                bb.returnedTerrainWeights = new Texture2D(bb.resX, bb.resZ, TextureFormat.RGBA32, false, true, true);
                

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
                        bb.returnedTerrainWeights.SetPixel(x, z, new Color32(maxI, secondMaxI, (byte)(255 * maxValue), (byte)(255 * secondMaxValue)));
                    }
                }
            }
        }
    }
    
}
