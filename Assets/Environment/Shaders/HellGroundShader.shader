Shader "Custom/HellGroundShader"
{
    Properties
    {
        [TopColor] _GroundColor("Ground Color", Color) = (1, 1, 1, 1)
        [BottomColor] _LavaColor("Lava Color", Color) = (1, 1, 1, 1)
        [BrightBottomColor] _LavaColorBright("Lava Color Bright", Color) = (1, 1, 1, 1)
        [CrackScale] _CrackScale("Crack Scale", float) = 10
        [CrackThickness] _CrackThickness("Crack Thickness", float) = 0.1
        [AngleOffset] _AngleOffset("Crack Angle Offset", float) = 0
        [LightningScale] _LightningScale("Lightning Scale", float) = 10
        [LightningThreshold] _LightningThreshold("Lightning Threshold", float) = 0.5
        [LightningTimeScale] _LightningTimeScale("Lightning Time scale", float) = 0.1
        //[Time] _Time("Time", float) = 0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "../../Scripts/Shaders/Noise/Voronoi.hlsl"
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
                half4 _GroundColor;
                half4 _LavaColor;
                half4 _LavaColorBright;
                float _CrackScale;
                float _CrackThickness;
                float _LightningScale;
                float _LightningTimeScale;
                float _LightningThreshold;
                float _AngleOffset;
            //    float _Time;
            CBUFFER_END

            

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = UnityObjectToClipPos(IN.positionOS);
                OUT.uv = IN.uv;
                OUT.positionWorld = IN.positionOS;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                
//                float noise1d = GradientNoise1D(_Time.y*2);
                
                //half4 bot = lerp(_BottomColor, _BottomColorBright, smoothstep(0.6, 1, noise1d));
//                
    //            lightningStrenght = smoothstep(_LightningThreshold, 1, lightningStrenght);
      //          half4 gradient = lerp(bot, _TopColor, IN.uv.y);
        //        half4 withLighnings = lerp(gradient, gradient + _LightningColor, lightningStrenght);
                
                float2 vor = Voronoi(IN.positionWorld.xz*_CrackScale, _AngleOffset);
                float2 lavaCoord = IN.positionWorld.xz*_LightningScale;
                float lavaTime = _Time.y * _LightningTimeScale;
                lavaCoord.x += lavaTime ;
                float3 noiseCoord = float3(lavaCoord, lavaTime );
                float lavaStrenght = SimplexNoise3D(noiseCoord);
                half4 lavaColor = lerp(_LavaColor, _LavaColorBright, lavaStrenght);
                half4 color = lerp(lavaColor, _GroundColor, smoothstep(0, _CrackThickness, vor.y));
                return color;
            }
            ENDHLSL
        }
    }
}
