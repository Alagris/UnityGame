using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ProcGrass : MonoBehaviour
{
    public Material material;

    [SerializeField]


    GraphicsBuffer meshNormals;
    GraphicsBuffer meshVertices;

    public void SetData(List<Tuple<float3, float3>> vertsNormals, uint seed)
    {
       /* 
        Array vertices = vertsNormals.ToArray();
        meshNormals = new GraphicsBuffer(GraphicsBuffer.Target.Structured, vertsNormals.Count, 3 * sizeof(float));
        meshNormals.SetData(mesh.triangles);
        meshVertices = new GraphicsBuffer(GraphicsBuffer.Target.Structured, vertsNormals.Count, 3 * sizeof(float));
        meshVertices.SetData(mesh.vertices);
       */
    }
}
