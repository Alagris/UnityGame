using System.Collections.Generic;
using System;
using TMPro;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class FloatingIslandGenerator : MonoBehaviour
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
    WarpType warpType = WarpType.NONE;
    [SerializeField]
    NoiseType noiseType = NoiseType.MORENOISE;
    [SerializeField]
    float3 offset = new float3(-5, 0, -5);
    [SerializeField]
    int resX=128;
    [SerializeField]
    int resY = 128;
    [SerializeField]
    float2 size=10;
    [SerializeField]
    float scale=1;
    [SerializeField]
    float height=10;
    [SerializeField]
    float uvScale=1;
    [SerializeField]
    float pointiness=0.8f;
    [SerializeField]
    float scalingPowerBase=1.5f;
    [SerializeField]
    int iterations=5;
    [SerializeField]
    bool enableCollision=true;
    [SerializeField]
    uint seed=4534842;
    [SerializeField]
    bool spawnGrass=true;
    [SerializeField]
    Material grassMaterial;
    [SerializeField]
    uint grassSeed = 474767;
    [SerializeField]
    float grassMaxTilt = 30;
    [SerializeField]
    float grassMinWidth = 1.0f;
    [SerializeField]
    float grassMaxWidth = 1.0f;
    [SerializeField]
    float grassMinHeight = 1.0f;
    [SerializeField]
    float grassMaxHeight = 1.0f;
    [SerializeField]
    Color grassBright = new Color(1,1,1);
    [SerializeField]
    Color grassDark = new Color(0.5f,1,0.5f);

    MeshFilter terrainMeshFilter, grassMeshFilter;
    MeshCollider terrainMeshCollision;

    // GraphicsBuffer grassEulerAngles;
    // GraphicsBuffer grassVertices;
    // GraphicsBuffer grassColors;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        //MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        //meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

        terrainMeshFilter = gameObject.AddComponent<MeshFilter>();

        if (spawnGrass)
        {
            GameObject grassChild = new GameObject("Grass");
            grassChild.transform.parent = transform;
            MeshRenderer grassMeshRenderer = grassChild.AddComponent<MeshRenderer>();
            grassMeshRenderer.material = grassMaterial;
            grassMeshFilter = grassChild.AddComponent<MeshFilter>();
        }
        if (enableCollision)
        {
            terrainMeshCollision = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
            
        }
        Refresh();
    }

    public void Refresh()
    {
        ProcMesh mesh;
        switch (noiseType)
        {
            case NoiseType.MORENOISE:
                mesh = ProcMesh.morenoise(offset, resX, resY, size, scale, pointiness, scalingPowerBase, iterations, height, uvScale, true);
                break;
            case NoiseType.PERLIN:
                mesh = ProcMesh.perlin(offset, resX, resY, size, scale, height, uvScale, true);
                break;
            default:
                mesh = null;
                break;
        }
        if (spawnGrass)
        {
            var grassPoints = mesh.distributePoints((vert, loop) => 1f, seed);
            RandomNumberGenerator rng = new RandomNumberGenerator(grassSeed);
            Vector3[] verts = new Vector3[grassPoints.Count*4];
            int[] indices = new int[grassPoints.Count*6];
            //grassVertices = new GraphicsBuffer(GraphicsBuffer.Target.Structured, grassPoints.Count, 3 * sizeof(float));
            //Array.
           // grassVertices.SetData
            float grassMaxTiltRadians = Mathf.Deg2Rad * grassMaxTilt;
            for(int j=0;j<grassPoints.Count;j++)
            {
                Tuple<float3, float3> t = grassPoints[j];
                float width = rng.get_float_in(grassMinWidth, grassMaxWidth);
                float height = rng.get_float_in(grassMinWidth, grassMaxHeight);
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
                for(int i = 0;i< quadVerts.Length; i++) {
                    float3 rotated = math.mul(rotationMatrix , quadVerts[i]);
                    float3 translated = t.Item1 +  rotated;
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
        
        Mesh m = mesh.toMesh();
        terrainMeshFilter.mesh = m;
        terrainMeshCollision.sharedMesh = enableCollision? m : null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}