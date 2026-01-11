Shader "Custom/HellSkyShader"
{
    Properties
    {
        [TopColor] _TopColor("Top Color", Color) = (1, 1, 1, 1)
        [BottomColor] _BottomColor("Bottom Color", Color) = (1, 1, 1, 1)
        [LightningColor] _LightningColor("Lightning Color", Color) = (1, 1, 1, 1)
        [SkyLightsStretch] _SkyLightsStretch("Sky Lights Stretch", float) = 1
        [SkyLightsFallSpeed] _SkyLightsFallSpeed("Sky Lights Fall Speed", float) = 0
        [SkyLightsHeight] _SkyLightsHeight("Sky Lights Height", float) = 0
        [SkyLightsHeightFalloff] _SkyLightsHeightFalloff("Sky Lights Height Falloff", float) = 0.1
        [SkyLightsThreshold] _SkyLightsThreshold("Sky Lights Threshold", float) = 0.5
        [SkyLightsFalloff] _SkyLightsFalloff("Sky Lights Falloff", float) = 0.2
        [SkyLightsColor] _SkyLightsColor("Sky Lights Color", Color) = (1, 1, 1, 1)
        [SkyLightsScale] _SkyLightsScale("Sky Lights Scale", float) = 10
        [SkyLightsTimeScale] _SkyLightsTimeScale("Sky Lights Time scale", float) = 0.1
        [SkyLogarithmicScale] _SkyLogarithmicScale("Sky Lights Logarithmic scale", float) = 1
        //[Time] _Time("Time", float) = 0
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off 
        ZWrite Off

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "../../Scripts/Shaders/Noise/Noise1D.hlsl"
            #include "../../Scripts/Shaders/Noise/SimplexNoise2D.hlsl"
            #include "../../Scripts/Shaders/Noise/SimplexNoise3D.hlsl"
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
                float3 positionWorld : TEXCOORD1;
            };

            
            

            CBUFFER_START(UnityPerMaterial)
                half4 _TopColor;
                half4 _BottomColor;
                half4 _LightningColor;
                float _SkyLightsStretch;
                float _SkyLightsFallSpeed;
                float _SkyLightsHeight;
                float _SkyLightsHeightFalloff;
                float _SkyLightsThreshold;
                float _SkyLightsFalloff;
                half4 _SkyLightsColor;
                float _SkyLightsScale;
                float _SkyLightsTimeScale;
                float _SkyLogarithmicScale;

            //    float _Time;
            CBUFFER_END

            

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = UnityObjectToClipPos(IN.positionOS);
                OUT.uv = IN.uv;
                OUT.positionWorld = normalize(IN.positionOS);
                return OUT;
            }

            

            half4 frag(Varyings IN) : SV_Target
            {
                
                float lightningOccurenceNoise1d = GradientNoise1D(_Time.y*2);
                
                half4 lightningColor = lerp(_BottomColor, _LightningColor, smoothstep(0.6, 1, lightningOccurenceNoise1d));
                float2 logarithmicSkyPositon = IN.positionWorld.xz/IN.positionWorld.y;
                float3 skyLightNoiseCoord = float3(logarithmicSkyPositon*_SkyLightsScale, _Time.y * _SkyLightsTimeScale);
                skyLightNoiseCoord.x *= _SkyLightsStretch;
                skyLightNoiseCoord.y +=  _Time.y * _SkyLightsFallSpeed;
                float skyLightStrenght = SimplexNoise3DFbm(skyLightNoiseCoord, 1.2, 0.8, float2(0.1, 0.0), 0.5, 5);
                skyLightStrenght = smoothstep(_SkyLightsThreshold, _SkyLightsThreshold+_SkyLightsFalloff, skyLightStrenght);
                skyLightStrenght = skyLightStrenght * smoothstep(_SkyLightsHeight, _SkyLightsHeight+_SkyLightsHeightFalloff, IN.uv.y);
                half4 gradient = lerp(lightningColor, _TopColor, IN.uv.y);

                half4 withSkyLights = lerp(gradient, gradient + _SkyLightsColor, skyLightStrenght);
                return withSkyLights;

            }
            ENDHLSL
        }
    }
}
