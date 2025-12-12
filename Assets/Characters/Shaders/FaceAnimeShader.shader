Shader "Custom/FaceAnimeShader"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [ShadowColor] _ShadowColor("Shadow Color", Color) = (0.3, 0.3, 0.3, 1)
        [HalfShadowColor] _HalfShadowColor("Half Shadow Color", Color) = (0.6, 0.6, 0.6, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        [LightSmooth] _LightSmooth("Light Smoothing", Range(0.0,1.0)) = 0.1
        [FaceLightSmooth] _FaceLightSmooth("Face Light Smoothing", Range(0.0,1.0)) = 0.1
        [MidBand] _MidBand("Mid Band lenght", Range(0.0,1.0)) = 0.01
        [FaceMidBand] _FaceMidBand("Face Mid Band lenght", Range(0.0,1.0)) = 0.01
        [FaceShadowAngle] _FaceShadowAngle("FaceShadowAngle", Float) = 0.0
        [Toggle] _FACE_SDF ("Use face SDF", Integer) = 0
        [FaceSdfMap] _FaceSdfMap("Face SDF Map", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _FACE_SDF_ON
            #pragma shader_feature _FACE_UV_CORNER_LEFT_ON
            #pragma shader_feature _FACE_UV_CORNER_TOP_ON
            
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

            UNITY_DECLARE_TEX2D(_BaseMap);
            UNITY_DECLARE_TEX2D_NOSAMPLER(_FaceSdfMap);
            

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _ShadowColor;
                half4 _HalfShadowColor;
                float4 _BaseMap_ST;
                float _LightSmooth;
                float _FaceLightSmooth;
                float _MidBand;
                float _FaceMidBand;
                float _FACE_UV_CORNER_SIZE;
                float _FaceShadowAngle;
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
                
                half4 skinDiffuse = UNITY_SAMPLE_TEX2D(_BaseMap, IN.uv) ; 
                float lightSmoothing = _LightSmooth;
                float midBand = _MidBand;
                
                #if _FACE_SDF_ON
                {
                    float relativeX = IN.uv.x*2.0;
                    float relativeY = (IN.uv.y-0.5)*2.0;
                    bool isFace = IN.uv.x<0.5 && IN.uv.y>0.5;
                  //  _FaceShadowAngle = -_FaceShadowAngle;
                    bool flipX = frac(_FaceShadowAngle) > 0.5;
                    float flippedX = flipX ? -relativeX : relativeX;
                    float threshold = abs(flipX - frac(_FaceShadowAngle*2.0));
                    half4 faceSdf = UNITY_SAMPLE_TEX2D_SAMPLER(_FaceSdfMap,_BaseMap,float2(flippedX,relativeY));
                    float faceLightIntensity = faceSdf.x-threshold;
                    //half4 faceColor = faceSdf>threshold ? _BaseColor : _ShadowColor;
                    lDot = isFace ? faceLightIntensity : lDot;
                    lightSmoothing  = isFace ? _FaceLightSmooth : lightSmoothing ;
                    midBand = isFace ? _FaceMidBand : midBand;
                    //lLerp = isFace ? faceColor : lLerp;
                }
                #endif
                float lightIntensity = smoothstep(0, _LightSmooth, lDot);
                half4 lLerp = lDot<lightSmoothing+ midBand? lerp(_ShadowColor, _HalfShadowColor, lightIntensity) : _BaseColor;
                half4 shadedColor = skinDiffuse * lLerp;
                return shadedColor;
            }
            ENDHLSL
        }
    }
}
