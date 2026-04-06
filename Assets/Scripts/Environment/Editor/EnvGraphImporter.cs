using Env.Runtime;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Env.Editor
{
    [ScriptedImporter(1, EnvGraph.AssetExtension)]
    public class EnvGraphImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            EnvGraph g = GraphDatabase.LoadGraphForImporter<EnvGraph>(ctx.assetPath);
            if (g == null)
            {
                Debug.LogError($"Cound't load EnvGraph: {ctx.assetPath}");
                return;
            }
            EnvCompiledGraph compiledGraph = ScriptableObject.CreateInstance<EnvCompiledGraph>();
            Return finalNode = g.GetNodes().OfType<Return>().FirstOrDefault();
            if (finalNode == null)
            {
                Debug.LogError($"No Return node is present in EnvGraph: {ctx.assetPath}");
                return;
            }
            if (!finalNode.LandscapePort.IsConnected)
            {
                Debug.LogError($"No landscape is returned in EnvGraph: {ctx.assetPath}");
                return;
            }
            List<EnvNode> sortedNodes = new List<EnvNode>();
            TopologicalSort(finalNode, sortedNodes, new HashSet<EnvNode>());
            sortedNodes.Add(finalNode);
            compiledGraph.functions = new EnvCompiledFunction[sortedNodes.Count];   
            Dictionary<IPort, int> variables = new Dictionary<IPort, int>();
            for(int i=0;i<sortedNodes.Count;i++)
            { 
                EnvNode node = sortedNodes[i];
                AddInputs(node, i,sortedNodes,variables,compiledGraph, ctx);
            }
            for (int i = 0; i < sortedNodes.Count; i++)
            {
                EnvNode node = sortedNodes[i];
                compiledGraph.functions[i] = node.compile(compiledGraph, variables);
            }
            compiledGraph.returnedLandscape = finalNode.landscapeIdx;
            compiledGraph.returnedInstances = finalNode.instancesIdx;
            ctx.AddObjectToAsset("RuntimeAsset", compiledGraph);
            ctx.SetMainObject(compiledGraph);
        }
        static void AddInputs(EnvNode node, int i, List<EnvNode> sortedNodes, Dictionary<IPort, int> variables, EnvCompiledGraph compiledGraph, AssetImportContext ctx)
        {
            
            foreach (var inPort in node.GetInputPorts())
            {
                if (inPort.IsConnected)
                {
                    IPort srcPort = inPort.FirstConnectedPort;
                    EnvNode srcNode = (EnvNode)srcPort.GetNode();
                    int srcIdx = sortedNodes.IndexOf(srcNode);
                    if (srcIdx >= i)
                    {
                        Debug.LogError($"EnvGraph contains a dependency cycle between {srcNode} and {node} in : {ctx.assetPath}");
                        return;
                    }
                    int idx;
                    if (variables.ContainsKey(srcPort))
                    {
                        idx = variables[srcPort];
                    }
                    else
                    {
                        static int push(List<int> l, IPort srcPort)
                        {
                            int idx = l.Count;
                            //List<IPort> dstPorts = new List<IPort>();
                            //srcPort.GetConnectedPorts(dstPorts);
                            l.Add(0);
                            return idx;
                        }
                        switch (srcPort.DataType)
                        {
                            case System.Type x when x == typeof(int):
                                idx = push(compiledGraph.intArraysCount, srcPort);
                                break;
                            case System.Type x when x == typeof(float):
                                idx = push(compiledGraph.floatArraysCount, srcPort);
                                break;
                            case System.Type x when x == typeof(Vector2Int):
                                idx = push(compiledGraph.int2ArraysCount, srcPort);
                                break;
                            case System.Type x when x == typeof(Vector2):
                                idx = push(compiledGraph.float2ArraysCount, srcPort);
                                break;
                            case System.Type x when x == typeof(Vector3Int):
                                idx = push(compiledGraph.int3ArraysCount, srcPort);
                                break;
                            case System.Type x when x == typeof(Vector3):
                                idx = push(compiledGraph.float3ArraysCount, srcPort);
                                break;
                            case System.Type x when x == typeof(Mesh):
                                idx = push(compiledGraph.procMeshesCount, srcPort);
                                break;
                            case System.Type x when x == typeof(Transform):
                                idx = push(compiledGraph.procInstanceSetsCount, srcPort);
                                break;
                            case System.Type x when x == typeof(InstanceableObject):
                                idx = push(compiledGraph.objectCount, srcPort);
                                break;
                            case System.Type x when x == typeof(Color):
                                idx = push(compiledGraph.colorCount, srcPort);
                                break;
                            default:
                                Debug.Assert(false);
                                idx = -1;
                                break;
                        }
                        Debug.Assert(idx>=0);
                        variables.Add(srcPort, idx);
                    }
                    Debug.Assert(idx >= 0);
                    variables.Add(inPort, idx);
                }
            }
            
        }
        static void TopologicalSort(EnvNode node, List<EnvNode> output, HashSet<EnvNode> visited)
        {
            foreach (IPort port in node.GetInputPorts())
            {
                if (port.IsConnected)
                {
                    IPort source = port.FirstConnectedPort;
                    INode next = source.GetNode();
                    if (next is EnvNode)
                    {
                        EnvNode n = (EnvNode)next;
                        if (visited.Add(n))
                        {
                            TopologicalSort(n, output, visited);
                            output.Add(n);
                        }
                    }
                }
            }
        }
        
    }
}
