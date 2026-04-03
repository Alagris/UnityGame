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
        internal static Vector3 Float3(INodeOption opt, Vector3 dflt)
        {
            return Val(opt, dflt);
        }
        internal static Vector2 Float2(INodeOption opt, Vector2 dflt)
        {
            return Val(opt, dflt);
        }
    }


    [Serializable]
    public abstract class AnyNoise : EnvNode
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
    public class ErosionNoise : AnyNoise
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
    public class PerlinFbmNoise : AnyNoise
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
    public class PerlinNoise : AnyNoise
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
    public class Linear : AnyNoise
    {
        INodeOption ScaleOpt;
        INodeOption HeightOpt;

        public override EnvCompiledFunction compile(int xArg, int zArg, int outGrad)
        {
            return new LinearCompiled(
                scale: Float2(ScaleOpt, new Vector2(1,1)),
                height: Float2(HeightOpt, new Vector2(0,0)),
                positionXArg: xArg,
                positionZArg: zArg,
                outputGradientsArg: outGrad
           );
        }
        protected override void OnDefineOptions(IOptionDefinitionContext ctx)
        {
            ScaleOpt = ctx.AddOption<Vector2>("Scale").WithDefaultValue(new Vector2(1, 1)).Build();
            HeightOpt = ctx.AddOption<Vector2>("Height").WithDefaultValue(new Vector2(0,0)).Build();
        }

    }
    /*
    [Serializable]
    public class Split : EnvNode
    {
        IPort gPort;
        IPort xPort;
        IPort zPort;
        
        protected override void OnDefinePorts(IPortDefinitionContext ctx)
        {
            xPort = ctx.AddOutputPort<Vector3>("X").Build();
            zPort = ctx.AddOutputPort<Vector3>("Z").Build();
            gPort = ctx.AddInputPort<Vector3>("Gradient").Build();
        }
        
        public override EnvCompiledFunction compile(Dictionary<IPort, int> variables)
        {
            return new SplitCompiled(variables.GetValueOrDefault(gPort, -1), variables.GetValueOrDefault(xPort, -1), variables.GetValueOrDefault(zPort, -1));
        }

    }
    */
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
    public class LoadStaticMesh : EnvNode
    {
        IPort ObjectPort;
        INodeOption OverrideMaterialsOpt, ObjectOpt;
        
            
        public override EnvCompiledFunction compile(Dictionary<IPort, int> variables)
        {
            MeshRenderer mesh = Val<MeshRenderer>(ObjectOpt, null);
            List<Material> mats = Val<List<Material>>(OverrideMaterialsOpt, null);
            int objArg = variables.GetValueOrDefault(ObjectPort, -1);
            return new LoadStaticMeshCompiled(objArg, mats, mesh);
        }
        protected override void OnDefineOptions(IOptionDefinitionContext ctx)
        {
            ObjectOpt = ctx.AddOption<MeshRenderer>("Object").WithDefaultValue(null).Build();
            OverrideMaterialsOpt = ctx.AddOption<List<Material>>("OverrideMaterials").Build();
        }
        protected override void OnDefinePorts(IPortDefinitionContext ctx)
        {
            ObjectPort = ctx.AddOutputPort<InstanceableObject>("Object").Build();
        }
    }
   
    [Serializable]
    public class LoadLODGroup : EnvNode
    {
        IPort ObjectPort;
        INodeOption OverrideMaterialsOpt, ObjectOpt;


        public override EnvCompiledFunction compile(Dictionary<IPort, int> variables)
        {
            LODGroup mesh = Val<LODGroup>(ObjectOpt, null);
            List<Material> mats = Val<List<Material>>(OverrideMaterialsOpt, null);
            int objArg = variables.GetValueOrDefault(ObjectPort, -1);
            return new LoadLODGroupCompiled(objArg, mats, mesh);
        }
        protected override void OnDefineOptions(IOptionDefinitionContext ctx)
        {
            ObjectOpt = ctx.AddOption<LODGroup>("Object").WithDefaultValue(null).Build();
            OverrideMaterialsOpt = ctx.AddOption<List<Material>>("OverrideMaterials").Build();
        }
        protected override void OnDefinePorts(IPortDefinitionContext ctx)
        {
            ObjectPort = ctx.AddOutputPort<InstanceableObject>("Object").Build();
        }
    }

    [Serializable]
    public class LoadInstanceableObjectAsset : EnvNode
    {
        IPort ObjectPort;
        INodeOption ObjectOpt;


        public override EnvCompiledFunction compile(Dictionary<IPort, int> variables)
        {
            InstanceableObjectAsset mesh = Val<InstanceableObjectAsset>(ObjectOpt, null);
            
            int objArg = variables.GetValueOrDefault(ObjectPort, -1);
            return new LoadInstanceableObjectAssetCompiled(objArg, mesh);
        }
        protected override void OnDefineOptions(IOptionDefinitionContext ctx)
        {
            ObjectOpt = ctx.AddOption<InstanceableObjectAsset>("Asset").WithDefaultValue(null).Build();
        }
        protected override void OnDefinePorts(IPortDefinitionContext ctx)
        {
            ObjectPort = ctx.AddOutputPort<InstanceableObject>("Object").Build();
        }
    }
    [Serializable]
    public class Distribute : EnvNode
    {
        IPort LandscapePort;
        IPort DensityPort;
        IPort InstancesPort;
        IPort ObjectPort;
        INodeOption SeedOpt, AlignToNormalOpt, MinTiltOpt, MaxTiltOpt, MinScaleOpt, MaxScaleOpt, ScaleUniformlyOpt;

        public override EnvCompiledFunction compile(Dictionary<IPort, int> variables)
        {
            int inputDensityArg = variables.GetValueOrDefault(DensityPort, -1);
            uint seed = UInt(SeedOpt, 4645785);
            bool alignToNormal = Bool(AlignToNormalOpt, true);
            float minTilt = Mathf.Deg2Rad * Float(MinTiltOpt, 0);
            float maxTilt = Mathf.Deg2Rad * Float(MaxTiltOpt, 0);
            bool scaleUniformly = Bool(ScaleUniformlyOpt, true);
            Vector3 minScale = Float3(MinScaleOpt, new Vector3(1,1,1));
            Vector3 maxScale = Float3(MaxScaleOpt, new Vector3(1, 1, 1));
            int objectArg = variables.GetValueOrDefault(ObjectPort, -1); 
            int inputLandscapeArg = variables.GetValueOrDefault(LandscapePort, -1);
            int outputInstancesArg = variables.GetValueOrDefault(InstancesPort, -1);
            if (inputDensityArg == -1)
            {
                Vector3 uniformDensity = new Vector3(0, 0, 0);
                DensityPort.TryGetValue(out uniformDensity);
                return new DistributeUniformCompiled(
                    seed: seed,
                    alignToNormal: alignToNormal,
                    minTilt: minTilt,
                    maxTilt: maxTilt,
                    minScale: minScale,
                    maxScale: maxScale,
                    scaleUniformly: scaleUniformly,
                    objectArg: objectArg,
                    inputLandscapeArg: inputLandscapeArg,
                    desnityUniform: uniformDensity.magnitude,
                    outputInstancesArg: outputInstancesArg
                );
            }
            else
            {
                return new DistributeCompiled(
                    seed: seed,
                    alignToNormal: alignToNormal,
                    minTilt: minTilt,
                    maxTilt: maxTilt,
                    minScale: minScale,
                    maxScale: maxScale,
                    scaleUniformly: scaleUniformly,
                    objectArg: objectArg,
                    inputLandscapeArg: inputLandscapeArg,
                    inputDensityArg: inputDensityArg,
                    outputInstancesArg: outputInstancesArg
                );
            }
            
        }

        protected override void OnDefineOptions(IOptionDefinitionContext ctx)
        {
            SeedOpt = ctx.AddOption<uint>("Seed").WithDefaultValue(4645785).Build();
            
            AlignToNormalOpt = ctx.AddOption<bool>("AlignToNormal").WithDefaultValue(true).Build();
            MinTiltOpt = ctx.AddOption<float>("MinTilt").WithDefaultValue(0).Build();
            MaxTiltOpt = ctx.AddOption<float>("MaxTilt").WithDefaultValue(0).Build();
            ScaleUniformlyOpt = ctx.AddOption<bool>("Scale Uniformly").WithDefaultValue(true).Build();
            MinScaleOpt = ctx.AddOption<Vector3>("MinScale").WithDefaultValue(new Vector3(1,1,1)).Build();
            MaxScaleOpt = ctx.AddOption<Vector3>("MaxScale").WithDefaultValue(new Vector3(1, 1, 1)).Build();
        }
        protected override void OnDefinePorts(IPortDefinitionContext ctx)
        {
            ObjectPort = ctx.AddInputPort<InstanceableObject>("Object").Build();
            LandscapePort = ctx.AddInputPort<Mesh>("Landscape").Build();
            DensityPort = ctx.AddInputPort<Vector3>("Density").Build();
            InstancesPort = ctx.AddOutputPort<Transform>("Instances").Build();
            
        }
    }

    [Serializable]
    public class OverrideMaterials : EnvNode
    {
        IPort InstancesPort, OverridenInstancesPort;
        INodeOption MaterialsOpt;
        public override EnvCompiledFunction compile(Dictionary<IPort, int> variables)
        {
            List<Material> mats;
            MaterialsOpt.TryGetValue(out mats);
            return new OverrideMaterialsCompiled(
                inArg: variables.GetValueOrDefault(InstancesPort, -1),
                outArg: variables.GetValueOrDefault(OverridenInstancesPort, -1),
                mats: mats
            );
        }

        protected override void OnDefineOptions(IOptionDefinitionContext ctx)
        {
            MaterialsOpt = ctx.AddOption<List<Material>>("Materials").Build();
        }
        protected override void OnDefinePorts(IPortDefinitionContext ctx)
        {
            InstancesPort = ctx.AddInputPort<Transform>("Instances").Build();
            OverridenInstancesPort = ctx.AddInputPort<Transform>("Instances").Build();
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
            ctx.AddInputPort<MeshRenderer>("Object").WithDefaultValue(null).Build();
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
