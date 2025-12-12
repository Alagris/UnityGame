Shader "Custom/HellSkyShader"
{
    Properties
    {
        [TopColor] _TopColor("Top Color", Color) = (1, 1, 1, 1)
        [BottomColor] _BottomColor("Bottom Color", Color) = (1, 1, 1, 1)
        [BrightBottomColor] _BottomColorBright("Bottom Color Bright", Color) = (1, 1, 1, 1)
        [LightningColor] _LightningColor("Lightning Color", Color) = (1, 1, 1, 1)
        [LightningScale] _LightningScale("Lightning Scale", float) = 10
        [LightningThreshold] _LightningThreshold("Lightning Threshold", float) = 0.5
        [LightningTimeScale] _LightningTimeScale("Lightning Time scale", float) = 0.1
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
                half4 _BottomColorBright;
                float _LightningScale;
                float _LightningTimeScale;
                float _LightningThreshold;
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
                
                float noise1d = GradientNoise1D(_Time.y*2);
                
                half4 bot = lerp(_BottomColor, _BottomColorBright, smoothstep(0.6, 1, noise1d));
                float3 noiseCoord = float3(IN.positionWorld.xz*_LightningScale, _Time.y * _LightningTimeScale);
                float lightningStrenght = SimplexNoise3D(noiseCoord);
                lightningStrenght = smoothstep(_LightningThreshold, 1, lightningStrenght);
                half4 gradient = lerp(bot, _TopColor, IN.uv.y);
                half4 withLighnings = lerp(gradient, gradient + _LightningColor, lightningStrenght);
                return withLighnings;
            }
            ENDHLSL
        }
    }
}
