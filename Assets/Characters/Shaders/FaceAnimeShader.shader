Shader "Custom/FaceAnimeShader"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)

        [Toggle] _SHADOW ("Enable shadows", Integer) = 0
        [ShadowColor] _ShadowColor("Shadow Color", Color) = (0.3, 0.3, 0.3, 1)
        [HalfShadowColor] _HalfShadowColor("Half Shadow Color", Color) = (0.6, 0.6, 0.6, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        [LightSmooth] _LightSmooth("Light Smoothing", Range(0.0,1.0)) = 0.1
        [FaceLightSmooth] _FaceLightSmooth("Face Light Smoothing", Range(0.0,1.0)) = 0.1
        [MidBand] _MidBand("Mid Band lenght", Range(0.0,1.0)) = 0.01

        [Toggle] _FACE_SDF ("Use face SDF", Integer) = 0
        [FaceSdfMap] _FaceSdfMap("Face SDF Map", 2D) = "white" {}
        [FaceMidBand] _FaceMidBand("Face Mid Band lenght", Range(0.0,1.0)) = 0.01
        [FaceShadowAngle] _FaceShadowAngle("FaceShadowAngle", Float) = 0.0

        [Toggle] _RIM_LIGHT ("Use Rim Light", Integer) = 0
        [RimLightColor] _RimLightColor("Rim Light Color", Color) = (1, 0, 0, 1)
        [RimLightPower] _RimLightPower("Rim Light Power", Float) = 1.5
        [Toggle] _RIM_LIGHT_SMOOTHSTEP ("Use Rim Light Smoothstep", Integer) = 0
        [RimLightFalloff] _RimLightFalloff("Rim Light Falloff", Float) = 0
        [Toggle] _RIM_LIGHT_ON_SHADOW_SIDE ("Show rim light on shaded side too", Integer) = 0

        [Toggle] _FRESNEL_SPECULAR_HIGHLIGHT ("Use Fresnel Specular Highlight", Integer) = 0
        [SpecularColor] _SpecularColor("Specular Color", Color) = (1, 0, 0, 1)
        [SpecularStrength] _SpecularStrength("Specular Strength", Float) = 0.9
        [SpecularFalloff] _SpecularFalloff("Specular Falloff", Float) = 1
        [Toggle] _DISABLE_FRESNEL_SPECULAR_HIGHLIGHT_ON_HAIR ("Disable Fresnel Specular Highlight on Hair", Integer) = 0
        [Toggle] _DISABLE_FRESNEL_SPECULAR_HIGHLIGHT_ON_FACE ("Disable Fresnel Specular Highlight on Face", Integer) = 0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _SHADOW_ON
            #pragma shader_feature _FACE_SDF_ON
            #pragma shader_feature _RIM_LIGHT_ON
            #pragma shader_feature _RIM_LIGHT_SMOOTHSTEP_ON
            #pragma shader_feature _RIM_LIGHT_ON_SHADOW_SIDE_ON
            #pragma shader_feature _FRESNEL_SPECULAR_HIGHLIGHT_ON
            #pragma shader_feature _DISABLE_FRESNEL_SPECULAR_HIGHLIGHT_ON_HAIR_ON
            #pragma shader_feature _DISABLE_FRESNEL_SPECULAR_HIGHLIGHT_ON_FACE_ON
            
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
                half3 viewDir: TEXCOORD1;
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
                half4 _RimLightColor;
                float _RimLightPower;
                float _RimLightFalloff;
                half4 _SpecularColor;
                float _SpecularStrength;
                float _SpecularFalloff;

            CBUFFER_END

            

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = UnityObjectToClipPos(IN.positionOS);
                OUT.uv = IN.uv;
                OUT.worldNormal = UnityObjectToWorldNormal(IN.normal);
               // float3 viewNormal = mul(UNITY_MATRIX_MV, IN.normal).xyz;
               // float3 viewPos = mul(UNITY_MATRIX_MV, IN.positionOS).xyz;
               // float3 viewDir = 
                OUT.viewDir = WorldSpaceViewDir(IN.positionOS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                //float3 worldSpaceViewDir = _WorldSpaceCameraPos.xyz - worldPos;
                bool isHair = IN.uv.x>0.5 && IN.uv.y>0.5;
                bool isFace = IN.uv.x<0.5 && IN.uv.y>0.5;
                bool isFaceOrHair = IN.uv.y>0.5;

                float lDot = dot(normalize(IN.worldNormal), normalize(_WorldSpaceLightPos0.xyz));
                
                half4 skinDiffuse = UNITY_SAMPLE_TEX2D(_BaseMap, IN.uv) ; 
                float lightSmoothing = _LightSmooth;
                float midBand = _MidBand;
                
                #if _FACE_SDF_ON
                {
                    float relativeX = IN.uv.x*2.0;
                    float relativeY = (IN.uv.y-0.5)*2.0;
                    
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

                half4 shadedColor = skinDiffuse;
                bool isShadow = false;
                #if _SHADOW_ON
                {
                    float lightIntensity = smoothstep(0, _LightSmooth, lDot);
                    isShadow = lDot<lightSmoothing+ midBand;
                    half4 lLerp = isShadow ? lerp(_ShadowColor, _HalfShadowColor, lightIntensity) : _BaseColor;
                    shadedColor *= lLerp;
                }
                #endif
                

                #if _RIM_LIGHT_ON || _FRESNEL_SPECULAR_HIGHLIGHT_ON
                {
                    float rimLightIntensity = dot(normalize(IN.viewDir), normalize(IN.worldNormal));
                    #if _FRESNEL_SPECULAR_HIGHLIGHT_ON
                    {
                        float specularStrength = smoothstep(_SpecularStrength, _SpecularFalloff, rimLightIntensity);
                        
                        #if _DISABLE_FRESNEL_SPECULAR_HIGHLIGHT_ON_HAIR_ON
                        {
                            #if _DISABLE_FRESNEL_SPECULAR_HIGHLIGHT_ON_FACE_ON
                            {
                                specularStrength = isFaceOrHair ? 0 : specularStrength;
                            }
                            #else
                            {
                                specularStrength = isHair ? 0 : specularStrength;
                            }
                            #endif
                        }
                        #else
                        {
                            #if _DISABLE_FRESNEL_SPECULAR_HIGHLIGHT_ON_FACE_ON
                            {
                                specularStrength = isFace ? 0 : specularStrength;
                            }
                            #endif
                        }
                        #endif
                        half4 specularColor = specularStrength * _SpecularColor;
                        shadedColor+=specularColor;
                    }
                    #endif
                    //rimLightIntensity = max(0.0, rimLightIntensity);
                    rimLightIntensity = 1.0 - rimLightIntensity;
                    #if _RIM_LIGHT_SMOOTHSTEP_ON
                    {
                        rimLightIntensity = smoothstep(_RimLightPower, _RimLightPower+_RimLightFalloff, rimLightIntensity);
                    }
                    #else
                    {
                        
                        rimLightIntensity = pow(rimLightIntensity, _RimLightPower);
                    }
                    #endif
                    #if !_RIM_LIGHT_ON_SHADOW_SIDE_ON
                    {
                        rimLightIntensity=isShadow?rimLightIntensity:0;
                    }
                    #endif
                    half4 rimLight = rimLightIntensity * _RimLightColor;
                    shadedColor+=rimLight;
                }
                #endif
                //rimLight.a = 0;

                //return shadedColor+rimLight;
                return shadedColor;
            }
            ENDHLSL
        }
    }
}
