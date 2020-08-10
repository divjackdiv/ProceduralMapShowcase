Shader "Unlit/ClipColor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ClipDist("Clip distance", float) = 0
        _ColorToClip1("Color To Clip", Color) = (1,1,1,1)
        _ColorToClip2("Color To Clip2", Color) = (1,1,1,1)
        _ColorToClip3("Color To Clip2", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

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

            float4 _ColorToClip1;
            float4 _ColorToClip2;
            float4 _ColorToClip3;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _ClipDist;

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
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv); 
                if (distance(col, _ColorToClip1) < _ClipDist) {
                    clip(-1);
                }  
                else if (distance(col, _ColorToClip2) < _ClipDist) {
                    clip(-1);
                }
                else if (distance(col, _ColorToClip3) < _ClipDist) {
                    clip(-1);
                }
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
