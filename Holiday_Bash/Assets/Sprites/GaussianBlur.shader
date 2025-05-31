Shader "Custom/GaussianBlur_WithOpacity"
{
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Float) = 1.0
        _Opacity ("Opacity", Range(0,1)) = 1.0
    }
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _BlurSize;
            float _Opacity;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float2 uv = i.uv;
                float4 col = float4(0,0,0,0);
                float weights[5] = {0.227027f, 0.1945946f, 0.1216216f, 0.054054f, 0.016216f};

                for (int j = -4; j <= 4; j++) {
                    float2 offset = float2(j, 0) * _MainTex_TexelSize.xy * _BlurSize;
                    col += tex2D(_MainTex, uv + offset) * weights[abs(j)];
                }

                col.a *= _Opacity;  // Apply opacity control
                return col;
            }
            ENDCG
        }
    }
}