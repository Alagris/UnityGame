Shader "Custom/TransparentAnimeShader"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        //[MainTexture] _BaseMap("Base Map", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
           

            struct Attributes
            {
                float4 positionOS : POSITION;
                //float3 normal : NORMAL;
                //float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                //float2 uv : TEXCOORD0;
                //half3 worldNormal: NORMAL;
            };

            sampler2D _BaseMap;
            

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                //float4 _BaseMap_ST;
            CBUFFER_END

            

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = UnityObjectToClipPos(IN.positionOS);
                //OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                //OUT.worldNormal = UnityObjectToWorldNormal(IN.normal);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                
                return _BaseColor;
            }
            ENDHLSL
        }
    }
}
