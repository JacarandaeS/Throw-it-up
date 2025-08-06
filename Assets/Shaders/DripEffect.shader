Shader "Custom/DripEffect"
{
     Properties
    {
        _MainTex ("Paint Texture", 2D) = "white" {}
        _DripSpeed ("Drip Speed", Float) = 0.001
        _DripFade ("Drip Fade", Float) = 0.99
        _DripBlend ("Drip Blend Strength", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            ZWrite Off
            ZTest Always
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _DripSpeed;
            float _DripFade;
            float _DripBlend;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                float2 uv = i.uv;

                float2 dripOffset = float2(0, -_DripSpeed); // down in UV space
                float2 aboveUV = uv + dripOffset;

                fixed4 current = tex2D(_MainTex, uv);
                fixed4 above = tex2D(_MainTex, aboveUV);

                // Blend between current and above (drip simulation)
                fixed4 result = lerp(current, above, _DripBlend);

                // Optional fade to avoid infinite buildup
                result *= _DripFade;

                return result;
            }
            ENDCG
        }
    }
    FallBack Off
}
