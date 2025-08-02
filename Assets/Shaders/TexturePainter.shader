Shader "TNTC/TexturePainter" 
{   
    Properties 
    {
        _PainterColor ("Painter Color", Color) = (0, 0, 0, 0)
        _MainTex ("Main Texture", 2D) = "white" {}
        [Enum(Main,0,Superior,1)] _MaskType ("Mask Type", Int) = 0
        [Enum(Circle, 0, Square, 1, Texture, 2)] _BrushMode ("Brush Mode", Int) = 0
        _BrushTex ("Brush Texture", 2D) = "white" {}
    }

    SubShader 
    {
        Tags { "RenderType" = "Opaque" }

        Cull Off
        ZWrite Off
        ZTest Off

        // 🔁 CAMBIADO: blending tradicional con alpha
        Blend SrcAlpha OneMinusSrcAlpha

        Pass 
        {
            CGPROGRAM
            #pragma target 3.0
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
            int _MaskType;
            int _BrushMode;
            sampler2D _BrushTex;
            float2 _BrushSize; 

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

            float circleMask(float3 position, float3 center, float radius, float hardness) 
            {
                float dist = distance(center.xy, position.xy);
                return saturate(1.0 - smoothstep(radius * hardness, radius, dist));
            }
           

           // windows
            v2f vert(appdata v) 
            {
                v2f o;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                float4 world = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = world.xyz;

                float4 clipPos;
                clipPos.xy = (v.uv.xy * 2 - 1) * float2(1, _ProjectionParams.x);
                clipPos.z = 0;
                clipPos.w = 1;
                o.vertex = clipPos;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target 
            {
                float4 baseColor = tex2D(_MainTex, i.uv);

                float influence = 0;

                if (_BrushMode == 0)
                {
                    float falloff = circleMask(i.worldPos.xyz, _PainterPosition, _Radius, _Hardness);
                    falloff = pow(falloff, 2.5);
                    influence = saturate(falloff * _Strength);
                }

                float falloffFactor = pow(influence, 0.9);
                if (falloffFactor < 0.01)
                    discard;

                // 🔁 CAMBIADO: premultiplicación correcta
                float4 paintColor = _PainterColor;
                paintColor.rgb *= paintColor.a; // premultiply RGB
                float gray = dot(paintColor.rgb, float3(0.3, 0.59, 0.11)); // Luminance
                paintColor.rgb = lerp(gray.xxx, paintColor.rgb, 0.8); // Reduce saturation to 80%

                // 🔁 CAMBIADO: mezcla sobre base usando transparencia
                float4 finalColor = lerp(baseColor, paintColor, falloffFactor);

                // opcional: descartar píxeles completamente transparentes
                if (finalColor.a < 0.01) discard;

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
