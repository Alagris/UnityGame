using Env.Runtime;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
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
            if (!finalNode.LandscapePort.isConnected)
            {
                Debug.LogError($"No landscape is returned in EnvGraph: {ctx.assetPath}");
                return;
            }
            List<EnvNode> sortedNodes = new List<EnvNode>();
            TopologicalSort(finalNode, sortedNodes, new HashSet<EnvNode>());
            compiledGraph.functions = new EnvCompiledFunction[sortedNodes.Count];   
            Dictionary<IPort, int> variables = new Dictionary<IPort, int>();
            for(int i=0;i<sortedNodes.Count;i++)
            { 
                EnvNode node = sortedNodes[i];
                AddInputs(node, i,sortedNodes,variables,compiledGraph, ctx);
                compiledGraph.functions[i] = node.compile(variables);
            }
            AddInputs(finalNode, sortedNodes.Count, sortedNodes, variables, compiledGraph, ctx);
            for (int i = 0; i < sortedNodes.Count; i++)
            {
                EnvNode node = sortedNodes[i];
                compiledGraph.functions[i] = node.compile(variables);
            }
            
            compiledGraph.returnedLandscape = variables[finalNode.LandscapePort.firstConnectedPort];
            compiledGraph.returnedInstances = finalNode.InstancesPort.isConnected ? variables[finalNode.InstancesPort.firstConnectedPort] : -1;
            ctx.AddObjectToAsset("RuntimeAsset", compiledGraph);
            ctx.SetMainObject(compiledGraph);
        }
        static void AddInputs(EnvNode node, int i, List<EnvNode> sortedNodes, Dictionary<IPort, int> variables, EnvCompiledGraph compiledGraph, AssetImportContext ctx)
        {
            
            foreach (var inPort in node.GetInputPorts())
            {
                if (inPort.isConnected)
                {
                    IPort srcPort = inPort.firstConnectedPort;
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
                        switch (srcPort.dataType)
                        {
                            case System.Type x when x == typeof(int):
                                idx = compiledGraph.intArraysCount++;
                                break;
                            case System.Type x when x == typeof(float):
                                idx = compiledGraph.floatArraysCount++;
                                break;
                            case System.Type x when x == typeof(Vector2Int):
                                idx = compiledGraph.int2ArraysCount++;
                                break;
                            case System.Type x when x == typeof(Vector2):
                                idx = compiledGraph.float2ArraysCount++;
                                break;
                            case System.Type x when x == typeof(Vector3Int):
                                idx = compiledGraph.int3ArraysCount++;
                                break;
                            case System.Type x when x == typeof(Vector3):
                                idx = compiledGraph.float3ArraysCount++;
                                break;
                            case System.Type x when x == typeof(Mesh):
                                idx = compiledGraph.procMeshesCount++;
                                break;
                            case System.Type x when x == typeof(Transform):
                                idx = compiledGraph.procInstanceSetsCount++;
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
                if (port.isConnected)
                {
                    IPort source = port.firstConnectedPort;
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
