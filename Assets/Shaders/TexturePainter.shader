Shader "TNTC/TexturePainter" 
{   
    Properties 
    {
        _PainterColor ("Painter Color", Color) = (0, 0, 0, 0)
        _MainTex ("Main Texture", 2D) = "white" {}
    }

    SubShader 
    {
        Tags { "RenderType" = "Opaque" }

        // Don't cull, write depth or test Z-buffer
        Cull Off
        ZWrite Off
        ZTest Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass 
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float3 _PainterPosition;
            float _Radius;
            float _Hardness;
            float _Strength;
            float4 _PainterColor;
            float _PrepareUV;

            struct appdata 
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f 
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            // Brush mask falloff
            float mask(float3 position, float3 center, float radius, float hardness) 
            {
                float dist = distance(center, position);
                return saturate(1.0 - smoothstep(radius * hardness, radius, dist));
            }

            v2f vert(appdata v) 
            {
                v2f o;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                float4 world = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = world.xyz;

                float4 clipPos;
                clipPos.xy = (v.uv.xy * 2 - 1) * float2(1, _ProjectionParams.x); // aspect-correct
                clipPos.z = 0;
                clipPos.w = 1;
                o.vertex = clipPos;
                return o;
            }

      fixed4 frag (v2f i) : SV_Target {
    if (_PrepareUV > 0) {
        return float4(0, 0, 1, 1); // UV visualization pass
    }

    float4 baseColor = tex2D(_MainTex, i.uv);
    
    // 👇 Ensure baseColor has a minimum alpha, so we blend properly even on transparent
    baseColor.a = max(baseColor.a, 0.001);

    float brushFalloff = mask(i.worldPos.xyz, _PainterPosition, _Radius, _Hardness);
    float influence = saturate(brushFalloff * _Strength);

    if (influence < 0.001) discard;

    // Optional: smoothen falloff for softness
    influence = pow(influence, 0.5);

    // Blend paint over base color
    float4 result = baseColor * (1 - influence) + _PainterColor * influence;

    return result;
}

            ENDCG
        }
    }

    FallBack "Diffuse"
}