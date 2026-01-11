using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Noise
{
    


    public static uint hash_bit_rotate(uint x, int k)
    {
        return (x << k) | (x >> (32- k));
    }

    public static void hash_bit_mix(ref uint a, ref uint b, ref uint c)
    {
        a -= c;
        a ^= hash_bit_rotate(c, 4);
        c += b;
        b -= a;
        b ^= hash_bit_rotate(a, 6);
        a += c;
        c -= b;
        c ^= hash_bit_rotate(b, 8);
        b += a;
        a -= c;
        a ^= hash_bit_rotate(c, 16);
        c += b;
        b -= a;
        b ^= hash_bit_rotate(a, 19);
        a += c;
        c -= b;
        c ^= hash_bit_rotate(b, 4);
        b += a;
    }

    public static void hash_bit_final(ref uint a, ref uint b, ref uint c)
    {
        c ^= b;
        c -= hash_bit_rotate(b, 14);
        a ^= c;
        a -= hash_bit_rotate(c, 11);
        b ^= a;
        b -= hash_bit_rotate(a, 25);
        c ^= b;
        c -= hash_bit_rotate(b, 16);
        a ^= c;
        a -= hash_bit_rotate(c, 4);
        b ^= a;
        b -= hash_bit_rotate(a, 14);
        c ^= b;
        c -= hash_bit_rotate(b, 24);
    }

    public static uint hash(uint kx)
    {
        uint a, b, c;
        a = b = c = 0xdeadbeef + (1 << 2) + 13;

        a += kx;
        hash_bit_final(ref a, ref b, ref c);
        return c;
    }
    public static uint int_to_uint(int i)
    {
        return unchecked((uint)i);
    }
    public static uint hash(int kx) {
        return hash(int_to_uint(kx));
    }
    public static uint hash(uint kx, uint ky)
    {
        uint a, b, c;
        a = b = c = 0xdeadbeef + (2 << 2) + 13;

        b += ky;
        a += kx;
        hash_bit_final(ref a, ref b, ref c);
        return c;
    }
    public static uint hash(int kx, int ky)
    {
        return hash(int_to_uint(kx), int_to_uint(kx));
    }
    public static uint hash(uint kx, uint ky, uint kz)
    {
        uint a, b, c;
        a = b = c = 0xdeadbeef + (3 << 2) + 13;

        c += kz;
        b += ky;
        a += kx;
        hash_bit_final(ref a, ref b, ref c);
        return c;
    }
    public static uint hash(int kx, int ky, int kz)
    {
        return hash(int_to_uint(kx), int_to_uint(kx), int_to_uint(kz));

    }
    public static uint hash(uint kx, uint ky, uint kz, uint kw)
    {
        uint a, b, c;
        a = b = c = 0xdeadbeef + (4 << 2) + 13;
        a += kx;
        b += ky;
        c += kz;
        hash_bit_mix(ref a, ref b, ref c);

        a += kw;
        hash_bit_final(ref a, ref b, ref c);
        return c;
    }
    public static uint hash(int kx, int ky, int kz, int kw) {
        return hash(int_to_uint(kx), int_to_uint(kx), int_to_uint(kz), int_to_uint(kw));
    }
    [StructLayout(LayoutKind.Explicit)]
    private struct IntFloat
    {
        [FieldOffset(0)]
        public uint IntValue;
        [FieldOffset(0)]
        public float FloatValue;
    }
    public static uint float_as_uint(float f)
    {
        var intFloat = new IntFloat { FloatValue = f };
        return intFloat.IntValue;
    }

    public static uint hash_float(float kx)
    {
        return hash(float_as_uint(kx));
    }

    public static uint hash_float(float2 k)
    {
        return hash(float_as_uint(k.x), float_as_uint(k.y));
    }

    public static uint hash_float(float3 k)
    {
        return hash(float_as_uint(k.x), float_as_uint(k.y), float_as_uint(k.z));
    }

    public static uint hash_float(float4 k)
    {
        return hash(float_as_uint(k.x), float_as_uint(k.y), float_as_uint(k.z), float_as_uint(k.w));
    }


    /* Hashing a number of int into a float in the range [0, 1]. */

    public static float uint_to_float_01(uint k)
    {
        return (float)(k) / (float)(0xFFFFFFFFu);
    }

    public static float hash_to_float(uint kx)
    {
        return uint_to_float_01(hash(kx));
    }

    public static float hash_to_float(uint kx, uint ky)
    {
        return uint_to_float_01(hash(kx, ky));
    }
    public static float hash_to_float(uint kx, uint ky, uint kz)
    {
        return uint_to_float_01(hash(kx, ky, kz));
    }

    public static float hash_to_float(uint kx, uint ky, uint kz, uint kw)
    {
        return uint_to_float_01(hash(kx, ky, kz, kw));
    }

    /* Hashing a number of floats into a float in the range [0, 1]. */

    public static float hash_float_to_float(float k)
    {
        return uint_to_float_01(hash_float(k));
    }

    public static float hash_float_to_float(float2 k)
    {
        return uint_to_float_01(hash_float(k));
    }

    public static float hash_float_to_float(float3 k)
    {
        return uint_to_float_01(hash_float(k));
    }

    public static float hash_float_to_float(float4 k)
    {
        return uint_to_float_01(hash_float(k));
    }



    public static float2 hash_float_to_float2(float3 k)
    {
        return new float2(hash_float_to_float(new float3(k.x, k.y, k.z)),
            hash_float_to_float(new float3(k.z, k.x, k.y)));
    }

    public static float2 hash_float_to_float2(float4 k)
    {
        return new float2(hash_float_to_float(new float4(k.x, k.y, k.z, k.w)),
            hash_float_to_float(new float4(k.z, k.x, k.w, k.y)));
    }

    public static float3 hash_float_to_float3(float k)
    {
        return new float3(hash_float_to_float(k),
            hash_float_to_float(new float2(k, 1.0f)),
            hash_float_to_float(new float2(k, 2.0f)));
    }

    public static float3 hash_float_to_float3(float2 k)
    {
        return new float3(hash_float_to_float(k),
            hash_float_to_float(new float3(k.x, k.y, 1.0f)),
            hash_float_to_float(new float3(k.x, k.y, 2.0f)));
    }

    public static float3 hash_float_to_float3(float3 k)
    {
        return new float3(hash_float_to_float(k),
            hash_float_to_float(new float4(k.x, k.y, k.z, 1.0f)),
            hash_float_to_float(new float4(k.x, k.y, k.z, 2.0f)));
    }

    public static float3 hash_float_to_float3(float4 k)
    {
        return new float3(hash_float_to_float(k),
            hash_float_to_float(new float4(k.z, k.x, k.w, k.y)),
            hash_float_to_float(new float4(k.w, k.z, k.y, k.x)));
    }

    public static float4 hash_float_to_float4(float4 k)
    {
        return new float4(hash_float_to_float(k),
            hash_float_to_float(new float4(k.w, k.x, k.y, k.z)),
            hash_float_to_float(new float4(k.z, k.w, k.x, k.y)),
            hash_float_to_float(new float4(k.y, k.z, k.w, k.x)));
    }


    public static float2 hash_float_to_float2(float2 k)
    {
        return new float2(hash_float_to_float(k), hash_float_to_float(new float3(k.x, k.y, 1.0f)));
    }


    public static float hash_to_float(uint2 k)
    {
        return hash_to_float(k.x, k.y);
    }

    public static float hash_to_float(uint3 k)
    {
        return hash_to_float(k.x, k.y, k.z);
    }

    public static float hash_to_float(uint4 k)
    {
        return hash_to_float(k.x, k.y, k.z, k.w);
    }


    public static float2 hash_to_float2(uint k)
    {
        return new float2(hash_to_float(k),
            hash_to_float(k, 1));
    }


    public static float2 hash_to_float2(uint2 k)
    {
        return new float2(hash_to_float(k), hash_to_float(k.x, k.y, 1));
    }

    public static float2 hash_to_float2(uint3 k)
    {
        return new float2(hash_to_float(k.x, k.y, k.z),
            hash_to_float(k.z, k.x, k.y));
    }

    public static float2 hash_to_float2(uint4 k)
    {
        return new float2(hash_to_float(k.x, k.y, k.z, k.w),
            hash_to_float(k.z, k.x, k.w, k.y));
    }


    public static float3 hash_to_float3(uint k)
    {
        return new float3(hash_to_float(k),
            hash_to_float(k, 1),
            hash_to_float(k, 2));
    }

    public static float3 hash_to_float3(uint2 k)
    {
        return new float3(hash_to_float(k),
            hash_to_float(new uint3(k.x, k.y, 1)),
            hash_to_float(new uint3(k.x, k.y, 2)));
    }

    public static float3 hash_to_float3(uint3 k)
    {
        return new float3(hash_to_float(k),
            hash_to_float(k.x, k.y, k.z, 1),
            hash_to_float(k.x, k.y, k.z, 2));
    }

    public static float3 hash_to_float3(uint4 k)
    {
        return new float3(hash_to_float(k),
            hash_to_float(new uint4(k.z, k.x, k.w, k.y)),
            hash_to_float(new uint4(k.w, k.z, k.y, k.x)));
    }

    public static float4 hash_to_float4(uint4 k)
    {
        return new float4(hash_to_float(k),
            hash_to_float(new uint4(k.w, k.x, k.y, k.z)),
            hash_to_float(new uint4(k.z, k.w, k.x, k.y)),
            hash_to_float(new uint4(k.y, k.z, k.w, k.x)));
    }

    public static float4 hash_to_float4(uint k)
    {
        return new float4(hash_to_float(k),
            hash_to_float(k, 1),
            hash_to_float(k, 2),
            hash_to_float(k, 3));
    }



    public static float fade(float t)
    {
        return t * t * t * (t * (t * 6.0f - 15.0f) + 10.0f);
    }
    public static float fade_derivative(float t)
    {
        return t * t * (t * (t * 5.0f * 6.0f - 4.0f * 15.0f) + 3.0f * 10.0f);
    }
    public static float fade_second_derivative(float t)
    {
        return t * (t * (t * 4.0f * 5.0f * 6.0f - 3.0f * 4.0f * 15.0f) + 2.0f * 3.0f * 10.0f);
    }

    public static float negate_if(float value, uint condition)
    {
        return (condition != 0u) ? -value : value;
    }
    public static float negate_if_derivative(uint condition)
    {
        return (condition != 0u) ? -1 : 1;
    }
    public static float noise_grad(uint hash, float x)
    {
        uint h = hash & 15u;
        float g = 1u + (h & 7u);
        return negate_if(g, h & 8u) * x;
    }

    public static float noise_grad(uint hash, float x, float y)
    {
        uint h = hash & 7u;
        float u = h < 4u ? x : y;
        float v = 2.0f * (h < 4u ? y : x);
        return negate_if(u, h & 1u) + negate_if(v, h & 2u);
    }

    public static float3 noise_grad_derivative(uint hash, float x, float y)
    {
        uint h = hash & 7u;
        byte maskx = h < 4u ? (byte)1u : (byte)2u;
        byte masky = h < 4u ? (byte)2u : (byte)1u;
        float mulx = h < 4u ? 1.0f : 2.0f;
        float muly = h < 4u ? 2.0f : 1.0f;
        if ((h & maskx) != 0u) mulx = -mulx;
        if ((h & masky) != 0u) muly = -muly;
        float r = mulx * x + muly * y;
        return new float3(mulx, muly, r);
    }

    public static float noise_grad(uint hash, float x, float y, float z)
    {
        uint h = hash & 15u;
        float u = h < 8u ? x : y;
        float vt = h==12u || h == 14u ? x : z;
        float v = h < 4u ? y : vt;
        return negate_if(u, h & 1u) + negate_if(v, h & 2u);
    }

    public static float noise_grad(uint hash, float x, float y, float z, float w)
    {
        uint h = hash & 31u;
        float u = h < 24u ? x : y;
        float v = h < 16u ? y : z;
        float s = h < 8u ? z : w;
        return negate_if(u, h & 1u) + negate_if(v, h & 2u) + negate_if(s, h & 4u);
    }

    public static float mix(float v0, float v1, float x)
    {
        return (1 - x) * v0 + x * v1;
    }
    public static float mix_derivative(float v0, float v1, float x)
    {
        return v1 - v0;
    }
    public static float mix(float v0, float v1, float v2, float v3, float x, float y)
    {
        float x1 = 1.0f - x;
        return (1.0f - y) * (v0 * x1 + v1 * x) + y * (v2 * x1 + v3 * x);
    }
    public static float2 mix_derivative(float v0, float v1, float v2, float v3, float x, float y)
    {
        float v = v0 - v1 - v2 + v3;
        return new float2(
            -v0 + v1 + v * y,
            v2 - v0 + v * x
        );
    }
    public static float mix(float v0,
       float v1,
       float v2,
       float v3,
       float v4,
       float v5,
       float v6,
       float v7,
       float x,
       float y,
       float z)
    {
        float x1 = 1.0f - x;
        float y1 = 1.0f - y;
        float z1 = 1.0f - z;
        return z1 * (y1 * (v0 * x1 + v1 * x) + y * (v2 * x1 + v3 * x)) +
            z * (y1 * (v4 * x1 + v5 * x) + y * (v6 * x1 + v7 * x));

    }

    public static float3 mix_derivative(float v0,
        float v1,
        float v2,
        float v3,
        float v4,
        float v5,
        float v6,
        float v7,
        float x,
        float y,
        float z)
    {
        float x1 = 1.0f - x;
        float y1 = 1.0f - y;
        float z1 = 1.0f - z;
        float v01x = v0 * x1 + v1 * x;
        float v23x = v2 * x1 + v3 * x;
        float v45x = v4 * x1 + v5 * x;
        float v67x = v6 * x1 + v7 * x;
        // you can feed this to worfram alpha
        // ( 1.0 - z) * ((1.0 - y) * (v0 * (1.0 - x) + v1 * x) + y * (v2 * (1.0 - x) + v3 * x)) + z* ((1.0 - y) * (v4 * (1.0 - x) + v5 * x) + y * (v6 * (1.0 - x) + v7 * x))
        return new float3(
            z1 * (y1 * (-v0 + v1) + y * (-v2 + v3)) +
            z * (y1 * (-v4 + v5) + y * (-v6 + v7)),
            z1 * (-v01x + v23x) +
            z * (-v45x + v67x),
            -(y1 * v01x + y * v23x) +
            (y1 * v45x + y * v67x)
        );
    }


    /* Quadrilinear Interpolation. */
    public static float mix(float v0,
        float v1,
        float v2,
        float v3,
        float v4,
        float v5,
        float v6,
        float v7,
        float v8,
        float v9,
        float v10,
        float v11,
        float v12,
        float v13,
        float v14,
        float v15,
        float x,
        float y,
        float z,
        float w)
    {
        return mix(mix(v0, v1, v2, v3, v4, v5, v6, v7, x, y, z),
            mix(v8, v9, v10, v11, v12, v13, v14, v15, x, y, z),
            w);
    }


    public static float floor_fraction(float x, ref int i)
    {
        float x_floor = Mathf.Floor(x);
        i = (int)x_floor;
        return x - x_floor;
    }
    public static float perlin_noise(float position)
    {
        int X=0;
        float fx = floor_fraction(position, ref X);
        float u = fade(fx);
        float v0 = noise_grad(hash(X), fx);
        float v1 = noise_grad(hash(X + 1), fx - 1.0f);
        float r = mix(v0, v1, u);
        return r;
    }

    public static float perlin_noise(float2 position)
    {
        int X=0, Y=0;

        float fx = floor_fraction(position.x, ref X);
        float fy = floor_fraction(position.y, ref Y);

        float u = fade(fx);
        float v = fade(fy);

        float r = mix(noise_grad(hash(X, Y), fx, fy),
            noise_grad(hash(X + 1, Y), fx - 1.0f, fy),
            noise_grad(hash(X, Y + 1), fx, fy - 1.0f),
            noise_grad(hash(X + 1, Y + 1), fx - 1.0f, fy - 1.0f),
            u,
            v);

        return r;
    }
    public struct PerlinValue
    {
        public float r;
        public float r_der_x;
        public float r_der_y;
        public float r_der_x_der_x;
        public float r_der_xy;
        public float r_der_y_der_y;
    };

    public static PerlinValue perlin_noise_derivative2(float2 position)
    {
        int X=0, Y=0;
        float fx = floor_fraction(position.x, ref X);
        float fy = floor_fraction(position.y, ref Y);
        float u = fade(fx);
        float v = fade(fy);
        float u_der = fade_derivative(fx);
        float v_der = fade_derivative(fy);
        float u_der_der = fade_second_derivative(fx);
        float v_der_der = fade_second_derivative(fy);


        float3 v0 = noise_grad_derivative(hash(X, Y), fx, fy);
        float3 v1 = noise_grad_derivative(hash(X + 1, Y), fx - 1.0f, fy);
        float3 v2 = noise_grad_derivative(hash(X, Y + 1), fx, fy - 1.0f);
        float3 v3 = noise_grad_derivative(hash(X + 1, Y + 1), fx - 1.0f, fy - 1.0f);
        float r = mix(v0.z, v1.z, v2.z, v3.z, u, v);

        float v1v0 = v1.z - v0.z;
        float v0v2 = v0.z - v2.z;
        float v3v1 = v3.z - v1.z;
        float v3v2 = v3.z - v2.z;
        float nv = v - 1.0f;
        float nu = u - 1.0f;
        // we make use of the property that
        // derivative of f(fx) wrt fx = derivative of f(fx) wrt x
        // from which follows
        // derivative of v0.z wrt x (or y) = derivative of v0.x*x + v0.y*y wrt x (or y)
        // we seek to compute
        // derivative of (1.0 - V(y)) * (v0(x,y) * (1 - U(x)) + v1(x,y) * U(x)) + V(y) * (v2(x,y) * (1 - U(x)) + v3(x,y) * U(x)) wrt x 
        // the notation v0(x,y) stands for v0.z and says that this value depends on x and y. Similarly U(x) stands for u
        // feeding the above formula to wolfram alpha yeilds
        // (1 - V(y)) (-(U(x) - 1) v0^(1, 0)(x, y) + U'(x) (-v0(x, y)) + U(x) v1^(1, 0)(x, y) + U'(x) v1(x, y)) + V(y) (-(U(x) - 1) v2^(1, 0)(x, y) + U'(x) (-v2(x, y)) + U(x) v3^(1, 0)(x, y) + U'(x) v3(x, y))
        // where v0^(1, 0)(x, y) stands for v0.x and U'(x) = u_der thus giving us
        float r_der_x = -nv * (-nu * v0.x + u_der * v1v0 + u * v1.x) + v * (-nu * v2.x + u_der * v3v2 + u * v3.x);
        // now for the y wolfram alpha yields
        // -(V'(y) (U(x) v1(x, y) - (U(x) - 1) v0(x, y))) + V'(y) (U(x) v3(x, y) - (U(x) - 1) v2(x, y)) + (1 - V(y)) (U(x) v1^(0, 1)(x, y) - (U(x) - 1) v0^(0, 1)(x, y)) + V(y) (U(x) v3^(0, 1)(x, y) - (U(x) - 1) v2^(0, 1)(x, y))
        float r_der_y = v_der * (u * v3v1 + nu * v0v2) + -nv * (u * v1.y - nu * v0.y) + v * (u * v3.y - nu * v2.y);

        float v0v2X = v0.x - v2.x;
        float v3v1X = v3.x - v1.x;
        float v1v0X = v1.x - v0.x;
        float v3v2X = v3.x - v2.x; // this is derivative of v3v2 wrt x
        float v3v2Y = v3.y - v2.y; // this is derivative of v3v2 wrt y
        float v1v0Y = v1.y - v0.y; // this is derivative of v1v0 wrt y
        float v3v1Y = v3.y - v1.y;
        float v0v2Y = v0.y - v2.y;

        // to find the second derivative is easy because v0.x and v0.y are constants
        float r_der_x_der_x = -nv * (u_der * 2 * v1v0X + u_der_der * v1v0) + v * (+u_der * 2f * v3v2X + u_der_der * v3v2);
        // because it's just polynomials, all of its derivatives are continuous, therefore r_der_y_der_x=r_der_x_der_y so we just call it r_der_xy and compute only once
        float r_der_xy = v_der * (nu * v0v2X + u_der * (v3v2 - v1v0) + u * v3v1X) - nv * u_der * v1v0Y + v * u_der * v3v2Y;
        float r_der_y_der_y = v_der_der * (u * v3v1 + nu * v0v2) + 2 * v_der * (u * v3v1Y + nu * v0v2Y);
        return new PerlinValue(){r = r,r_der_x = r_der_x, r_der_y = r_der_y,r_der_x_der_x = r_der_x_der_x, r_der_xy = r_der_xy, r_der_y_der_y = r_der_y_der_y };
    }
    public static float3 perlin_noise_derivative(float2 position)
    {
        PerlinValue v = perlin_noise_derivative2(position);
        return new float3(v.r_der_x, v.r_der_y, v.r);
    }
    public static float perlin_noise(float3 position)
    {
        int X=0, Y = 0, Z = 0;

        float fx = floor_fraction(position.x, ref X);
        float fy = floor_fraction(position.y, ref Y);
        float fz = floor_fraction(position.z, ref Z);

        float u = fade(fx);
        float v = fade(fy);
        float w = fade(fz);

        float r = mix(noise_grad(hash(X, Y, Z), fx, fy, fz),
            noise_grad(hash(X + 1, Y, Z), fx - 1, fy, fz),
            noise_grad(hash(X, Y + 1, Z), fx, fy - 1, fz),
            noise_grad(hash(X + 1, Y + 1, Z), fx - 1, fy - 1, fz),
            noise_grad(hash(X, Y, Z + 1), fx, fy, fz - 1),
            noise_grad(hash(X + 1, Y, Z + 1), fx - 1, fy, fz - 1),
            noise_grad(hash(X, Y + 1, Z + 1), fx, fy - 1, fz - 1),
            noise_grad(hash(X + 1, Y + 1, Z + 1), fx - 1, fy - 1, fz - 1),
            u,
            v,
            w);

        return r;
    }
    public static float3 perlin_noise_derivative(float2 position, float distanceScale, float heightScale)
    {
        float3 der_and_height = perlin_noise_derivative(position * distanceScale) * heightScale;
        der_and_height.x *= distanceScale;
        der_and_height.y *= distanceScale;
        return der_and_height;
    }
    public static float perlin_noise(float2 position, float distanceScale, float heightScale)
    {
        return perlin_noise(position * distanceScale) * heightScale;
    }
    public static float3 perlin_noise_derivative(float2 position, float scale)
    {
        float3 der_and_height = perlin_noise_derivative(position * scale);
        der_and_height.z /= scale;
        return der_and_height;
    }
    public static float perlin_noise(float2 position, float scale)
    {
        float height = perlin_noise(position * scale);
        height /= scale;
        return height;
    }

    public static float perlin_noise(float4 position)
    {
        int X=0, Y = 0, Z = 0, W = 0;

        float fx = floor_fraction(position.x, ref X);
        float fy = floor_fraction(position.y, ref Y);
        float fz = floor_fraction(position.z, ref Z);
        float fw = floor_fraction(position.w, ref W);

        float u = fade(fx);
        float v = fade(fy);
        float t = fade(fz);
        float s = fade(fw);

        float r = mix(
            noise_grad(hash(X, Y, Z, W), fx, fy, fz, fw),
            noise_grad(hash(X + 1, Y, Z, W), fx - 1.0f, fy, fz, fw),
            noise_grad(hash(X, Y + 1, Z, W), fx, fy - 1.0f, fz, fw),
            noise_grad(hash(X + 1, Y + 1, Z, W), fx - 1.0f, fy - 1.0f, fz, fw),
            noise_grad(hash(X, Y, Z + 1, W), fx, fy, fz - 1.0f, fw),
            noise_grad(hash(X + 1, Y, Z + 1, W), fx - 1.0f, fy, fz - 1.0f, fw),
            noise_grad(hash(X, Y + 1, Z + 1, W), fx, fy - 1.0f, fz - 1.0f, fw),
            noise_grad(hash(X + 1, Y + 1, Z + 1, W), fx - 1.0f, fy - 1.0f, fz - 1.0f, fw),
            noise_grad(hash(X, Y, Z, W + 1), fx, fy, fz, fw - 1.0f),
            noise_grad(hash(X + 1, Y, Z, W + 1), fx - 1.0f, fy, fz, fw - 1.0f),
            noise_grad(hash(X, Y + 1, Z, W + 1), fx, fy - 1.0f, fz, fw - 1.0f),
            noise_grad(hash(X + 1, Y + 1, Z, W + 1), fx - 1.0f, fy - 1.0f, fz, fw - 1.0f),
            noise_grad(hash(X, Y, Z + 1, W + 1), fx, fy, fz - 1.0f, fw - 1.0f),
            noise_grad(hash(X + 1, Y, Z + 1, W + 1), fx - 1.0f, fy, fz - 1.0f, fw - 1.0f),
            noise_grad(hash(X, Y + 1, Z + 1, W + 1), fx, fy - 1.0f, fz - 1.0f, fw - 1.0f),
            noise_grad(hash(X + 1, Y + 1, Z + 1, W + 1), fx - 1.0f, fy - 1.0f, fz - 1.0f, fw - 1.0f),
            u,
            v,
            t,
            s);

        return r;
    }

    
    public static float prec_correction(float f)
    {
        return Mathf.Abs(f) >= 1000000.0f ? 1f : 0f;
    }
    /* Signed versions of perlin noise in the range [-1, 1]. The scale values were computed
     * experimentally by the OSL developers to remap the noise output to the correct range. */
    public static float perlin_signed(float position)
    {
        return perlin_noise(position) * 0.2500f;
    }

    public static float perlin_signed(float2 position)
    {
        return perlin_noise(position) * 0.6616f;
    }


    public static float perlin_signed(float3 position)
    {
        return perlin_noise(position) * 0.9820f;
    }

    public static float perlin_signed(float4 position)
    {
        return perlin_noise(position) * 0.8344f;
    }

    /* Positive versions of perlin noise in the range [0, 1]. */

    public static float perlin(float position)
    {
        return perlin_signed(position) / 2.0f + 0.5f;
    }

    public static float perlin(float2 position)
    {
        return perlin_signed(position) / 2.0f + 0.5f;
    }

    public static float perlin(float3 position)
    {
        return perlin_signed(position) / 2.0f + 0.5f;
    }

    public static float perlin(float4 position)
    {
        return perlin_signed(position) / 2.0f + 0.5f;
    }


    /* Fractal perlin noise. */
    public static float perlin_multi_fractal(float3 p, int iterations, float roughness, float lacunarity)
    {
        float value = 1.0f;
        float pwr = 1.0f;

        while(iterations-->0)
        {
            value *= (pwr * perlin_signed(p) + 1.0f);
            pwr *= roughness;
            p *= lacunarity;
        }
        return value;
    }


    /* The following offset functions generate random offsets to be added to
     * positions to act as a seed since the noise functions don't have seed values.
     * The offset's components are in the range [100, 200], not too high to cause
     * bad precision and not too small to be noticeable. We use float seed because
     * OSL only supports float hashes and we need to maintain compatibility with it.
     */

    public static float random_float_offset(float seed)
    {
        return 100.0f + hash_float_to_float(seed) * 100.0f;
    }

    public static float2 random_float2_offset(float seed)
    {
        return new float2(100.0f + hash_float_to_float(new float2(seed, 0.0f)) * 100.0f,
            100.0f + hash_float_to_float(new float2(seed, 1.0f)) * 100.0f);
    }

    public static float3 random_float3_offset(float seed)
    {
        return new float3(100.0f + hash_float_to_float(new float2(seed, 0.0f)) * 100.0f,
            100.0f + hash_float_to_float(new float2(seed, 1.0f)) * 100.0f,
            100.0f + hash_float_to_float(new float2(seed, 2.0f)) * 100.0f);
    }

    public static float4 random_float4_offset(float seed)
    {
        return new float4(100.0f + hash_float_to_float(new float2(seed, 0.0f)) * 100.0f,
            100.0f + hash_float_to_float(new float2(seed, 1.0f)) * 100.0f,
            100.0f + hash_float_to_float(new float2(seed, 2.0f)) * 100.0f,
            100.0f + hash_float_to_float(new float2(seed, 3.0f)) * 100.0f);
    }

    /* Perlin noises to be added to the position to distort other noises. */

    public static float perlin_distortion(float position, float strength)
    {
        return perlin_signed(position + random_float_offset(0.0f)) * strength;
    }

    public static float2 perlin_distortion(float2 position, float strength)
    {
        return new float2(perlin_signed(position + random_float2_offset(0.0f)) * strength,
            perlin_signed(position + random_float2_offset(1.0f)) * strength);
    }

    public static float3 perlin_distortion(float3 position, float strength)
    {
        return new float3(perlin_signed(position + random_float3_offset(0.0f)) * strength,
            perlin_signed(position + random_float3_offset(1.0f)) * strength,
            perlin_signed(position + random_float3_offset(2.0f)) * strength);
    }

    public static float4 perlin_distortion(float4 position, float strength)
    {
        return new float4(perlin_signed(position + random_float4_offset(0.0f)) * strength,
            perlin_signed(position + random_float4_offset(1.0f)) * strength,
            perlin_signed(position + random_float4_offset(2.0f)) * strength,
            perlin_signed(position + random_float4_offset(3.0f)) * strength);
    }

    /* Distorted fractal perlin noise. */




    public static float3 random_vector(ref float3 min_value, ref float3 max_value, uint id, uint seed)
    {
        float x = hash_to_float(seed, id, 0);
        float y = hash_to_float(seed, id, 1);
        float z = hash_to_float(seed, id, 2);
        return new float3(x, y, z) * (max_value - min_value) + min_value;
    }
    public static float random_float(float min_value, float max_value, uint id, uint seed)
    {
        float value = hash_to_float(seed, id);
        return value * (max_value - min_value) + min_value;
    }
    public static int random_int(int min_value, int max_value, uint id, uint seed)
    {
        float value = hash_to_float(id, seed);
        /* Add one to the maximum and use floor to produce an even
         * distribution for the first and last values (See #93591). */
        return Mathf.FloorToInt(value * (max_value + 1 - min_value) + min_value);
    }
    public static bool random_bool(float probability, uint id, uint seed)
    {
        return hash_to_float(id, seed) <= probability;
    }
    public static float3 morenoise(float2 position, float scale, float pointiness, float scalePowerBase, int iterations)
    {
        float3 der_and_height = morenoise(position / scale, pointiness, scalePowerBase, iterations);
        der_and_height.x /= scale;
        der_and_height.y /= scale;
        return der_and_height;
    }
    public static float3 morenoise(float2 position, float pointiness, float scalePowerBase, int iterations)
    {
        float2 c = new float2(0f, 0f);//cummulative first order derivative of perlin noise
        float cX_der_x = 0;
        float cY_der_y = 0;
        float cY_der_x = 0;//==cX_der_y
        float3 o = new float3(0, 0, 0);
        float scale = 1;

        while (iterations-- > 0)
        {

            PerlinValue v = perlin_noise_derivative2(position * scale);
            v.r += 1f;
            v.r /= scale;

            c += new float2(v.r_der_x, v.r_der_y);
            float erosion = (1f + pointiness * math.dot(c, c));
            o.z += v.r / erosion;

            // derivative of v.r / erosion wrt x = 
            // (v.r_der_x * erosion - v.r * (derivative of erosion wrt x)) / (erosion*erosion)
            // where
            // derivative of erosion wrt x = derivative of pointiness * (c.x*c.x + c.y*c.y) wrt x =
            // pointiness * 2 * (c.x* (der of c.x wrt x) + c.y* (der of c.y wrt x)) 
            // where
            // der of c.x wrt x = der of (v1.x + v2.x + ... vn.x) wrt x =
            // = der of v1.x wrt x  +  der of v2.x wrt x  + ... + der of vn.x wrt x =
            // = der of v1.x wrt x  +  der of v2.x wrt x  + ... + der of vn.x wrt x =
            cX_der_x += v.r_der_x_der_x;
            cY_der_x += v.r_der_xy;
            cY_der_y += v.r_der_y_der_y;
            float erosion_der_x = 2f * pointiness * (c.x * cX_der_x + c.y * cY_der_x);
            float erosion_der_y = 2f * pointiness * (c.x * cY_der_x + c.y * cY_der_y);
            o.x += (v.r_der_x * erosion - v.r * erosion_der_x) / (erosion * erosion);
            o.y += (v.r_der_y * erosion - v.r * erosion_der_y) / (erosion * erosion);
            scale *= scalePowerBase;
        }
        return o;
    }


    public static float perlin_fbm(float2 position, float fscale, float height,  float heightPowerBase,  float scalePowerBase, int iterations) {
        float sum = 0;
        while (iterations-- > 0) {
            float v = perlin_noise(position, fscale, height);
            sum += v;
            height *= heightPowerBase;
            fscale *= scalePowerBase;
        }
        return sum;
    }
    public static float3 perlin_fbm_derivative(float2 position, float fscale, float height, float heightPowerBase, float scalePowerBase, int iterations) {
        float3 sum = new float3(0, 0, 0);
        while (iterations-- > 0) {
            float3 v = perlin_noise_derivative(position, fscale, height);
            sum += v;
            height *= heightPowerBase;
            fscale *= scalePowerBase;
        }
        return sum;
    }

    public static void distribute_points_on_faces(Vector3[] vertices, Vector3[] normals, int[] triangles, Func<float3, int, float> densityFunction, Action<float3, float3, int> resultCollectorFunction, uint seed) {

        for (int tri_i = 0, k=0; tri_i < triangles.Length; tri_i +=3) {
            int v0_loop = triangles[tri_i];
            int v1_loop = triangles[tri_i +1];
            int v2_loop = triangles[tri_i + 2];
            float3 v0_pos = vertices[v0_loop];
            float3 v1_pos = vertices[v1_loop];
            float3 v2_pos = vertices[v2_loop];
            float3 v0_nor = normals[v0_loop];
            float3 v1_nor = normals[v1_loop];
            float3 v2_nor = normals[v2_loop];
            float v0_density_factor = Mathf.Max(0.0f, densityFunction(v0_pos, v0_loop));
            float v1_density_factor = Mathf.Max(0.0f, densityFunction(v1_pos, v1_loop));
            float v2_density_factor = Mathf.Max(0.0f, densityFunction(v2_pos, v2_loop));
            float corner_tri_density_factor = (v0_density_factor + v1_density_factor + v2_density_factor) / 3.0f;
            
            float area = BMath.area_tri(v0_pos, v1_pos, v2_pos);
            
            uint corner_tri_seed = hash((uint)tri_i, seed);
            RandomNumberGenerator corner_tri_rng = new RandomNumberGenerator(corner_tri_seed);

            int point_amount = corner_tri_rng.round_probabilistic(area * corner_tri_density_factor);

            for (int i = 0; i < point_amount; i++, k++) {
                float3 bary_coord = corner_tri_rng.get_barycentric_coordinates();
                
                float3 point_pos = BMath.interp_v3_v3v3v3(v0_pos, v1_pos, v2_pos, bary_coord);
                float3 point_nor = BMath.interp_v3_v3v3v3(v0_nor, v1_nor, v2_nor, bary_coord);
                resultCollectorFunction(point_pos,point_nor,k);
            }
        }
    }

}
