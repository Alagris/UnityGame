#ifndef _INCLUDE_VORONOI_HLSL_
#define _INCLUDE_VORONOI_HLSL_

inline float2 randomVector(float2 UV, float offset)
{
    float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
    UV = frac(sin(mul(UV, m)) * 46839.32);
    return float2(sin(UV.y * +offset) * 0.5 + 0.5, cos(UV.x * offset) * 0.5 + 0.5);
}

// Based on code by Inigo Quilez: https://iquilezles.org/articles/voronoilines/
//returns float2(DistFromCenter, DistFromEdge);
float2 Voronoi(float2 UV, float AngleOffset)
{
    int2 cell = floor(UV);
    float2 posInCell = frac(UV);
    
    float DistFromCenter = 8.0f;
    float2 closestOffset;

    for (int y = -1; y <= 1; ++y)
    {
        for (int x = -1; x <= 1; ++x)
        {
            int2 cellToCheck = int2(x, y);
            float2 cellOffset = float2(cellToCheck) - posInCell + randomVector(cell + cellToCheck, AngleOffset);

            float distToPoint = dot(cellOffset, cellOffset);

            if (distToPoint < DistFromCenter)
            {
                DistFromCenter = distToPoint;
                closestOffset = cellOffset;
            }
        }
    }

    float DistFromEdge = 8.0f;

    for (int y = -1; y <= 1; ++y)
    {
        for (int x = -1; x <= 1; ++x)
        {
            int2 cellToCheck = int2(x, y);
            float2 cellOffset = float2(cellToCheck) - posInCell + randomVector(cell + cellToCheck, AngleOffset);

            float distToEdge = dot(0.5f * (closestOffset + cellOffset), normalize(cellOffset - closestOffset));

            DistFromEdge = min(DistFromEdge, distToEdge);
        }
    }
    return float2(DistFromCenter, DistFromEdge);
}
#endif
