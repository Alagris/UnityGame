
using System;
using Unity.Mathematics;
using UnityEngine;

using UnityEngine.UIElements;
using System.Linq;
using System.Collections.Generic;
using Codice.CM.SEIDInfo;
using Env.Runtime;

public class ProcMesh
{
    public Vector3[] vertices;
    public Vector2[] uvs;
    public Vector3[] normals;
    public int[] triangles;

    public Mesh toMesh()
    {
        if (vertices.Length > short.MaxValue)
        {
            Debug.LogError("Warning! Number of vertices exceeds short.MaxValue");
        }
        Mesh o = new Mesh();
        o.vertices = vertices;
        o.uv = uvs;
        o.normals = normals;
        o.triangles = triangles;
        return o;
    }

    public ProcMesh ShadeFlat()
    {
        ProcMesh flat = new ProcMesh();
        flat.vertices = new Vector3[triangles.Length];
        flat.uvs = new Vector2[triangles.Length];
        flat.normals = new Vector3[triangles.Length];
        flat.triangles = new int[triangles.Length];
        for (int i = 0; i < triangles.Length; i ++)
        {
            flat.triangles[i] = i;  
        }
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int i1 = i;
            int i2 = i + 1;
            int i3 = i + 2;
            Vector3 v1 = flat.vertices[i1] = vertices[triangles[i1]];
            Vector3 v2 = flat.vertices[i2] = vertices[triangles[i2]];
            Vector3 v3 = flat.vertices[i3] = vertices[triangles[i3]];
            flat.uvs[i1] = uvs[triangles[i1]];
            flat.uvs[i2] = uvs[triangles[i2]];
            flat.uvs[i3] = uvs[triangles[i3]];
            Vector3 n = -math.normalize(BMath.cross_tri(v1, v2, v3));    
            flat.normals[i1] = flat.normals[i2] = flat.normals[i3] = n;
        }
        return flat;
    }

    /**minTilt, maxTilt is in radians*/
    public ProcInstances distributePoints(float[] density, uint seed, bool alignToNormal, float minTilt, float maxTilt)
    {
        ProcInstances i = new ProcInstances();
        RandomNumberGenerator r = new RandomNumberGenerator(seed);
        Action<float3, float3, int> action;
        if (alignToNormal)
        {
            action = (vert, nor, idx) => i.Add(vert, r.RandomRot(nor, minTilt, maxTilt));
        }
        else
        {
            action = (vert, nor, idx) => i.Add(vert, r.RandomRot(minTilt, maxTilt));
        }
        Noise.distribute_points_on_faces(vertices, normals, triangles, (pos, idx) => density[idx], action, seed);
        return i;
    }
    public List<Tuple<float3, float3>> distributePoints(Func<float3, int, float> densityFunction, uint seed) {
        List<Tuple<float3,float3>> points = new List<Tuple<float3, float3>> ();
        Noise.distribute_points_on_faces(vertices, normals, triangles, densityFunction,(vert, nor, idx)=>points.Add(Tuple.Create(vert, nor)), seed);
        return points;
    }

    public static ProcMesh flat(float3 offset, int resX, int resY, float2 size, bool genTriangles)
    {
        return plane(offset, resX, resY, size, v=>v , genTriangles);
    }
    public static ProcMesh plane(float3 offset, int resX, int resY, float2 size, Func<float3, float3> heightFunction, bool genTriangles)
    {
        return plane(offset, resX, resY, size, heightFunction, (int vx, int vy, float3 vertex)=>new float2(vx / (resX - 1), vy / (resY - 1)), genTriangles);
    }

    public static ProcMesh planeScaledUVs(float3 offset, int resX, int resY, float2 size, Func<float3, float3> heightFunction, float uvScale, bool genTriangles)
    {
        return plane(offset, resX, resY, size, heightFunction, (int vx, int  vy, float3 vertex) => vertex.xy * uvScale, genTriangles);
    }
    /**plane is just a horizontally aligned grid*/
    public static ProcMesh plane(float3 offset, int resX, int resY, float2 size, Func<float3, float3> heightFunction, Func<int,int,float3,float2> uvTransform, bool genTriangles)
    {
        return grid(offset, resX, resY, new float3(size.x, 0, 0), new float3(0, 0, size.y), new float3(0, 1, 0),  heightFunction, uvTransform, genTriangles);
    }
    public static int[] gridTriangles(int resX, int resY)
    {
        int[] triangles = new int[(resY - 1) * (resX - 1) * 2 * 3];
        for (int vy = 0, i = 0; vy < resY - 1; vy++)
        {
            for (int vx = 0; vx < resX - 1; vx++, i += 6)
            {
                int bottomLeft = vx + vy * resX;
                int bottomRight = bottomLeft + 1;
                int topLeft = bottomLeft + resX;
                int topRight = topLeft + 1;
                triangles[i] = bottomLeft;
                triangles[i + 1] = topLeft;
                triangles[i + 2] = bottomRight;

                triangles[i + 3] = bottomRight;
                triangles[i + 4] = topLeft;
                triangles[i + 5] = topRight;

            }
        }
        return triangles;
        
    }
    struct VertNorUv
    {
        float3 vertex;
        float3 normal;
        float2 uv;
    }

    public static float2[] gridVertices2D(int resX, int resZ, float widthX, float heightZ)
    {
        float spacingX = widthX / (resX - 1);
        float spacingZ = heightZ / (resZ - 1);
        float2[] vertices = new float2[resZ * resX];
        for (int vz = 0, i = 0; vz < resZ; vz++)
        {
            for (int vx = 0; vx < resX; vx++, i++)
            {
                vertices[i] = new float2(vx * spacingX, vz * spacingZ);
            }
        }
        return vertices;
    }

    public static T[] forEachPosition2D<T>(float2 offset, int resX, int resZ, float widthX, float heightZ, Func<int, float2, T> f)
    {
        float spacingX = widthX / (resX - 1);
        float spacingZ = heightZ / (resZ - 1);
        T[] o = new T[resZ * resX];
        for (int vz = 0, i = 0; vz < resZ; vz++)
        {
            for (int vx = 0; vx < resX; vx++, i++)
            {
                float2 vertex = offset + new float2(vx * spacingX, vz * spacingZ);
                o[i] = f(i, vertex);
            }
        }
        return o;
    }
    public static ProcMesh gridPrecomputed(float3 offset, int resX, int resZ, float widthX, float heightZ, float3[] gradients, float uvScaling)
    {
        float spacingX = widthX / (resX - 1);
        float spacingZ = heightZ / (resZ - 1);
        ProcMesh o = new ProcMesh();
        Vector3[] vertices = new Vector3[resZ * resX];
        Vector2[] uvs = new Vector2[resZ * resX];
        Vector3[] normals = new Vector3[resZ * resX];

        for (int vz = 0, i = 0; vz < resZ; vz++)
        {
            for (int vx = 0; vx < resX; vx++, i++)
            {
                float3 vertex = offset + new float3(vx * spacingX, gradients[i].z, vz * spacingZ);
                vertices[i] = vertex;
                uvs[i] = vertex.xz * uvScaling;
                // I'm not sure why the normal has to be negated. This code was originally written in unreal and now ported to unity.
                // Unreal uses different axes. I suspect that this has something to do with unity's using different convention for which side is up and which is down.
                float3 normal = -math.normalize(BMath.normal(gradients[i].xy));
                normals[i] = normal;   
            }
        }
        o.vertices = vertices;
        o.uvs = uvs;
        o.normals = normals;
        o.triangles = gridTriangles(resX, resZ);
        return o;
    }
    public static ProcMesh grid(float3 offset, int resX, int resY, float3 width, float3 height, float3 displacementDirection, Func<float3,float3> displacementFunction, Func<int, int, float3, float2> uvTransform, bool genTriangles)
    {
        float3 spacingX = width / (resX - 1);
        float3 spacingY = height / (resY - 1);
        ProcMesh o = new ProcMesh();
        Vector3[] vertices = new Vector3[resY * resX];
		Vector2[] uvs = new Vector2[resY * resX];
		Vector3[] normals = new Vector3[resY * resX];
		Vector3[] tangents = new Vector3[resY * resX];

        for (int vy = 0, i=0; vy < resY; vy++)
        {
            for (int vx = 0; vx < resX; vx++,i++)
            {
                float3 vertex = offset + (float)(vx) * spacingX + (float)(vy) * spacingY;
                float3 derivative_and_displacement = displacementFunction(vertex);

                vertex += displacementDirection * derivative_and_displacement.z;
                vertices[i] = vertex;
                float2 uv = uvTransform(vx, vy, vertex);
				uvs[i] = uv;
                // I'm not sure why the normal has to be negated. This code was originally written in unreal and now ported to unity.
                // Unreal uses different axes. I suspect that this has something to do with unity's using different convention for which side is up and which is down.
                float3 normal = -math.normalize(BMath.normal(derivative_and_displacement.xy));
                float3 tangent = -math.normalize(BMath.tangent(derivative_and_displacement.xy));
                normals[i] = normal;
                tangents[i] = tangent;
            }
        }
        o.vertices = vertices;
        o.uvs = uvs;
        o.normals = normals;
        

        if (genTriangles ) {
            o.triangles = gridTriangles(resX,resY);
        }
        return o;
    }

    public static ProcMesh perlin_fbm(float3 offset, int resX, int resY, float2 size, float scale, float heightPowerBase, float scalePowerBase, int iterations, float height, float uvScale, bool genTriangles)
    {
        return planeScaledUVs(offset, resX, resY, size,  vertex => Noise.perlin_fbm_derivative(vertex.xz, scale, height, heightPowerBase, scalePowerBase, iterations), uvScale, genTriangles);
    }
    public static ProcMesh perlin(float3 offset, int resX, int resY, float2 size, float scale, float height, float uvScale, bool genTriangles)
    {
        return planeScaledUVs(offset, resX, resY, size, vertex => Noise.perlin_noise_derivative(vertex.xz, scale) * height, uvScale, genTriangles);
    }
    public static ProcMesh morenoise(float3 offset, int resX, int resY, float2 size, float scale, float pointiness, float scalingPowerBase, int iterations, float height, float uvScale, bool genTriangles)
    {
        return planeScaledUVs(offset, resX, resY, size, vertex=>Noise.morenoise(vertex.xz, scale, pointiness, scalingPowerBase, iterations) * height, uvScale, genTriangles);
    }


}
