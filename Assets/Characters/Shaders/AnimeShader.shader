Shader "Custom/AnimeShader"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [ShadowColor] _ShadowColor("Shadow Color", Color) = (0.3, 0.3, 0.3, 1)
        [HalfShadowColor] _HalfShadowColor("Half Shadow Color", Color) = (0.6, 0.6, 0.6, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
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

            sampler2D _BaseMap;
            

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _ShadowColor;
                half4 _HalfShadowColor;
                float4 _BaseMap_ST;
                float _LightSmooth;
                float _MidBand;
            CBUFFER_END

            

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = UnityObjectToClipPos(IN.positionOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                OUT.worldNormal = UnityObjectToWorldNormal(IN.normal);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float lDot = dot(normalize(IN.worldNormal), normalize(_WorldSpaceLightPos0.xyz));
                float lSmooth = smoothstep(0, _LightSmooth, lDot);
                half4 color = tex2D(_BaseMap, IN.uv) ; 
                half4 lLerp = lDot<_LightSmooth+_MidBand ? lerp(_ShadowColor, _HalfShadowColor, lSmooth) : _BaseColor;
                half4 shadedColor = color * lLerp;
                return shadedColor;
            }
            ENDHLSL
        }
    }
}
