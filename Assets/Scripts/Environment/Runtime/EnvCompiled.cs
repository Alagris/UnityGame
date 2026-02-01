using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

namespace Env.Runtime
{
   
    
    public interface EnvCompiledFunction
    {
        public void run(Blackboard bb);
    }

    [Serializable]
    public abstract class AbstractNoiseCompiled : EnvCompiledFunction
    {
        [SerializeField]
        int positionXArg, positionZArg, outputGradientsArg;
        public AbstractNoiseCompiled( int positionXArg, int positionZArg, int outputGradientsArg)
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
        internal override float3 EvalNoise(float2 pos)
        {
            return Noise.perlin_fbm_derivative(pos, scale, height, heightPowerBase, scalePowerBase, iterations);
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
        internal override float3 EvalNoise(float2 pos)
        {
            return Noise.perlin_noise_derivative(pos, scale) * height;
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
        internal override float3 EvalNoise(float2 position)
        {
            return Noise.morenoise(position, scale, pointiness, scalingPowerBase, iterations) * height;
        }
    }
    
    [Serializable]
    public class LandscapeCompiled : EnvCompiledFunction
    {
        [SerializeField]
        bool shadeFlat;
        [SerializeField]
        float uvScaling;
        [SerializeField]
        int gradientArg, outputLandscapeArg;
        public LandscapeCompiled(bool shadeFlat, float uvScaling, int gradientArg, int outputLandscapeArg)
        {
            this.uvScaling = uvScaling;
            this.shadeFlat = shadeFlat;
            this.gradientArg = gradientArg;
            this.outputLandscapeArg = outputLandscapeArg;
        }
        public void run(Blackboard bb)
        {
            float3[] gradients = bb.getFloat3(gradientArg);
            ProcMesh m = ProcMesh.gridPrecomputed(bb.offset, bb.resX, bb.resZ, bb.size, bb.size, gradients, uvScaling);
            if (shadeFlat)
            {
                m = m.ShadeFlat();
            }
            bb.setMesh(outputLandscapeArg, m);
        }
    }

    [Serializable]
    public class DistributeCompiled : EnvCompiledFunction
    {
        [SerializeField]
        private readonly uint seed;
        [SerializeField]
        private readonly bool alignToNormal;
        [SerializeField]
        private readonly float minTilt;
        [SerializeField]
        private readonly float maxTilt;
        [SerializeField]
        private readonly InstanceableObject Object;
        [SerializeField]
        private readonly int inputDensityArg, inputLandscapeArg, outputInstancesArg;
        public DistributeCompiled(uint seed, bool alignToNormal, float minTilt, float maxTilt, InstanceableObject Object, int inputLandscapeArg, int inputDensityArg, int outputInstancesArg)
        {
            this.seed = seed;
            this.alignToNormal = alignToNormal;
            this.minTilt = minTilt;
            this.maxTilt = maxTilt;
            this.Object = Object;
            this.inputLandscapeArg = inputLandscapeArg;
            this.inputDensityArg = inputDensityArg;
            this.outputInstancesArg = outputInstancesArg;
        }
        public void run(Blackboard bb)
        {
            ProcMesh landscape = bb.getMesh(inputLandscapeArg);
            float[] density = bb.getFloat(inputDensityArg);
            ProcInstances m = landscape.distributePoints(density, seed,alignToNormal,minTilt,maxTilt);
            m.Object = Object;
            bb.setInsatnceSet(outputInstancesArg, m);
        }
    }
    [Serializable]
    public class JoinInstancesCompiled : EnvCompiledFunction
    {
        
        private readonly int aArg, bArg, outArg;
        public JoinInstancesCompiled(int aArg, int bArg, int outArg)
        {
            this.aArg = aArg;
            this.bArg = bArg;
            this.outArg = outArg;
        }
        public void run(Blackboard bb)
        {
            ProcInstanceSet a = bb.getInsatnceSet(aArg);
            ProcInstanceSet b = bb.getInsatnceSet(aArg);
            bb.setInsatnceSet(outArg, a.join(b));
        }
    }

}
