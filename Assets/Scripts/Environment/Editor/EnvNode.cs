
using Env.Runtime;
using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;
using Unity.Mathematics;
using UnityEngine;

namespace Env.Editor
{
    [Serializable]
    public abstract class EnvNode : Node
    {
        public abstract EnvCompiledFunction compile(EnvCompiledGraph compiledGraph, Dictionary<IPort, int> variables);

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



        public int readIntArrays(EnvCompiledGraph g, Dictionary<IPort, int> variables, IPort port, bool mutable)
        {
            Debug.Assert(port.DataType == typeof(int));
            Debug.Assert(port.Direction == PortDirection.Input);
            int idx = variables.GetValueOrDefault(port, -1);
            if (idx >= 0)
            {
                g.intArraysCount[idx]++;
            }
            return idx;
        }
        public int readFloatArrays(EnvCompiledGraph g, Dictionary<IPort, int> variables, IPort port, bool mutable)
        {
            Debug.Assert(port.DataType == typeof(float));
            Debug.Assert(port.Direction == PortDirection.Input);
            int idx = variables.GetValueOrDefault(port, -1);
            if (idx >= 0)
            {
                g.floatArraysCount[idx]++;
            }
            return idx;
        }
        public int readInt2Arrays(EnvCompiledGraph g, Dictionary<IPort, int> variables, IPort port, bool mutable)
        {
            Debug.Assert(port.DataType == typeof(Vector2Int));
            Debug.Assert(port.Direction == PortDirection.Input);
            int idx = variables.GetValueOrDefault(port, -1);
            if (idx >= 0)
            {
                g.int2ArraysCount[idx]++;
            }
            return idx;
        }
        public int readFloat2Arrays(EnvCompiledGraph g, Dictionary<IPort, int> variables, IPort port, bool mutable)
        {
            Debug.Assert(port.DataType == typeof(Vector2));
            Debug.Assert(port.Direction == PortDirection.Input);
            int idx = variables.GetValueOrDefault(port, -1);
            if (idx >= 0)
            {
                g.float2ArraysCount[idx]++;
            }
            return idx;
        }
        public int readInt3Arrays(EnvCompiledGraph g, Dictionary<IPort, int> variables, IPort port, bool mutable)
        {
            Debug.Assert(port.DataType == typeof(Vector3Int));
            Debug.Assert(port.Direction == PortDirection.Input);
            int idx = variables.GetValueOrDefault(port, -1);
            if (idx >= 0)
            {
                g.int3ArraysCount[idx]++;
            }
            return idx;
        }
        public int readFloat3Arrays(EnvCompiledGraph g, Dictionary<IPort, int> variables, IPort port, bool mutable)
        {
            Debug.Assert(port.DataType == typeof(Vector3));
            Debug.Assert(port.Direction == PortDirection.Input);
            int idx = variables.GetValueOrDefault(port, -1);
            if (idx >= 0)
            {
                g.float3ArraysCount[idx]++;
            }
            return idx;
        }
        public int readInt4Arrays(EnvCompiledGraph g, Dictionary<IPort, int> variables, IPort port, bool mutable)
        {
            Debug.Assert(port.DataType == typeof(Vector3Int));
            Debug.Assert(port.Direction == PortDirection.Input);
            int idx = variables.GetValueOrDefault(port, -1);
            if (idx >= 0)
            {
                g.int4ArraysCount[idx]++;
            }
            return idx;
        }
        public int readFloat4Arrays(EnvCompiledGraph g, Dictionary<IPort, int> variables, IPort port, bool mutable)
        {
            Debug.Assert(port.DataType == typeof(Vector4));
            Debug.Assert(port.Direction == PortDirection.Input);
            int idx = variables.GetValueOrDefault(port, -1);
            if (idx >= 0)
            {
                g.float4ArraysCount[idx]++;
            }
            return idx;
        }
        public int readProcMeshes(EnvCompiledGraph g, Dictionary<IPort, int> variables, IPort port, bool mutable)
        {
            Debug.Assert(port.DataType==typeof(Mesh));
            Debug.Assert(port.Direction == PortDirection.Input);
            int idx = variables.GetValueOrDefault(port, -1);
            if (idx >= 0)
            {
                g.procMeshesCount[idx]++;
            }
            return idx;
        }
        public int readProcInstanceSets(EnvCompiledGraph g, Dictionary<IPort, int> variables, IPort port, bool mutable)
        {
            Debug.Assert(port.DataType == typeof(Transform));
            Debug.Assert(port.Direction == PortDirection.Input);
            int idx = variables.GetValueOrDefault(port, -1);
            if (idx >= 0)
            {
                g.procInstanceSetsCount[idx]++;
            }
            return idx;
        }
        public int readObject(EnvCompiledGraph g, Dictionary<IPort, int> variables, IPort port, bool mutable)
        {
            Debug.Assert(port.DataType == typeof(InstanceableObject));
            Debug.Assert(port.Direction == PortDirection.Input);
            int idx = variables.GetValueOrDefault(port, -1);
            if (idx >= 0)
            {
                g.objectCount[idx]++;
            }
            return idx;
        }
        public int readColor(EnvCompiledGraph g, Dictionary<IPort, int> variables, IPort port, bool mutable)
        {
            Debug.Assert(port.DataType == typeof(Color));
            Debug.Assert(port.Direction == PortDirection.Input);
            int idx = variables.GetValueOrDefault(port, -1);
            if (idx >= 0)
            {
                g.colorCount[idx]++;
            }
            return idx;
        }

        public int writeIntArrays(EnvCompiledGraph g, Dictionary<IPort, int> variables, IPort port) {
            Debug.Assert(port.DataType == typeof(int));
            Debug.Assert(port.Direction==PortDirection.Output);
            return variables.GetValueOrDefault(port, -1);
        }
        public int writeFloatArrays(EnvCompiledGraph g, Dictionary<IPort, int> variables, IPort port) {
            Debug.Assert(port.DataType == typeof(float));
            Debug.Assert(port.Direction==PortDirection.Output);
            return variables.GetValueOrDefault(port, -1);
        }
        public int writeInt2Arrays(EnvCompiledGraph g, Dictionary<IPort, int> variables, IPort port) {
            Debug.Assert(port.DataType == typeof(Vector2Int));
            Debug.Assert(port.Direction==PortDirection.Output);
            return variables.GetValueOrDefault(port, -1);
        }
        public int writeFloat2Arrays(EnvCompiledGraph g, Dictionary<IPort, int> variables, IPort port) {
            Debug.Assert(port.DataType == typeof(Vector2));
            Debug.Assert(port.Direction==PortDirection.Output);
            return variables.GetValueOrDefault(port, -1);
        }
        public int writeInt3Arrays(EnvCompiledGraph g, Dictionary<IPort, int> variables, IPort port) {
            Debug.Assert(port.DataType == typeof(Vector3Int));
            Debug.Assert(port.Direction==PortDirection.Output);
            return variables.GetValueOrDefault(port, -1);
        }
        public int writeFloat3Arrays(EnvCompiledGraph g, Dictionary<IPort, int> variables, IPort port) {
            Debug.Assert(port.DataType == typeof(Vector3));
            Debug.Assert(port.Direction==PortDirection.Output);
            return variables.GetValueOrDefault(port, -1);
        }
        public int writeInt4Arrays(EnvCompiledGraph g, Dictionary<IPort, int> variables, IPort port)
        {
            Debug.Assert(port.DataType == typeof(int4));
            Debug.Assert(port.Direction == PortDirection.Output);
            return variables.GetValueOrDefault(port, -1);
        }
        public int writeFloat4Arrays(EnvCompiledGraph g, Dictionary<IPort, int> variables, IPort port)
        {
            Debug.Assert(port.DataType == typeof(Vector4));
            Debug.Assert(port.Direction == PortDirection.Output);
            return variables.GetValueOrDefault(port, -1);
        }
        public int writeProcMeshes(EnvCompiledGraph g, Dictionary<IPort, int> variables, IPort port) {
            Debug.Assert(port.DataType == typeof(Mesh));
            Debug.Assert(port.Direction==PortDirection.Output);
            return variables.GetValueOrDefault(port, -1);
        }
        public int writeProcInstanceSets(EnvCompiledGraph g, Dictionary<IPort, int> variables, IPort port) {
            Debug.Assert(port.DataType == typeof(Transform));
            Debug.Assert(port.Direction==PortDirection.Output);
            return variables.GetValueOrDefault(port, -1);
        }
        public int writeObject(EnvCompiledGraph g, Dictionary<IPort, int> variables, IPort port) {
            Debug.Assert(port.DataType == typeof(InstanceableObject));
            Debug.Assert(port.Direction==PortDirection.Output);
            return variables.GetValueOrDefault(port, -1);
        }
        public int writeColor(EnvCompiledGraph g, Dictionary<IPort, int> variables, IPort port) {
            Debug.Assert(port.DataType == typeof(Color));
            Debug.Assert(port.Direction == PortDirection.Output);
            return variables.GetValueOrDefault(port, -1);
        }
        internal void Redirect(Dictionary<IPort, int> variables, IPort inPort, IPort outPort)
        {
            Debug.Assert(inPort.Direction == PortDirection.Input);
            Debug.Assert(outPort.Direction == PortDirection.Output);
            int inIdx = variables.GetValueOrDefault(inPort, -1);
            List<IPort> neighbours = new List<IPort>();
            outPort.GetConnectedPorts(neighbours);
            foreach (IPort i in neighbours)
            {
                variables.Add(i, inIdx);
            }
        }
        internal void Disconnect(Dictionary<IPort, int> variables, IPort outPort)
        {
            Debug.Assert(outPort.Direction == PortDirection.Output);
            List<IPort> neighbours = new List<IPort>();
            outPort.GetConnectedPorts(neighbours);
            foreach (IPort i in neighbours)
            {
                variables.Add(i, -1);
            }
        }
    }

    public enum NoiseMode
    {
        LANDSCAPE, INSTANCES
    }
    [Serializable]
    public abstract class AnyNoise : EnvNode
    {
        IPort xPort;
        IPort zPort;
        IPort instancesPort;
        IPort gPort;
        NoiseMode mode;
        protected virtual bool allowInstances() => true;
        protected override void OnDefineOptions(IOptionDefinitionContext ctx)
        {
            if (allowInstances()) {
                ctx.AddOption<NoiseMode>("Mode").WithDefaultValue(NoiseMode.LANDSCAPE).Delayed();
            }
        }
        protected override void OnDefinePorts(IPortDefinitionContext ctx)
        {
            mode = NoiseMode.LANDSCAPE;
            if (allowInstances())
            {
                GetNodeOptionByName("Mode").TryGetValue(out mode);
            }
            instancesPort = null;
            xPort = null;
            zPort = null;
            switch (mode) {
                case NoiseMode.LANDSCAPE:
                    xPort = ctx.AddInputPort<float>("X").Build();
                    zPort = ctx.AddInputPort<float>("Z").Build();
                    break;
                case NoiseMode.INSTANCES:
                    instancesPort = ctx.AddInputPort<Transform>("Instances").Build();
                    break;
            }
            gPort = ctx.AddOutputPort<float>("Height").Build();
        }
        public abstract EnvCompiledFunction compile(int xArg, int zArg, int outGrad);
        public abstract EnvCompiledFunction compileInstances(int instanceArg, int outGrad);
        public override EnvCompiledFunction compile(EnvCompiledGraph compiledGraph, Dictionary<IPort, int> variables)
        {
            switch (mode)
            {
                case NoiseMode.LANDSCAPE:
                    return compile(
                        xArg: readFloatArrays(compiledGraph, variables, xPort, false),
                        zArg: readFloatArrays(compiledGraph, variables, zPort, false),
                        outGrad: writeFloatArrays(compiledGraph, variables, gPort)
                    );
                case NoiseMode.INSTANCES:
                    return compileInstances(
                        instanceArg: readProcInstanceSets(compiledGraph, variables, instancesPort, false),
                        outGrad: writeFloatArrays(compiledGraph, variables, gPort)
                    );
                default:
                    return null;
            }
            
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

        public override EnvCompiledFunction compileInstances(int instanceArg, int outGrad)
        {
            return new ErosionNoiseInstanceCompiled(
                scale: Float(ScaleOpt, 1),
                pointiness: Float(PointinessOpt, 0.5f),
                scalingPowerBase: Float(ScalingPowerBaseOpt, 3),
                iterations: Int(IterationsOpt, 1),
                height: Float(HeightOpt, 30),
                instanceArg: instanceArg,
                outputGradientsArg: outGrad
           );
        }

        protected override void OnDefineOptions(IOptionDefinitionContext ctx)
        {
            base.OnDefineOptions(ctx);
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

        public override EnvCompiledFunction compileInstances(int instanceArg, int outGrad)
        {
            return new PerlinFbmInstanceCompiled(
                   scale: Float(ScaleOpt, 1),
                   heightPowerBase: Float(HeightPowerBaseOpt, 0.5f),
                   scalePowerBase: Float(ScalePowerBaseOpt, 0.5f),
                   iterations: Int(IterationsOpt, 1),
                   height: Float(HeightOpt, 30),
                   instanceArg: instanceArg,
                   outputGradientsArg: outGrad
              );
        }

        protected override void OnDefineOptions(IOptionDefinitionContext ctx)
        {
            base.OnDefineOptions(ctx);
            IterationsOpt = ctx.AddOption<int>("Iterations").WithDefaultValue(3).Build();
            ScaleOpt = ctx.AddOption<float>("Scale").WithDefaultValue(0.02f).Build();
            HeightPowerBaseOpt = ctx.AddOption<float>("HeightPowerBase").WithDefaultValue(0.5f).Build();
            ScalePowerBaseOpt = ctx.AddOption<float>("ScalePowerBase").WithDefaultValue(3f).Build();
            HeightOpt = ctx.AddOption<float>("Height").WithDefaultValue(5).Build();
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

        public override EnvCompiledFunction compileInstances(int instanceArg, int outGrad)
        {
            return new PerlinInstanceCompiled(
               scale: Float(ScaleOpt, 1),
               height: Float(HeightOpt, 30),
               instanceArg: instanceArg,
               outputGradientsArg: outGrad
          );
        }

        protected override void OnDefineOptions(IOptionDefinitionContext ctx)
        {
            base.OnDefineOptions(ctx);
            ScaleOpt = ctx.AddOption<float>("Scale").WithDefaultValue(1).Build();
            HeightOpt = ctx.AddOption<float>("Height").WithDefaultValue(30).Build();
        }
        
    }



    [Serializable]
    public class WhiteNoise : AnyNoise
    {
        INodeOption MinOpt;
        INodeOption MaxOpt;

        public override EnvCompiledFunction compile(int xArg, int zArg, int outGrad)
        {
            return new WhiteNoiseCompiled(
                min: Float(MinOpt, 0),
                max: Float(MaxOpt, 1),
                positionXArg: xArg,
                positionZArg: zArg,
                outputGradientsArg: outGrad
           );
        }

        public override EnvCompiledFunction compileInstances(int instanceArg, int outGrad)
        {
            return new WhiteNoiseInstanceCompiled(
                min: Float(MinOpt, 0),
                max: Float(MaxOpt, 1),
                instanceArg: instanceArg,
                outputGradientsArg: outGrad
           );
        }

        protected override void OnDefineOptions(IOptionDefinitionContext ctx)
        {
            base.OnDefineOptions(ctx);
            MinOpt = ctx.AddOption<float>("Min").WithDefaultValue(0).Build();
            MaxOpt = ctx.AddOption<float>("Max").WithDefaultValue(1).Build();
        }

    }
    [Serializable]
    public class Linear : AnyNoise
    {
        INodeOption ScaleOpt;
        INodeOption HeightOpt;
        protected override bool allowInstances()
        {
            return false;
        }
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
            base.OnDefineOptions(ctx);
            ScaleOpt = ctx.AddOption<Vector2>("Scale").WithDefaultValue(new Vector2(1, 1)).Build();
            HeightOpt = ctx.AddOption<Vector2>("Height").WithDefaultValue(new Vector2(0,0)).Build();
        }

        public override EnvCompiledFunction compileInstances(int instanceArg, int outGrad)
        {
            throw new NotImplementedException();
        }
    }
    
    [Serializable]
    public abstract class AbstractBinary : EnvNode
    {
        IPort aPort;
        IPort bPort;
        IPort cPort;
        
        protected override void OnDefinePorts(IPortDefinitionContext ctx)
        {
            aPort = ctx.AddInputPort<float>("A").Build();
            bPort = ctx.AddInputPort<float>("B").Build();
            cPort = ctx.AddOutputPort<float>("C").Build();
        }
        
        public override EnvCompiledFunction compile(EnvCompiledGraph compiledGraph, Dictionary<IPort, int> variables)
        {
            int a = readFloatArrays(compiledGraph, variables, aPort, false);
            int b = readFloatArrays(compiledGraph, variables, bPort, false);
            if (a == -1)
            {
                if(b == -1)
                {
                    Disconnect(variables, cPort);
                    return null;
                }
                else
                {
                    Redirect(variables, bPort, cPort);
                    return null;
                }

            }
            else
            {
                if (b == -1)
                {
                    Redirect(variables, aPort, cPort);
                    return null;
                }
                else
                {
                    return compile(variables, a, b, writeFloatArrays(compiledGraph, variables, cPort));
                }
            }
            
        }

        protected abstract EnvCompiledFunction compile(Dictionary<IPort, int> variables, int a, int b, int output);
    }

    [Serializable]
    public class Multiply : AbstractBinary
    {

        protected override EnvCompiledFunction compile(Dictionary<IPort, int> variables, int a, int b, int output)
        {
            return new MultiplyCompiled(a,b,output);
        }

    }

    [Serializable]
    public class Add : AbstractBinary
    {

        protected override EnvCompiledFunction compile(Dictionary<IPort, int> variables, int a, int b, int output)
        {
            return new AddCompiled(a, b, output);
        }

    }
    [Serializable]
    public class Subtract : AbstractBinary
    {

        protected override EnvCompiledFunction compile(Dictionary<IPort, int> variables, int a, int b, int output)
        {
            return new SubCompiled(a, b, output);
        }

    }
    [Serializable]
    public class LandscapeMesh : EnvNode
    {
        
        IPort HeightPort;
        IPort LandscapePort;
        INodeOption ShadeFlatOpt, UV_Mode;
        INodeOption UVScalingOpt;
        public override EnvCompiledFunction compile(EnvCompiledGraph compiledGraph, Dictionary<IPort, int> variables)
        {
            
            return new LandscapeCompiled(
                shadeFlat:Bool(ShadeFlatOpt, false),
                uvScaling:Float(UVScalingOpt, 1),
                uvMode: Val(UV_Mode, UVMode.GLOBAL),
                heightArg: readFloatArrays(compiledGraph, variables, HeightPort, false),
                outputLandscapeArg: writeProcMeshes(compiledGraph, variables, LandscapePort)
                
                );
        }
        
        protected override void OnDefineOptions(IOptionDefinitionContext ctx)
        {
            ShadeFlatOpt = ctx.AddOption<bool>("ShadeFlat").WithDefaultValue(false).Build();
            UV_Mode = ctx.AddOption<UVMode>("UV Mode").WithDefaultValue(UVMode.GLOBAL).Build();
            UVScalingOpt = ctx.AddOption<float>("UV Scaling").WithDefaultValue(1).Build();
        }
        protected override void OnDefinePorts(IPortDefinitionContext ctx)
        {
            HeightPort = ctx.AddInputPort<float>("Height").Build();
            LandscapePort = ctx.AddOutputPort<Mesh>("Landscape").Build();
            
        }
    }

    [Serializable]
    public class Normalize : EnvNode
    {
        List<IPort> InputPorts = new List<IPort>();
        List<IPort> OutputPorts = new List<IPort>();
        public override EnvCompiledFunction compile(EnvCompiledGraph compiledGraph, Dictionary<IPort, int> variables)
        {
            
            int[] inputs = new int[InputPorts.Count];
            int[] outputs = new int[OutputPorts.Count];
            for (int i=0;i< inputs.Length;i++)
            {
                inputs[i] = readFloatArrays(compiledGraph, variables, InputPorts[i], true);
            }
            for (int i = 0; i < outputs.Length; i++)
            {
                outputs[i] = writeFloatArrays(compiledGraph, variables, OutputPorts[i]);
            }
            return new NormalizeCompiled(outputs, inputs);
        }

        protected override void OnDefineOptions(IOptionDefinitionContext ctx)
        {
            
            ctx.AddOption<int>("Count").WithDefaultValue(2).Delayed();
        }
        protected override void OnDefinePorts(IPortDefinitionContext ctx)
        {
            int layerCount=0;
            GetNodeOptionByName("Count").TryGetValue<int>(out layerCount);
            InputPorts.Clear();
            OutputPorts.Clear();
            for (int i = 1; i <= layerCount; i++) {
                
                IPort inPort = ctx.AddInputPort<float>("Input "+i).Build();
                IPort outPort = ctx.AddOutputPort<float>("Output "+i).Build();
                InputPorts.Add(inPort);
                OutputPorts.Add(outPort);
            }


        }
    }
    [Serializable]
    public class LoadStaticMesh : EnvNode
    {
        IPort ObjectPort;
        INodeOption OverrideMaterialsOpt, ObjectOpt;
        
            
        public override EnvCompiledFunction compile(EnvCompiledGraph compiledGraph, Dictionary<IPort, int> variables)
        {
            MeshRenderer mesh = Val<MeshRenderer>(ObjectOpt, null);
            List<Material> mats = Val<List<Material>>(OverrideMaterialsOpt, null);
            int objArg = writeObject(compiledGraph, variables, ObjectPort);
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


        public override EnvCompiledFunction compile(EnvCompiledGraph compiledGraph, Dictionary<IPort, int> variables)
        {
            LODGroup mesh = Val<LODGroup>(ObjectOpt, null);
            List<Material> mats = Val<List<Material>>(OverrideMaterialsOpt, null);
            int objArg = writeObject(compiledGraph, variables, ObjectPort);
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


        public override EnvCompiledFunction compile(EnvCompiledGraph compiledGraph, Dictionary<IPort, int> variables)
        {
            InstanceableObjectAsset mesh = Val<InstanceableObjectAsset>(ObjectOpt, null);
            
            int objArg = writeObject(compiledGraph, variables, ObjectPort);
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

        public override EnvCompiledFunction compile(EnvCompiledGraph compiledGraph, Dictionary<IPort, int> variables)
        {
            int inputDensityArg = readFloatArrays(compiledGraph, variables, DensityPort, false);
            uint seed = (uint)Int(SeedOpt, 4645785);
            bool alignToNormal = Bool(AlignToNormalOpt, true);
            float minTilt = Mathf.Deg2Rad * Float(MinTiltOpt, 0);
            float maxTilt = Mathf.Deg2Rad * Float(MaxTiltOpt, 0);
            bool scaleUniformly = Bool(ScaleUniformlyOpt, true);
            Vector3 minScale = Float3(MinScaleOpt, new Vector3(1,1,1));
            Vector3 maxScale = Float3(MaxScaleOpt, new Vector3(1, 1, 1));
            int objectArg = readObject(compiledGraph, variables, ObjectPort, false); 
            int inputLandscapeArg = readProcMeshes(compiledGraph, variables, LandscapePort, false);
            int outputInstancesArg = writeProcInstanceSets(compiledGraph, variables, InstancesPort);
            if (inputDensityArg == -1)
            {
                float uniformDensity = 0;
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
                    desnityUniform: uniformDensity,
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
            SeedOpt = ctx.AddOption<int>("Seed").WithDefaultValue(4645785).Build();
            
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
            DensityPort = ctx.AddInputPort<float>("Density").Build();
            InstancesPort = ctx.AddOutputPort<Transform>("Instances").Build();
            
        }
    }


    [Serializable]
    public class SamplePoints : EnvNode
    {
        IPort InstancesPort, InAttrPort, OutAttrPort;


        public override EnvCompiledFunction compile(EnvCompiledGraph compiledGraph, Dictionary<IPort, int> variables)
        {
            if (GetNodeOptionByName("Type").TryGetValue(out AttributeType attrType))
            {
                int inIdx = readProcInstanceSets(compiledGraph, variables, InstancesPort, false);
                int attrIdx = -1;
                int outAttrIdx = -1;
                switch (attrType)
                {
                    case AttributeType.FLOAT:
                        attrIdx = readFloatArrays(compiledGraph, variables, InAttrPort, false);
                        outAttrIdx = writeFloatArrays(compiledGraph, variables, OutAttrPort);
                        return new SamplePointsCompiledFloat(inIdx, attrIdx, outAttrIdx);
                    case AttributeType.FLOAT2:
                        attrIdx = readFloat2Arrays(compiledGraph, variables, InAttrPort, false);
                        outAttrIdx = writeFloat2Arrays(compiledGraph, variables, OutAttrPort);
                        return new SamplePointsCompiledFloat2(inIdx, attrIdx, outAttrIdx);
                    case AttributeType.FLOAT3:
                        attrIdx = readFloat3Arrays(compiledGraph, variables, InAttrPort, false);
                        outAttrIdx = writeFloat3Arrays(compiledGraph, variables, OutAttrPort);
                        return new SamplePointsCompiledFloat3(inIdx, attrIdx, outAttrIdx);
                    case AttributeType.FLOAT4:
                        attrIdx = readFloat4Arrays(compiledGraph, variables, InAttrPort, false);
                        outAttrIdx = writeFloat4Arrays(compiledGraph, variables, OutAttrPort);
                        return new SamplePointsCompiledFloat4(inIdx, attrIdx, outAttrIdx);
                    case AttributeType.INT:
                    case AttributeType.UINT:
                        attrIdx = readIntArrays(compiledGraph, variables, InAttrPort, false);
                        outAttrIdx = writeIntArrays(compiledGraph, variables, OutAttrPort);
                        return new SamplePointsCompiledInt(inIdx, attrIdx, outAttrIdx);
                    case AttributeType.UINT2:
                    case AttributeType.INT2:
                        attrIdx = readInt2Arrays(compiledGraph, variables, InAttrPort, false);
                        outAttrIdx = writeInt2Arrays(compiledGraph, variables, OutAttrPort);
                        return new SamplePointsCompiledInt2(inIdx, attrIdx, outAttrIdx);
                    case AttributeType.UINT3:
                    case AttributeType.INT3:
                        attrIdx = readInt3Arrays(compiledGraph, variables, InAttrPort, false);
                        outAttrIdx = writeInt3Arrays(compiledGraph, variables, OutAttrPort);
                        return new SamplePointsCompiledInt3(inIdx, attrIdx, outAttrIdx);
                    case AttributeType.UINT4:
                    case AttributeType.INT4:
                        attrIdx = readInt4Arrays(compiledGraph, variables, InAttrPort, false);
                        outAttrIdx = writeInt4Arrays(compiledGraph, variables, OutAttrPort);
                        return new SamplePointsCompiledInt4(inIdx, attrIdx, outAttrIdx);
                }
                
            }
            return null;
        }
        protected override void OnDefineOptions(IOptionDefinitionContext ctx)
        {
            ctx.AddOption<AttributeType>("Type").WithDefaultValue(AttributeType.FLOAT).Delayed();
            
        }
        private void AddPorts<T>(IPortDefinitionContext ctx)
        {
            const string inName = "Landscape Data";
            const string outName = "Instance Data";
            InAttrPort = ctx.AddInputPort<T>(inName).Build();
            OutAttrPort = ctx.AddOutputPort<T>(outName).Build();
        }
        protected override void OnDefinePorts(IPortDefinitionContext ctx)
        {

            if (GetNodeOptionByName("Type").TryGetValue(out AttributeType attrType))
            {
                InstancesPort = ctx.AddInputPort<Transform>("Instances").Build();
                
                switch (attrType)
                {
                    
                    case AttributeType.FLOAT:
                        AddPorts<float>(ctx);
                        break;
                    case AttributeType.FLOAT2:
                        AddPorts<Vector2>(ctx);
                        break;
                    case AttributeType.FLOAT3:
                        AddPorts<Vector3>(ctx);
                        break;
                    case AttributeType.FLOAT4:
                        AddPorts<Vector4>(ctx);
                        break;
                    case AttributeType.UINT:
                    case AttributeType.INT:
                        AddPorts<int>(ctx);
                        break;
                    case AttributeType.UINT2:
                    case AttributeType.INT2:
                        AddPorts<Vector2Int>(ctx);
                        break;
                    case AttributeType.UINT3:
                    case AttributeType.INT3:
                        AddPorts<Vector3Int>(ctx);
                        break;
                }

                
            }

        }
    }
    [Serializable]
    public class SetAttribute : EnvNode
    {
        IPort InInstancesPort, OutInstancesPort;
        INodeOption CountOpt;


        public override EnvCompiledFunction compile(EnvCompiledGraph compiledGraph, Dictionary<IPort, int> variables)
        {
            CountOpt.TryGetValue(out int count);
            for (int i = 0; i < count; i++)
            {

                if (GetNodeOptionByName("Type " + (i + 1)).TryGetValue(out AttributeType attrType))
                {
                    int inIdx = readProcInstanceSets(compiledGraph, variables, InInstancesPort, true);
                    int outIdx = writeProcInstanceSets(compiledGraph, variables, OutInstancesPort);
                    int attrIdx = -1;
                    GetNodeOptionByName("Name " + (i + 1)).TryGetValue(out string attrName) ;
                    IPort AttrPort = GetInputPortByName("Attribute " + (i + 1));
                    switch (attrType)
                    {
                        case AttributeType.FLOAT:
                            attrIdx = readFloatArrays(compiledGraph, variables, AttrPort, false);
                            return new SetAttributeCompiledFloat(attrName, inIdx, attrIdx, outIdx);
                        case AttributeType.FLOAT2:
                            attrIdx = readFloat2Arrays(compiledGraph, variables, AttrPort, false);
                            return new SetAttributeCompiledFloat2(attrName, inIdx, attrIdx, outIdx);
                        case AttributeType.FLOAT3:
                            attrIdx = readFloat3Arrays(compiledGraph, variables, AttrPort, false);
                            return new SetAttributeCompiledFloat3(attrName, inIdx, attrIdx, outIdx);
                        case AttributeType.FLOAT4:
                            attrIdx = readFloat4Arrays(compiledGraph, variables, AttrPort, false);
                            return new SetAttributeCompiledFloat4(attrName, inIdx, attrIdx, outIdx);
                        case AttributeType.INT:
                            attrIdx = readIntArrays(compiledGraph, variables, AttrPort, false);
                            return new SetAttributeCompiledInt(attrName, inIdx, attrIdx, outIdx);
                        case AttributeType.INT2:
                            attrIdx = readInt2Arrays(compiledGraph, variables, AttrPort, false);
                            return new SetAttributeCompiledInt2(attrName, inIdx, attrIdx, outIdx);
                        case AttributeType.INT3:
                            attrIdx = readInt3Arrays(compiledGraph, variables, AttrPort, false);
                            return new SetAttributeCompiledInt3(attrName, inIdx, attrIdx, outIdx);
                        case AttributeType.INT4:
                            attrIdx = readInt4Arrays(compiledGraph, variables, AttrPort, false);
                            return new SetAttributeCompiledInt4(attrName, inIdx, attrIdx, outIdx);
                        case AttributeType.UINT:
                            attrIdx = readIntArrays(compiledGraph, variables, AttrPort, false);
                            return new SetAttributeCompiledUInt(attrName, inIdx, attrIdx, outIdx);
                        case AttributeType.UINT2:
                            attrIdx = readInt2Arrays(compiledGraph, variables, AttrPort, false);
                            return new SetAttributeCompiledUInt2(attrName, inIdx, attrIdx, outIdx);
                        case AttributeType.UINT3:
                            attrIdx = readInt3Arrays(compiledGraph, variables, AttrPort, false);
                            return new SetAttributeCompiledUInt3(attrName, inIdx, attrIdx, outIdx);
                        case AttributeType.UINT4:
                            attrIdx = readInt4Arrays(compiledGraph, variables, AttrPort, false);
                            return new SetAttributeCompiledUInt4(attrName, inIdx, attrIdx, outIdx);
                    }

                }
            }
            return null;
        }
        protected override void OnDefineOptions(IOptionDefinitionContext ctx)
        {
            CountOpt = ctx.AddOption<int>("Count").WithDefaultValue(1).Build();
            CountOpt.TryGetValue(out int count);
            for (int i = 0; i < count; i++)
            {
                ctx.AddOption<AttributeType>("Type "+(i+1)).WithDefaultValue(AttributeType.FLOAT).Delayed();
                ctx.AddOption<string>("Name "+(i + 1)).WithDefaultValue("").Delayed();
            }
        }
        protected override void OnDefinePorts(IPortDefinitionContext ctx)
        {
            InInstancesPort = ctx.AddInputPort<Transform>("Instances").Build();
            OutInstancesPort = ctx.AddOutputPort<Transform>("Instances").Build();
            CountOpt.TryGetValue(out int count);
            for (int i = 0; i < count; i++)
            {
                if (GetNodeOptionByName("Type "+(i+1)).TryGetValue(out AttributeType attrType))
                {   
                    string AttrPortName = "Attribute " + (i + 1);
                    switch (attrType)
                    {
                        case AttributeType.FLOAT:
                            ctx.AddInputPort<float>(AttrPortName ).Build();
                            break;
                        case AttributeType.FLOAT2:
                            ctx.AddInputPort<Vector2>(AttrPortName ).Build();
                            break;
                        case AttributeType.FLOAT3:
                            ctx.AddInputPort<Vector3>(AttrPortName ).Build();
                            break;
                        case AttributeType.FLOAT4:
                            ctx.AddInputPort<Vector4>(AttrPortName ).Build();
                            break;
                        case AttributeType.UINT:
                        case AttributeType.INT:
                            ctx.AddInputPort<int>(AttrPortName ).Build();
                            break;
                        case AttributeType.UINT2:
                        case AttributeType.INT2:
                            ctx.AddInputPort<Vector2Int>(AttrPortName ).Build();
                            break;
                        case AttributeType.UINT3:
                        case AttributeType.INT3:
                            ctx.AddInputPort<Vector3Int>(AttrPortName ).Build();
                            break;
                        case AttributeType.UINT4:
                        case AttributeType.INT4:
                            ctx.AddInputPort<int4>(AttrPortName).Build();
                            break;
                    }

                    
                }
            }
        }
    }
    [Serializable]
    public class OverrideMaterials : EnvNode
    {
        IPort InstancesPort, OverridenInstancesPort;
        INodeOption MaterialsOpt;
        public override EnvCompiledFunction compile(EnvCompiledGraph compiledGraph, Dictionary<IPort, int> variables)
        {
            List<Material> mats;
            MaterialsOpt.TryGetValue(out mats);
            return new OverrideMaterialsCompiled(
                inArg: readProcInstanceSets(compiledGraph, variables, InstancesPort, true),
                outArg: writeProcInstanceSets(compiledGraph, variables, OverridenInstancesPort),
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
            OverridenInstancesPort = ctx.AddInputPort<Transform>("Output Instances").Build();
        }
    }


    [Serializable]
    public class JoinInstances : EnvNode
    {
        List<IPort> InstancesPort = new List<IPort>();
        IPort JoinedInstancesPort;

        public override EnvCompiledFunction compile(EnvCompiledGraph compiledGraph, Dictionary<IPort, int> variables)
        {
            int[] inputs = new int[InstancesPort.Count];
            for (int i = 0; i < InstancesPort.Count; i++)
            {
                inputs[i] = readProcInstanceSets(compiledGraph, variables, InstancesPort[i], false);
            }
            return new JoinInstancesCompiled(inputArgs: inputs, outArg: writeProcInstanceSets(compiledGraph, variables, JoinedInstancesPort));
        }

        protected override void OnDefineOptions(IOptionDefinitionContext ctx)
        {
            ctx.AddOption<int>("Count").WithDefaultValue(2).Delayed();
        }
        protected override void OnDefinePorts(IPortDefinitionContext ctx)
        {
            int count = 2;
            if(!GetNodeOptionByName("Count").TryGetValue(out count))
            {
                count = 2;
            }
            for (int i = 0; i < count; i++)
            {
                InstancesPort.Add(ctx.AddInputPort<Transform>("Instances "+(i+1)).Build());
            }
            JoinedInstancesPort = ctx.AddOutputPort<Transform>("Joined Instances").Build();
        }
    }

    [Serializable]
    public class SetInstancedObject : EnvNode
    {
        public override EnvCompiledFunction compile(EnvCompiledGraph compiledGraph, Dictionary<IPort, int> variables)
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
        INodeOption MatOpt, MatWeightsParamOpt;
        public int instancesIdx;
        public int landscapeIdx;
        public string matWeightsParam;
        public Material landscapeMaterial;
        List<IPort> WeightPorts = new List<IPort>();
        public override EnvCompiledFunction compile(EnvCompiledGraph compiledGraph, Dictionary<IPort, int> variables)
        {
            instancesIdx = readProcInstanceSets(compiledGraph, variables, InstancesPort, false);
            landscapeIdx = readProcMeshes(compiledGraph, variables, LandscapePort, false);
            MatOpt.TryGetValue(out landscapeMaterial);
            MatWeightsParamOpt.TryGetValue(out matWeightsParam);
            int[] weightsArgs = new int[WeightPorts.Count];
            for (int i=0;i< WeightPorts.Count; i++)
            {
                weightsArgs[i]= readFloatArrays(compiledGraph, variables, WeightPorts[i], false);
            }
            return new ReturnCompiled(weightsArgs, matWeightsParam, landscapeMaterial);
        }

        protected override void OnDefineOptions(IOptionDefinitionContext ctx)
        {
            
            ctx.AddOption<TerrainLayers>("Layers").Delayed();
            MatOpt = ctx.AddOption<Material>("LandscapeMaterial").Build();
            MatWeightsParamOpt = ctx.AddOption<string>("MaterialWeightsParam").Build();

        }
        protected override void OnDefinePorts(IPortDefinitionContext ctx)
        {
            InstancesPort = ctx.AddInputPort<Transform>("Instances").Build();
            LandscapePort = ctx.AddInputPort<Mesh>("Landscape").Build();

            TerrainLayers layers = null;
            GetNodeOptionByName("Layers").TryGetValue(out layers);
            WeightPorts.Clear();
            if (layers != null)
            {
                for (int i = 1; i <= layers.diffuse.depth; i++)
                {
                    string name = "Weights " +  (layers.names == null || layers.names.Length <= i || layers.names[i] == null ? i : layers.names[i]);
                    IPort weightsPort = ctx.AddInputPort<float>(name).Build();
                    WeightPorts.Add(weightsPort);
                }
            }
            
        }


    }

}
