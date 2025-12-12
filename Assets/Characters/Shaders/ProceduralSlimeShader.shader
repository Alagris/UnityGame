Shader "Custom/ProceduralSlimeShader"
{
    Properties
    {
        [MainBottomColor] _MainBottomColor("Bottom Color", Color) = (0.380285, 0, 0.589085, 1)
        [MainTopColor] _MainTopColor("Top Color", Color) = (1.0, 0.229825, 0, 1)
        [EyeBottomColor] _EyeBottomColor("Eye Bottom Color", Color) = (0.588239, 0, 0.223527, 1)
        [EyeTopColor] _EyeTopColor("Eye Top Color", Color) = (0.870652, 0.755421, 1.0, 1)
        [EyeOutlineColor] _EyeOutlineColor("Eye Outline Color", Color) = (0.475382, 0, 0.255606, 1)
        [ShadowColor] _ShadowColor("Shadow Color", Color) = (0.3, 0.3, 0.3, 1)
        [HalfShadowColor] _HalfShadowColor("Half Shadow Color", Color) = (0.6, 0.6, 0.6, 1)
        [LightSmooth] _LightSmooth("Light Smoothing", Range(0.0,1.0)) = 0.1
        [MidBand] _MidBand("Mid Band lenght", Range(0.0,1.0)) = 0.1
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
           

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
                half3 worldNormal: NORMAL;
            };

            

            CBUFFER_START(UnityPerMaterial)

                half4 _MainBottomColor;
                half4 _MainTopColor;
                half4 _EyeBottomColor;
                half4 _EyeTopColor;
                half4 _EyeOutlineColor;
                half4 _ShadowColor;
                half4 _HalfShadowColor;
                float _LightSmooth;
                float _MidBand;
            CBUFFER_END

            

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = UnityObjectToClipPos(IN.positionOS);
                OUT.uv = IN.uv;
                OUT.worldNormal = UnityObjectToWorldNormal(IN.normal);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float lDot = dot(normalize(IN.worldNormal), normalize(_WorldSpaceLightPos0.xyz));
                float lSmooth = smoothstep(0, _LightSmooth, lDot);
                half4 color = lerp(_MainBottomColor, _MainTopColor, IN.uv.x) ; 
                half4 lLerp = lDot<_LightSmooth+_MidBand ? lerp(_ShadowColor, _HalfShadowColor, lSmooth) : 1;
                half4 shadedColor = color * lLerp;
                half4 eyeColor = IN.uv.x < 0.125 ? _EyeOutlineColor : lerp(_EyeBottomColor, _EyeTopColor, abs(IN.uv.x-0.5));
                half4 proceduralColor = IN.uv.y < 0.75 ? shadedColor : eyeColor;
                return proceduralColor;
            }
            ENDHLSL
        }
    }
}
