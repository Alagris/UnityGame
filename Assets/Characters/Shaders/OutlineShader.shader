Shader "Custom/OutlineShader"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        [BlendFactor] _BlendFactor("Blend Factor", float) = 0.5
        [OutlineThickness] _OutlineThickness("Outline Thickness", float) = 0.1
        [Toggle] _HAS_FACE ("Has face", Integer) = 0
        [FaceOutlineThickness] _FaceOutlineThickness("Face Outline Thickness", float) = 0.1
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        ZWrite On // if this is Off, the outline won't appear on sky background. TODO: figure out if there is a better way around this
        Cull Front 

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _HAS_FACE_ON
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
            };

            sampler2D _BaseMap;
            

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
                float _BlendFactor;
                float _OutlineThickness;
                float _FaceOutlineThickness;
            CBUFFER_END

            

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                float thickness = _OutlineThickness;
                #if _HAS_FACE_ON
                {
                    bool isFace = OUT.uv.x<0.5 && OUT.uv.y>0.5;
                    thickness = isFace ? _FaceOutlineThickness : thickness;
                    //lLerp = isFace ? faceColor : lLerp;
                }
                #endif
                OUT.positionHCS = UnityObjectToClipPos(IN.positionOS + IN.normal * thickness);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                
                half4 color = tex2D(_BaseMap, IN.uv) ;
                half4 blended = color * _BlendFactor + _BaseColor * (1.0-_BlendFactor); 
                return blended;
            }
            ENDHLSL
        }
    }
}
