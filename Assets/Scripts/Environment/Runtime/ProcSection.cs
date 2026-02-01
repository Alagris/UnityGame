using System;
using Unity.Mathematics;
using UnityEngine;
using Env.Runtime;
using NUnit.Framework;
using System.Collections.Generic;

public class ProcSection : MonoBehaviour, Section
{
    ProcSectionSpawner spawner;
    
    internal int idx;
    MeshRenderer terrainMeshRenderer;
    MeshFilter terrainMeshFilter, grassMeshFilter;
    MeshCollider terrainMeshCollision;
    private int2 sectionPosition;

    public void Setup(ProcSectionSpawner spawner, int idx)
    {
        
        this.spawner = spawner;
        this.idx = idx;
        terrainMeshRenderer = gameObject.AddComponent<MeshRenderer>();
        terrainMeshFilter = gameObject.AddComponent<MeshFilter>();
        terrainMeshCollision = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        List<Material> mats = new List<Material>();
        mats.Add(spawner.LandscapeMaterial);
        terrainMeshRenderer.SetSharedMaterials(mats);
    }


 
    public string GetDebugStr()
    {
        string a = idx.ToString();
        string p = a.PadLeft(3);
        return p;
    }

    public void OnDistanceChanged(float distance)
    {
        
    }

    public void OnExit()
    {
        Destroy(gameObject);
    }

    public void OnLoad(float distance, int2 absolutePosition)
    {
        this.sectionPosition = absolutePosition;
        Refresh();
    }

    public void Refresh()
    {
        Debug.Log("Refreshing section "+idx);
        float2 xzOffset = (float2)sectionPosition * spawner.ChunkSize;
        float3 offset = new float3(xzOffset.x, spawner.OffsetY, xzOffset.y);
        Blackboard bb = spawner.ProcEnv.Run(spawner.ResX, spawner.ResZ, spawner.ChunkSize, offset);
        
        Mesh m = bb.GetReturnedMesh().toMesh();
        
        terrainMeshFilter.mesh = m;
        terrainMeshCollision.sharedMesh = m ;
    }

    public void OnUnload()
    {
        grassMeshFilter.mesh = null;
        terrainMeshFilter.mesh = null;
        terrainMeshCollision.sharedMesh = null;
        
    }
}
