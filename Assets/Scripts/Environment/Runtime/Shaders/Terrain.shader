// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Proc Env/Terrain"
{
    Properties
    {
        _TerrainLayers ("Layers", 2DArray) = "" {}
        _LayerWeights("Weight Map", 2D) = "white" {}
        _UVScale ("UVScale", Float) = 1.0
    }
    SubShader
    {
         Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma require 2darray
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWorld : TEXCOORD1;
                float4 shadowCoords : TEXCOORD3;
                float3 normal: NORMAL;
            };

            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = IN.uv;
                OUT.positionWorld = IN.positionOS;
                OUT.normal = IN.normal;
                VertexPositionInputs positions = GetVertexPositionInputs(IN.positionOS.xyz);
                float4 shadowCoordinates = GetShadowCoord(positions);
                OUT.shadowCoords = shadowCoordinates;
                return OUT;
            }
            
            TEXTURE2D_ARRAY(_TerrainLayers);
            SAMPLER(sampler_TerrainLayers);
            sampler2D _LayerWeights;

            float2 mix2d(float2 v0, float2 v1, float2 v2, float2 v3, float x, float y)
            {
                float x1 = 1.0f - x;
                return (1.0f - y) * (v0 * x1 + v1 * x) + y * (v2 * x1 + v3 * x);
            }
            half4 frag (Varyings IN) : SV_Target
            {
                half shadowAmount = MainLightRealtimeShadow(IN.shadowCoords); 
                /*
                float2 bottomLeft = floor(IN.uv);
                float2 bottomRight = bottomLeft+float2(1,0);
                float2 topLeft = bottomLeft+float(0,1);
                float2 topRight = bottomLeft+float2(1,1);
                const float2 res = flaot2(34,34);

                float2 uvNormalised = IN.uv - bottomLeft;

                float2 bottomLeftNormalised = bottomLeft/res;
                float2 bottomRightNormalised = bottomRight/res;
                float2 topLeftNormalised = topLeft/res;
                float2 topRightNormalised = topRight/res;

                float4 bottomLeftColor = tex2D(_LayerWeights, bottomLeftNormalised);
                float4 bottomRightColor = tex2D(_LayerWeights, bottomRightNormalised);
                float4 topLeftColor = tex2D(_LayerWeights, topLeftNormalised);
                float4 topRightColor = tex2D(_LayerWeights, topRightNormalised);

                mix2d(bottomLeftColor.zw, bottomRightColor.zw, topLeftColor.zw, topRightColor.zw);
                */
                return SAMPLE_TEXTURE2D_ARRAY(_TerrainLayers, sampler_TerrainLayers, IN.positionWorld.xz, 2) * shadowAmount;
            }
            ENDHLSL
        }
        
    }
}
