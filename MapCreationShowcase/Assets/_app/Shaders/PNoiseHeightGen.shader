Shader "Unlit/PNoiseHeightGen"
{
    Properties
    { 
        _MainTex("MainTex", 2d) = "white" {}
        _HeightMap("Height Map", 2d) = "white" {}
        _ColorAffected("Color Affected", Color) = (1,1,1,1)
        _ResolutionX("Resolution X", Int) = 0
        _ResolutionY("Resolution Y", Int) = 0
        _TilingSpeed("Tiling Speed", Vector) = (1,1,0,0)

    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog


            #pragma multi_compile CNOISE
            #pragma multi_compile  THREED

            #include "UnityCG.cginc"
            #if defined(CNOISE) || defined(PNOISE)

                #if defined(THREED)
                    #include "Packages/jp.keijiro.noiseshader/Shader/ClassicNoise3D.hlsl"
                #else
                    #include "Packages/jp.keijiro.noiseshader/Shader/ClassicNoise2D.hlsl"
                #endif
            #endif

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            float4 _ColorAffected;

            float _Seed;

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _HeightMap;
            float4 _HeightMap_ST;

            int _FilledArrays;
            float _SpeedArray[5];
            float _FrequencyArray[5];
            float _AmplitudeArray[5]; 

            int _GradientKeysCount;
            fixed4 _GradientColorArr[10];
            float _GradientKeysArr[10];

            int _GradientAlphaKeysCount;
            float _AlphaKeysArr[10];
            float _AlphaKeysTimeArr[10];

            int _ResolutionX;
            int _ResolutionY;
            float4  _TilingSpeed;
            float _Scale;
            float _Startval;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 mainTex = tex2D(_MainTex, i.uv);
                fixed4 heightCol = tex2D(_HeightMap, i.uv);
                if (distance(mainTex, _ColorAffected) > 0.01) {
                    clip(-1);
                }
                float compoundNoise = 0;
                for (int j = 0; j < _FilledArrays;j++)
                {
                    float2 uv = i.uv;
                    uv.x = floor(_ResolutionX * uv.x + _TilingSpeed.x * _Time.y + _Seed);
                    uv.y = floor(_ResolutionY * uv.y + _TilingSpeed.y * _Time.y + _Seed);
                    uv = uv * _FrequencyArray[j]; 
                    float3 coord = float3(uv * _Scale, _Time.y * _SpeedArray[j]);
                    float noise = cnoise(coord) * _AmplitudeArray[j];
                    compoundNoise = compoundNoise + noise;
                }
                 
                float height = heightCol + compoundNoise;
                fixed4 gradientCol;
                for (int j = 0; j < _GradientKeysCount; j++)
                { 
                    if (_GradientKeysArr[j] > height || j == _GradientKeysCount - 1) {
                        gradientCol = _GradientColorArr[j] ;
                        break;
                    }
                } 
                for (int j = 0; j < _GradientAlphaKeysCount; j++)
                { 
                    if (_AlphaKeysTimeArr[j] > height || j == _GradientAlphaKeysCount) {
                        gradientCol.a = _AlphaKeysArr[j];
                        break;
                    }
                } 
                return gradientCol;
            }
            ENDCG
        }
    }
}
