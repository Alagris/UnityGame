using System.Collections.Generic;
using System;
using TMPro;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ProcTerrainGenerator : MonoBehaviour
{
    public enum WarpType
    {
        PERLIN, PERLIN_FBM, MORENOISE, NONE
    }
    public enum NoiseType
    {
        PERLIN, PERLIN_FBM, MORENOISE
    }
    
    [SerializeField]
    internal WarpType warpType = WarpType.NONE;
    [SerializeField]
    internal NoiseType noiseType = NoiseType.MORENOISE;
    [SerializeField]
    internal float offset = 0;
    [SerializeField]
    internal int resX =128;
    [SerializeField]
    internal int resY = 128;
    [SerializeField]
    internal float2 size =10;
    [SerializeField]
    internal float scale =1;
    [SerializeField]
    internal float height =10;
    [SerializeField]
    internal float uvScale =1;
    [SerializeField]
    internal float pointiness =0.8f;
    [SerializeField]
    internal float scalingPowerBase =1.5f;
    [SerializeField]
    internal int iterations =5;
    [SerializeField]
    internal bool enableCollision =true;
 

    [SerializeField]
    internal float chunkSize =10;
    [SerializeField]
    internal PlayerController Player;
    [SerializeField]
    ProcSection SectionPrefab;
    [SerializeField]
    internal bool ShadeFlat = false;

    // GraphicsBuffer grassEulerAngles;
    // GraphicsBuffer grassVertices;
    // GraphicsBuffer grassColors;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Player == null)
        {
            Player = FindFirstObjectByType<PlayerController>();
        }

       
        //MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        //meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
        
        
    }



    
    // Update is called once per frame
    void Update()
    {
       
    }
}