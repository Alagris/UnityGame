using System;
using Unity.Mathematics;
using UnityEngine;
using static ProcTerrainGenerator;
using UnityEngine.UIElements;

public class ProcSection : MonoBehaviour, Section
{
    internal ProcTerrainGenerator world;
    internal int idx;
    public void Setup(ProcTerrainGenerator world, int idx)
    {
        this.gameObject.name = "Section " + idx;
        this.world = world;
        this.idx = idx;
        terrainMeshFilter = gameObject.AddComponent<MeshFilter>();

        if (world.spawnGrass)
        {
            GameObject grassChild = new GameObject("Grass");
            grassChild.transform.SetParent(transform, false);
            MeshRenderer grassMeshRenderer = grassChild.AddComponent<MeshRenderer>();
            grassMeshRenderer.material = world.grassMaterial;
            grassMeshFilter = grassChild.AddComponent<MeshFilter>();
        }
        if (world.enableCollision)
        {
            terrainMeshCollision = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;

        }
    }

    MeshFilter terrainMeshFilter, grassMeshFilter;
    MeshCollider terrainMeshCollision;
    private int2 sectionPosition;

 
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
        ProcMesh mesh;
        float2 xzOffset = (float2)sectionPosition * world.chunkSize;
        float3 offset = new float3(xzOffset.x, world.offset, xzOffset.y);
        float2 chunkSize = new float2(world.chunkSize, world.chunkSize);
        switch (world.noiseType)
        {
            case NoiseType.MORENOISE:
                mesh = ProcMesh.morenoise(offset, world.resX, world.resY, chunkSize, world.scale, world.pointiness, world.scalingPowerBase, world.iterations, world.height, world.uvScale, true);
                break;
            case NoiseType.PERLIN:
                mesh = ProcMesh.perlin(offset, world.resX, world.resY, chunkSize, world.scale, world.height, world.uvScale, true);
                break;
            default:
                mesh = null;
                break;
        }
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

        Mesh m = mesh.toMesh();
        terrainMeshFilter.mesh = m;
        terrainMeshCollision.sharedMesh = world.enableCollision ? m : null;
    }

    public void OnUnload()
    {
        grassMeshFilter.mesh = null;
        terrainMeshFilter.mesh = null;
        terrainMeshCollision.sharedMesh = null;
        
    }
}
