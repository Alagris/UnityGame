//
// Description : Array and textureless GLSL 2D simplex noise function.
//      Author : Ian McEwan, Ashima Arts.
//  Maintainer : stegu
//     Lastmod : 20110822 (ijm)
//     License : Copyright (C) 2011 Ashima Arts. All rights reserved.
//               Distributed under the MIT License. See LICENSE file.
//               https://github.com/ashima/webgl-noise
//               https://github.com/stegu/webgl-noise
//

//
// This code has been modified by Keijiro Takahashi for use in Unity,
// including a rewrite in HLSL with simplifications and optimizations.
// Rights to the modifications are waived, and the original license
// terms remain unchanged.
//

#ifndef _INCLUDE_JP_KEIJIRO_NOISESHADER_SIMPLEX_NOISE_2D_HLSL_
#define _INCLUDE_JP_KEIJIRO_NOISESHADER_SIMPLEX_NOISE_2D_HLSL_

#include "Common.hlsl"

float3 SimplexNoise2DGrad(float2 v)
{
    const float C1 = (3 - sqrt(3)) / 6;
    const float C2 = (sqrt(3) - 1) / 2;

    // First corner
    float2 i  = floor(v + dot(v, C2));
    float2 x0 = v -   i + dot(i, C1);

    // Other corners
    float2 i1 = x0.x > x0.y ? float2(1, 0) : float2(0, 1);
    float2 x1 = x0 + C1 - i1;
    float2 x2 = x0 + C1 * 2 - 1;

    // Permutations
    i = wglnoise_mod289(i); // Avoid truncation effects in permutation
    float3 p = wglnoise_permute(    i.y + float3(0, i1.y, 1));
           p = wglnoise_permute(p + i.x + float3(0, i1.x, 1));

    // Gradients: 41 points uniformly over a unit circle.
    // The ring size 17*17 = 289 is close to a multiple of 41 (41*7 = 287)
    float3 phi = p / 41 * 3.14159265359 * 2;
    float2 g0 = float2(cos(phi.x), sin(phi.x));
    float2 g1 = float2(cos(phi.y), sin(phi.y));
    float2 g2 = float2(cos(phi.z), sin(phi.z));

    // Compute noise and gradient at P
    float3 m  = float3(dot(x0, x0), dot(x1, x1), dot(x2, x2));
    float3 px = float3(dot(g0, x0), dot(g1, x1), dot(g2, x2));

    m = max(0.5 - m, 0);
    float3 m3 = m * m * m;
    float3 m4 = m * m3;

    float3 temp = -8 * m3 * px;
    float2 grad = m4.x * g0 + temp.x * x0 +
                  m4.y * g1 + temp.y * x1 +
                  m4.z * g2 + temp.z * x2;

    return 99.2 * float3(grad, dot(m4, px));
}

float SimplexNoise2D(float2 v)
{
    return SimplexNoise2DGrad(v).z;
}

float SimplexNoise2DFbm(float2 x, float frequencyChange, float amplitudeChange, float2 shift, float rotAngle, int octaves) {
    float v = 0.0;
    float frequency = frequencyChange;
    float amplitude = 1;
    //float2 shift = float2(100.0, 0.0);
    // Rotate to reduce axial bias
    float2x2 rot = float2x2(cos(rotAngle), sin(rotAngle), -sin(rotAngle), cos(rotAngle));
    for (int i = 0; i < octaves; ++i) {

        v += amplitude * SimplexNoise2D(x);
        x *= frequency;
        // float2 r = mul(rot, x.xz); 
         //x.xz = r * 2.0 + shift;
        frequency *= frequencyChange;
        amplitude *= amplitudeChange;
    }
    return v;
}
#endif
