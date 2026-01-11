Shader "Custom/LineDebugShader"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 0, 0, 1)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        //ZWrite On 
        

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
           

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };


            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
            CBUFFER_END

            

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = UnityObjectToClipPos(IN.positionOS);
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
