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
    internal uint seed =4534842;
    [SerializeField]
    internal bool spawnGrass=true;
    [SerializeField]
    internal Material grassMaterial;
    [SerializeField]
    internal uint grassSeed = 474767;
    [SerializeField]
    internal float grassMaxTilt = 30;
    [SerializeField]
    internal float grassMinWidth = 1.0f;
    [SerializeField]
    internal float grassMaxWidth = 1.0f;
    [SerializeField]
    internal float grassMinHeight = 1.0f;
    [SerializeField]
    internal float grassMaxHeight = 1.0f;
    [SerializeField]
    internal Color grassBright = new Color(1,1,1);
    [SerializeField]
    internal Color grassDark = new Color(0.5f,1,0.5f);

    [SerializeField]
    internal int chunkDespawnRadius = 3;
    [SerializeField]
    internal float chunkSpawnRadius = 2;
    [SerializeField]
    internal float chunkSize =10;
    [SerializeField]
    PlayerController Player;
    [SerializeField]
    ProcSection SectionPrefab;

    MovingGrid<ProcSection> Grid;
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

        Grid = new MovingGrid<ProcSection>((idx) => {
            GameObject sectionObj = Instantiate(SectionPrefab.gameObject, transform);
            ProcSection section = sectionObj.GetComponent<ProcSection>();
            section.Setup(this, idx);
            return section;
        }, chunkSize, chunkDespawnRadius, chunkSpawnRadius, new float3(0,0,0));
        //MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        //meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
        
        
    }

   
    public void Refresh()
    {
        if (Player.HasCharacter())
        {
            Grid.reset(chunkDespawnRadius, chunkSize, Player.GetCharacterPosition());
        }
        else
        {
            Grid.forEachSection(m => m.Refresh());
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Player.HasCharacter())
        {
            float3 position = Player.GetCharacterPosition();
            Grid.update(position, chunkSpawnRadius);
        }
    }
}