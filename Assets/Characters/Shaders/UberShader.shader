Shader "Custom/FaceAnimeShader"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _DiffuseMap("Base Map", 2D) = "white" {}

        [Toggle] _SHADOW ("Enable shadows", Integer) = 0
        [ShadowColor] _ShadowColor("Shadow Color", Color) = (0.3, 0.3, 0.3, 1)
        [HalfShadowColor] _HalfShadowColor("Half Shadow Color", Color) = (0.6, 0.6, 0.6, 1)
        
        [LightSmooth] _LightSmooth("Light Smoothing", Range(0.0,1.0)) = 0.1
        [MidBand] _MidBand("Mid Band lenght", Range(0.0,1.0)) = 0.01


        
        [RimLightColor] _RimLightColor("Rim Light Color", Color) = (1, 0, 0, 1)
        [RimLightPower] _RimLightPower("Rim Light Power", Float) = 1.5
        [Toggle] _RIM_LIGHT_SMOOTHSTEP ("Use Rim Light Smoothstep", Integer) = 0
        [RimLightFalloff] _RimLightFalloff("Rim Light Falloff", Float) = 0
        [Toggle] _RIM_LIGHT_ON_SHADOW_SIDE ("Show rim light on shaded side too", Integer) = 0

        [SpecularColor] _SpecularColor("Specular Color", Color) = (1, 0, 0, 1)
        [SpecularStrength] _SpecularStrength("Specular Strength", Float) = 0.9
        [SpecularFalloff] _SpecularFalloff("Specular Falloff", Float) = 1
        
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _RIM_LIGHT_SMOOTHSTEP_ON
            #pragma shader_feature _RIM_LIGHT_ON_SHADOW_SIDE_ON
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
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
                half3 worldNormal: NORMAL;
                half3 viewDir: TEXCOORD1;
            };

            TEXTURE2D_ARRAY(_DiffuseMap);
            SAMPLER(sampler_DiffuseMap);
            

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _ShadowColor;
                half4 _HalfShadowColor;
                float _LightSmooth;
                float _MidBand;
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
                OUT.worldNormal = UnityObjectToWorldNormal(IN.normal);= 
                OUT.viewDir = WorldSpaceViewDir(IN.positionOS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {

                float lDot = dot(normalize(IN.worldNormal), normalize(_WorldSpaceLightPos0.xyz));
                
                half4 diffuse = UNITY_SAMPLE_TEX2D(_BaseMap, IN.uv) ; 
                float lightSmoothing = _LightSmooth;
                float midBand = _MidBand;

                half4 shadedColor = diffuse;
                bool isShadow = false;
                #if _SHADOW_ON
                {
                    float lightIntensity = smoothstep(0, _LightSmooth, lDot);
                    isShadow = lDot<lightSmoothing+ midBand;
                    half4 lLerp = isShadow ? lerp(_ShadowColor, _HalfShadowColor, lightIntensity) : _BaseColor;
                    shadedColor *= lLerp;
                }
                #endif
                

                
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
                
                //rimLight.a = 0;

                //return shadedColor+rimLight;
                return shadedColor;
            }
            ENDHLSL
        }
    }
}
