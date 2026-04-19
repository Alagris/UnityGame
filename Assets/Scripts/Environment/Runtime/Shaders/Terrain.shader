// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Proc Env/Terrain"
{
    Properties
    {
        _DiffuseMap ("Diffuse", 2DArray) = "" {}
        _NormalMap ("Normal", 2DArray) = "" {}
        _Ambient ("Ambient", Color) = (0.1, 0.1, 0.1, 1)
        _LayerWeights("Weight Map", 2D) = "white" {}
        _LightSmooth("Light Smoothing", Range(0.0,1.0)) = 0.1
        _UVScale ("UVScale", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry"}
        ZWrite On
        ZTest LEqual 
        Stencil
        {
             Ref 221
             Comp Always
             Pass Replace
        }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma require 2darray
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"   
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            

            CBUFFER_START(UnityPerMaterial)
                half4 _Ambient;
                float _UVScale;
                float _LightSmooth;
            CBUFFER_END

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
                float3 worldNormal: NORMAL;
            };

            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = IN.uv;
                OUT.positionWorld = IN.positionOS;
                OUT.worldNormal = TransformObjectToWorldNormal(IN.normal);
                
                return OUT;
            }
            
            TEXTURE2D_ARRAY(_DiffuseMap);
            SAMPLER(sampler_DiffuseMap);
            TEXTURE2D_ARRAY(_NormalMap);
            SAMPLER(sampler_NormalMap);
            sampler2D _LayerWeights;

            float2 mix2d(float2 v0, float2 v1, float2 v2, float2 v3, float x, float y)
            {
                float x1 = 1.0f - x;
                return (1.0f - y) * (v0 * x1 + v1 * x) + y * (v2 * x1 + v3 * x);
            }
            half4 frag (Varyings IN) : SV_Target
            {
                float2 worldUV = IN.positionWorld.xz * _UVScale;
                float layerIdx = 2;
                VertexPositionInputs positions = GetVertexPositionInputs(IN.positionWorld);
                float4 shadowCoords = GetShadowCoord(positions);
                half shadowAmount = MainLightRealtimeShadow(shadowCoords); 
                Light mainLight = GetMainLight();
                float lightDot = dot(normalize(IN.worldNormal), normalize(-mainLight.direction)) ;
                float lightSmooth = smoothstep(0, _LightSmooth, lightDot);
                half3 diffuse = SAMPLE_TEXTURE2D_ARRAY(_DiffuseMap, sampler_DiffuseMap, worldUV, layerIdx) ;

                half4 normal = SAMPLE_TEXTURE2D_ARRAY(_NormalMap, sampler_NormalMap, worldUV, layerIdx);
                half3 lighting = max(mainLight.color, _Ambient) * diffuse * lerp(_LightSmooth, 1, shadowAmount); 
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
                return half4(lighting, 1);
            }
            ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            // -------------------------------------
            // Render State Commands
            ZWrite On
            ZTest LEqual
            ColorMask 0
            // Cull[_Cull]

            HLSLPROGRAM
            #pragma target 2.0

            // -------------------------------------
            // Shader Stages
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

            // -------------------------------------
            // Universal Pipeline keywords

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ LOD_FADE_CROSSFADE

            // This is used during shadow map generation to differentiate between directional and punctual light shadows, as they use different formulas to apply Normal Bias
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

            // -------------------------------------
            // Includes
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }
        
    }
}
