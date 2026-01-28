Shader "Custom/EarthGroundShader"
{
    Properties
    {
        [TopColor1] _DarkColor1("Dark Color 1", Color) = (1, 1, 1, 1)
        [TopColorChance1] _DarkColor1Chance("Dark Color 1 Chance", float) = 0.1
        [TopColorScale1] _DarkColor1Scale("Dark Color 1 Scale", float) = 10
        [TopColorFbmIterations1] _TopColorFbmIterations1("Dark Color 1 FbmIterations", int) = 1
        [TopColorFbmAmplitudeChange1] _DarkColor1FbmAmplitudeChange("Dark Color 1 Amplitude Change", float) = 0.8
        [TopColorFbmFrequencyChange1] _DarkColor1FbmFrequencyChange("Dark Color 1 Frequency Change", float) = 1.2
        [TopColor2] _DarkColor2("Dark Color 2", Color) = (1, 1, 1, 1)
        [TopColorChance2] _DarkColor2Chance("Dark Color 2 Chance", float) = 0.1
        [TopColorScale2] _DarkColor2Scale("Dark Color 2 Scale", float) = 10
        [TopColorFbmIterations2] _TopColorFbmIterations2("Dark Color 2 FbmIterations", int) = 1
        [TopColorFbmAmplitudeChange2] _DarkColor2FbmAmplitudeChange("Dark Color 2 Amplitude Change", float) = 0.8
        [TopColorFbmFrequencyChange2] _DarkColor2FbmFrequencyChange("Dark Color 2 Frequency Change", float) = 1.2
        [BottomColor] _LightColor("Light Color", Color) = (1, 1, 1, 1)
        
        
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "../../../Scripts/Shaders/Noise/SimplexNoise2D.hlsl"
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
                half4 _DarkColor1;
                float _DarkColor1Chance;
                float _DarkColor1Scale;
                int _TopColorFbmIterations1;
                float _DarkColor1FbmAmplitudeChange;
                float _DarkColor1FbmFrequencyChange;
                half4 _DarkColor2;
                float _DarkColor2Chance;
                float _DarkColor2Scale;
                int _TopColorFbmIterations2;
                float _DarkColor2FbmAmplitudeChange;
                float _DarkColor2FbmFrequencyChange;
                half4 _LightColor;

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
                
                float grassTone1 = SimplexNoise2DFbm(IN.positionWorld.xz*_DarkColor1Scale, _DarkColor1FbmFrequencyChange, _DarkColor1FbmAmplitudeChange, float2(0.1, 0.0), 0.5, _TopColorFbmIterations1);
                float grassTone2 = SimplexNoise2DFbm(IN.positionWorld.zx*_DarkColor2Scale, _DarkColor2FbmFrequencyChange, _DarkColor2FbmAmplitudeChange, float2(0.1, 0.0), 0.5, _TopColorFbmIterations2);
                //float grassTone1 = SimplexNoise2D(IN.positionWorld.xz*_DarkColor1Scale);
                //float grassTone2 = SimplexNoise2D(IN.positionWorld.zx*_DarkColor2Scale);
                bool isGrass1 = grassTone1 < _DarkColor1Chance;
                bool isGrass2 = grassTone2 < _DarkColor2Chance;

                half4 grassColor = grassTone1 < grassTone2 ? ( isGrass1 ? _DarkColor1 : _LightColor ): (isGrass2 ? _DarkColor2 : _LightColor );
                //grassColor = isGrass2 ? _DarkColor2: grassColor ;
                return grassColor;
            }
            ENDHLSL
        }
    }
}
