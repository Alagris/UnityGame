using System;
using UnityEngine;
using Unity.GraphToolkit.Editor;
using Env.Runtime;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace Env.Editor
{
    [Serializable]
    public abstract class EnvNode : Node
    {
        public abstract EnvCompiledFunction compile(Dictionary<IPort, int> variables);

        internal static T Val<T>(INodeOption opt, T dflt)
        {
            T v;
            return opt.TryGetValue<T>(out v) ? v : dflt;
        }
        internal static float Float(INodeOption opt, float dflt)
        {
            return Val(opt, dflt);
        }
        internal static bool Bool(INodeOption opt, bool dflt)
        {
            return Val(opt, dflt);
        }
        internal static int Int(INodeOption opt, int dflt)
        {
            return Val(opt, dflt);
        }
        internal static uint UInt(INodeOption opt, uint dflt)
        {
            return Val(opt, dflt);
        }
        
    }


    [Serializable]
    public abstract class AnyNoiseNoise : EnvNode
    {
        IPort xPort;
        IPort zPort;
        IPort gPort;
        protected override void OnDefinePorts(IPortDefinitionContext ctx)
        {
            xPort = ctx.AddInputPort<Vector3>("X").Build();
            zPort = ctx.AddInputPort<Vector3>("Z").Build();
            gPort = ctx.AddOutputPort<Vector3>("Gradient").Build();
        }
        public abstract EnvCompiledFunction compile(int xArg, int zArg, int outGrad);
        public override EnvCompiledFunction compile(Dictionary<IPort, int> variables)
        {
             return compile(variables.GetValueOrDefault(xPort,-1), variables.GetValueOrDefault(zPort, -1), variables.GetValueOrDefault(gPort, -1));
        }
    }

    
    [Serializable]
    public class ErosionNoise : AnyNoiseNoise
    {

        INodeOption IterationsOpt;
        INodeOption ScaleOpt;
        INodeOption PointinessOpt;
        INodeOption ScalingPowerBaseOpt;
        INodeOption HeightOpt;
        public override EnvCompiledFunction compile(int xArg, int zArg, int outGrad)
        {
            return new ErosionNoiseCompiled(
                scale: Float(ScaleOpt, 1),
                pointiness: Float(PointinessOpt, 0.5f),
                scalingPowerBase: Float(ScalingPowerBaseOpt, 3),
                iterations: Int(IterationsOpt, 1),
                height: Float(HeightOpt, 30),
                positionXArg: xArg,
                positionZArg: zArg,
                outputGradientsArg: outGrad
           );
        }

        protected override void OnDefineOptions(IOptionDefinitionContext ctx)
        {
            IterationsOpt = ctx.AddOption<int>("Iterations").WithDefaultValue(1).Build();
            ScaleOpt = ctx.AddOption<float>("Scale").WithDefaultValue(1).Build();
            PointinessOpt = ctx.AddOption<float>("Pointiness").WithDefaultValue(0.5f).Build();
            ScalingPowerBaseOpt = ctx.AddOption<float>("ScalingPowerBase").WithDefaultValue(3).Build();
            HeightOpt = ctx.AddOption<float>("Height").WithDefaultValue(30).Build();
        }
        
    }


    [Serializable]
    public class PerlinFbmNoise : AnyNoiseNoise
    {
        INodeOption IterationsOpt;
        INodeOption ScaleOpt;
        INodeOption HeightPowerBaseOpt;
        INodeOption ScalePowerBaseOpt;
        INodeOption HeightOpt;
    
        public override EnvCompiledFunction compile(int xArg, int zArg, int outGrad)
        {
            return new PerlinFbmCompiled(
                scale: Float(ScaleOpt, 1),
                heightPowerBase: Float(HeightPowerBaseOpt, 0.5f),
                scalePowerBase: Float(ScalePowerBaseOpt, 0.5f),
                iterations: Int(IterationsOpt, 1),
                height: Float(HeightOpt, 30),
                positionXArg: xArg,
                positionZArg: zArg,
                outputGradientsArg: outGrad
           );
        }
        protected override void OnDefineOptions(IOptionDefinitionContext ctx)
        {
            IterationsOpt = ctx.AddOption<int>("Iterations").WithDefaultValue(1).Build();
            ScaleOpt = ctx.AddOption<float>("Scale").WithDefaultValue(1).Build();
            HeightPowerBaseOpt = ctx.AddOption<float>("HeightPowerBase").WithDefaultValue(0.5f).Build();
            ScalePowerBaseOpt = ctx.AddOption<float>("ScalePowerBase").WithDefaultValue(0.5f).Build();
            HeightOpt = ctx.AddOption<float>("Height").WithDefaultValue(30).Build();
        }
        
    }

    [Serializable]
    public class PerlinNoise : AnyNoiseNoise
    {
        INodeOption ScaleOpt;
        INodeOption HeightOpt;

        public override EnvCompiledFunction compile(int xArg, int zArg, int outGrad)
        {
            return new PerlinCompiled(
                scale: Float(ScaleOpt, 1),
                height: Float(HeightOpt, 30),
                positionXArg: xArg,
                positionZArg: zArg,
                outputGradientsArg: outGrad
           );
        }
        protected override void OnDefineOptions(IOptionDefinitionContext ctx)
        {
            ScaleOpt = ctx.AddOption<float>("Scale").WithDefaultValue(1).Build();
            HeightOpt = ctx.AddOption<float>("Height").WithDefaultValue(30).Build();
        }
        
    }



    [Serializable]
    public class LandscapeMesh : EnvNode
    {
        
        IPort GradientPort;
        IPort LandscapePort;
        INodeOption ShadeFlatOpt;
        INodeOption UVScalingOpt;
        public override EnvCompiledFunction compile(Dictionary<IPort, int> variables)
        {
            return new LandscapeCompiled(
                shadeFlat:Bool(ShadeFlatOpt, false),
                uvScaling:Float(UVScalingOpt, 1),
                gradientArg: variables.GetValueOrDefault(GradientPort, -1),
                outputLandscapeArg: variables.GetValueOrDefault(LandscapePort, -1)
                );
        }

        protected override void OnDefineOptions(IOptionDefinitionContext ctx)
        {
            ShadeFlatOpt = ctx.AddOption<bool>("ShadeFlat").WithDefaultValue(false).Build();
            UVScalingOpt = ctx.AddOption<float>("UV Scaling").WithDefaultValue(1).Build();
        }
        protected override void OnDefinePorts(IPortDefinitionContext ctx)
        {
            GradientPort = ctx.AddInputPort<Vector3>("Gradient").Build();
            LandscapePort = ctx.AddOutputPort<Mesh>("Landscape").Build();
        }
    }


    [Serializable]
    public class Distribute : EnvNode
    {
        IPort LandscapePort;
        IPort DensityPort;
        IPort InstancesPort;
        INodeOption SeedOpt, AlignToNormalOpt, MinTiltOpt, MaxTiltOpt, ObjectOpt;

        public override EnvCompiledFunction compile(Dictionary<IPort, int> variables)
        {
            return new DistributeCompiled(
                seed: UInt(SeedOpt,4645785),
                alignToNormal: Bool(AlignToNormalOpt, true),
                minTilt: Float(MinTiltOpt, 0),
                maxTilt: Float(MaxTiltOpt, 0),
                Object: Val<InstanceableObject>(ObjectOpt,null),
                inputLandscapeArg: variables.GetValueOrDefault(LandscapePort, -1),
                inputDensityArg: variables.GetValueOrDefault(DensityPort, -1),
                outputInstancesArg:variables.GetValueOrDefault(InstancesPort, -1)
            );
        }

        protected override void OnDefineOptions(IOptionDefinitionContext ctx)
        {
            SeedOpt = ctx.AddOption<uint>("Seed").WithDefaultValue(4645785).Build();
            ObjectOpt = ctx.AddOption<InstanceableObject>("Object").WithDefaultValue(null).Build();
            AlignToNormalOpt = ctx.AddOption<bool>("AlignToNormal").WithDefaultValue(true).Build();
            MinTiltOpt = ctx.AddOption<float>("MinTilt").WithDefaultValue(0).Build();
            MaxTiltOpt = ctx.AddOption<float>("MaxTilt").WithDefaultValue(0).Build();
        }
        protected override void OnDefinePorts(IPortDefinitionContext ctx)
        {
            LandscapePort = ctx.AddInputPort<Mesh>("Landscape").Build();
            DensityPort = ctx.AddInputPort<Vector3>("Density").Build();
            InstancesPort = ctx.AddOutputPort<Transform>("Instances").Build();
        }
    }


    [Serializable]
    public class JoinInstances : EnvNode
    {
        IPort InstancesPort,MoreInstancesPort,JoinedInstancesPort;

        public override EnvCompiledFunction compile(Dictionary<IPort, int> variables)
        {
            return new JoinInstancesCompiled(
                aArg: variables.GetValueOrDefault(InstancesPort,-1),
                bArg: variables.GetValueOrDefault(MoreInstancesPort, -1),
                outArg: variables.GetValueOrDefault(JoinedInstancesPort, -1)
                );
        }

        protected override void OnDefineOptions(IOptionDefinitionContext ctx)
        {
            
        }
        protected override void OnDefinePorts(IPortDefinitionContext ctx)
        {
            InstancesPort = ctx.AddInputPort<Transform>("Instances").Build();
            MoreInstancesPort = ctx.AddInputPort<Transform>("More Instances").Build();
            JoinedInstancesPort = ctx.AddOutputPort<Transform>("Joined Instances").Build();
        }
    }

    [Serializable]
    public class SetInstancedObject : EnvNode
    {
        public override EnvCompiledFunction compile(Dictionary<IPort, int> variables)
        {
            throw new NotImplementedException();
        }

        protected override void OnDefineOptions(IOptionDefinitionContext ctx)
        {
            //ctx.AddOption<Vector2Int>("Resolution");
        }
        protected override void OnDefinePorts(IPortDefinitionContext ctx)
        {
            ctx.AddInputPort<Transform>("Instances").Build();
            ctx.AddInputPort<InstanceableObject>("Object").WithDefaultValue(null).Build();
        }
    }

    [Serializable]
    public class Return : EnvNode
    {
        internal IPort InstancesPort;
        internal IPort LandscapePort;
        public override EnvCompiledFunction compile(Dictionary<IPort, int> variables)
        {
            return null;
        }

        protected override void OnDefineOptions(IOptionDefinitionContext ctx)
        {
            //ctx.AddOption<Vector2Int>("Resolution");
        }
        protected override void OnDefinePorts(IPortDefinitionContext ctx)
        {
            InstancesPort = ctx.AddInputPort<Transform>("Instances").Build();
            LandscapePort = ctx.AddInputPort<Mesh>("Landscape").Build();
        }
    }


    /*
      if (world.spawnGrass)
        {
            var grassPoints = mesh.distributePoints((vert, loop) => 1f, world.seed);
            RandomNumberGenerator rng = new RandomNumberGenerator(world.grassSeed);
            Vector3[] verts = new Vector3[grassPoints.Count * 4];
            int[] indices = new int[grassPoints.Count * 6];
            //grassVertices = new GraphicsBuffer(GraphicsBuffer.Target.Structured, grassPoints.Count, 3 * sizeof(float));
            //Array.
            // grassVertices.SetData
            float grassMaxTiltRadians = Mathf.Deg2Rad * world.grassMaxTilt;
            for (int j = 0; j < grassPoints.Count; j++)
            {
                Tuple<float3, float3> t = grassPoints[j];
                float width = rng.get_float_in(world.grassMinWidth, world.grassMaxWidth);
                float height = rng.get_float_in(world.grassMinWidth, world.grassMaxHeight);
                float rotationAboutY = rng.get_float_in(0, Mathf.PI * 2f);

                float rotationAboutX = rng.get_float_in(-grassMaxTiltRadians, grassMaxTiltRadians);
                float3 normal = t.Item2;
                float2 yawPitch = BMath.normalToYawPitch(normal);
                float tiltedZ = -height * Mathf.Sin(rotationAboutX);
                float tiltedY = height * Mathf.Cos(rotationAboutX);
                float3[] quadVerts = new float3[] {
                    new float3(-0.5f*width,0, 0),
                    new float3(0.5f*width, 0, 0) ,
                    //new float3(0.5f*width, height, 0),
                    //new float3(-0.5f*width, height, 0),
                    new float3(0.5f*width, tiltedY, tiltedZ),
                    new float3(-0.5f*width, tiltedY, tiltedZ)
                };

                //Quaternion rotateAboutXY = Quaternion.EulerAngles(rotationAboutX, rotationAboutY, 0);
                //Quaternion rotate = alignToNormal * rotateAboutXY;
                float3x3 rotationMatrix = BMath.rotationByEulerYXY(rotationAboutY, yawPitch.x, yawPitch.y);
                float3 normal_ = math.mul(rotationMatrix, Vector3.up);
                int jVertsOffset = j * 4;
                for (int i = 0; i < quadVerts.Length; i++)
                {
                    float3 rotated = math.mul(rotationMatrix, quadVerts[i]);
                    float3 translated = t.Item1 + rotated;
                    verts[jVertsOffset + i] = translated;
                }

                indices[j * 6 + 0] = jVertsOffset + 0;
                indices[j * 6 + 1] = jVertsOffset + 1;
                indices[j * 6 + 2] = jVertsOffset + 2;
                indices[j * 6 + 3] = jVertsOffset + 0;
                indices[j * 6 + 4] = jVertsOffset + 2;
                indices[j * 6 + 5] = jVertsOffset + 3;


            }
            //grassEulerAngles = new GraphicsBuffer(GraphicsBuffer.Target.Structured, vertsNormals.Count, 3 * sizeof(float));
            //meshNormals.SetData(mesh.triangles);

            Mesh grassMesh = new Mesh();
            
            grassMesh.vertices = verts;
            grassMesh.triangles = indices;
            grassMeshFilter.mesh = grassMesh;
            
        }
     
     */
}
