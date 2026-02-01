using System;
using Unity.Mathematics;
using UnityEngine;


public struct RandomNumberGenerator
{

    private ulong x_;

    public RandomNumberGenerator(uint seed = 0)
    {
        x_ = 0;
        this.seed(seed);
    }

    /**
     * Creates a random number generator with a somewhat random seed. This can be used when
     * determinism is not necessary or not desired.
     */
    //static RandomNumberGenerator from_random_seed();

    /**
     * Set the seed for future random numbers.
     */
    public void seed(uint seed)
    {
        const ulong lowseed = 0x330E;
        x_ = ((ulong)(seed) << 16) | lowseed;
    }

    public uint get_uint32()
    {
        step();
        return (uint)(x_ >> 17);
    }

    public int get_int32()
    {
        step();
        return (int)(x_ >> 17);
    }

    public ulong get_uint64()
    {
        return ((ulong)(get_uint32()) << 32) | get_uint32();
    }

    /**
     * \return Random value (0..N), but never N.
     */
    public int get_int32(int max_exclusive)
    {
        Debug.Assert(max_exclusive > 0);
        return get_int32() % max_exclusive;
    }

    /**
     * \return Random value (0..1), but never 1.0.
     */
    public double get_double()
    {
        return (double)(get_int32()) / 0x80000000;
    }

    /**
     * \return Random value (0..1), but never 1.0.
     */
    public float get_float()
    {
        return (float)get_int32() / 0x80000000;
    }

    public float get_float_in(float min, float max)
    {
        return get_float()*(max-min)+min;
    }

    public float2 get_float2()
    {
        return new float2(get_float(), get_float());
    }

    public float3 get_float3()
    {
        return new float3(get_float(), get_float(), get_float());
    }
    /**
     * Compute uniformly distributed barycentric coordinates.
     */
    public float3 get_barycentric_coordinates()
    {
        float rand1 = get_float();
        float rand2 = get_float();

        if (rand1 + rand2 > 1.0f)
        {
            rand1 = 1.0f - rand1;
            rand2 = 1.0f - rand2;
        }

        return new float3(rand1, rand2, 1.0f - rand1 - rand2);
    }

    /**
     * Round value to the next integer randomly.
     * 4.9f is more likely to round to 5 than 4.6f.
     */
    public int round_probabilistic(float x)
    {
        /* Support for negative values can be added when necessary. */
        Debug.Assert(x >= 0.0f);
        float round_up_probability = (x);
        bool round_up = round_up_probability > get_float();
        return (int)(x) + (round_up ? 1 : 0);
    }

    public float2 get_unit_float2()
    {
        float a = (float)(Mathf.PI * 2.0f) * get_float();
        return new float2(Mathf.Cos(a), Mathf.Sin(a));
    }
    public float3 get_unit_float3()
    {
        float z = (2.0f * get_float()) - 1.0f;
        float r = 1.0f - z * z;
        if (r > 0.0f)
        {
            float a = (float)(Mathf.PI * 2.0f) * get_float();
            r = Mathf.Sqrt(r);
            float x = r * Mathf.Cos(a);
            float y = r * Mathf.Sin(a);
            return new float3(x, y, z);
        }
        return new float3(0.0f, 0.0f, 1.0f);
    }
    /**
     * Generate a random point inside the given triangle.
     */
    public float2 get_triangle_sample(float2 v1, float2 v2, float2 v3)
    {
        float u = get_float();
        float v = get_float();

        if (u + v > 1.0f)
        {
            u = 1.0f - u;
            v = 1.0f - v;
        }

        float2 side_u = v2 - v1;
        float2 side_v = v3 - v1;

        float2 sample = v1;
        sample += side_u * u;
        sample += side_v * v;
        return sample;
    }
    public float3 get_triangle_sample_3d(float3 v1, float3 v2, float3 v3)
    {
        float u = get_float();
        float v = get_float();

        if (u + v > 1.0f)
        {
            u = 1.0f - u;
            v = 1.0f - v;
        }

        float3 side_u = v2 - v1;
        float3 side_v = v3 - v1;

        float3 sample = v1;
        sample += side_u * u;
        sample += side_v * v;
        return sample;
    }
    /**
     * Simulate getting \a n random values.
     */
    public void skip(int n)
    {
        while (n-- > 0)
        {
            step();
        }
    }


    public void step()
    {
        const ulong multiplier = 0x5DEECE66DL;
        const ulong addend = 0xB;
        const ulong mask = 0x0000FFFFFFFFFFFFL;

        x_ = (multiplier * x_ + addend) & mask;
    }
    /**minTilt, maxTilt is in radians*/
    internal Quaternion RandomRot(float3 nor, float minTilt, float maxTilt)
    {
        return Quaternion.LookRotation(Vector3.forward, nor) * RandomRot(minTilt, maxTilt);
    }
    /**minTilt, maxTilt is in radians*/
    internal Quaternion RandomRot(float minTilt, float maxTilt)
    {
        float pitch = get_float_in(minTilt, maxTilt);
        float yaw = get_float_in(0, Mathf.PI*2);
        return Quaternion.EulerAngles(pitch, yaw, 0);
    }
}
