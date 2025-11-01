Shader "TNTC/TexturePainter"
{
    Properties
    {
        _PainterColor ("Painter Color", Color) = (0,0,0,0)
        [Enum(Circle,0,Square,1,Texture,2)] _BrushMode ("Brush Mode", Int) = 0
        _BrushTex ("Brush Texture", 2D) = "white" {}
        _BrushSize ("Brush Size (world XY)", Vector) = (1,1,0,0)
    }

    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Off

        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float3 _PainterPosition;
            float _Radius;
            float _Hardness;
            float _Strength;
            float4 _PainterColor;
            float _PrepareUV;
            int _BrushMode;
            sampler2D _BrushTex;
            float4 _BrushSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
            };

            // --- Masks (world-space) ---
            float maskCircle(float3 pos, float3 center, float radius, float hardness)
            {
                float d = distance(pos, center);
                return 1.0 - smoothstep(radius * hardness, radius, d);
            }

            float maskSquare(float3 pos, float3 center, float2 size, float hardness, float3 worldNormal)
            {
                // Calculate tangent and bitangent vectors
                float3 worldUp = abs(worldNormal.y) > 0.999 ? float3(0, 0, 1) : float3(0, 1, 0);
                float3 tangent = normalize(cross(worldNormal, worldUp));
                float3 bitangent = cross(worldNormal, tangent);
                
                // Transform position to local tangent space
                float3 delta = pos - center;
                float2 localPos = float2(dot(delta, tangent), dot(delta, bitangent));
                
                float2 halfSize = max(size * 0.5, 1e-5.xx);
                float2 nd = abs(localPos) / halfSize;
                float m = max(nd.x, nd.y);
                return 1.0 - smoothstep(hardness, 1.0, m);
            }

            float maskTexture(float3 pos, float3 center, float radius, float2 size, sampler2D brushTex, float3 worldNormal)
            {
                // Calculate tangent and bitangent vectors
                float3 worldUp = abs(worldNormal.y) > 0.999 ? float3(0, 0, 1) : float3(0, 1, 0);
                float3 tangent = normalize(cross(worldNormal, worldUp));
                float3 bitangent = cross(worldNormal, tangent);
                
                // Transform position to local tangent space
                float3 delta = pos - center;
                float2 localPos = float2(dot(delta, tangent), dot(delta, bitangent));
                
                // Convert to UV space
                float2 half = max(size * 0.5, 1e-5.xx);
                float2 uv = localPos / half * 0.5 + 0.5;

                if (any(uv < 0.0) || any(uv > 1.0)) return 0.0;
                return tex2D(brushTex, uv).a;
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.uv = v.uv;

                float4 clip = float4(0,0,0,1);
                clip.xy = float2(1, _ProjectionParams.x) * (v.uv.xy * 2.0 - 1.0);
                o.vertex = clip;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                if (_PrepareUV > 0) return float4(0,0,1,1);

                float4 baseCol = tex2D(_MainTex, i.uv);
                float infl = 0.0;

                if (_BrushMode == 0)
                {
                    infl = maskCircle(i.worldPos.xyz, _PainterPosition, _Radius, saturate(_Hardness));
                }
                else if (_BrushMode == 1)
                {
                    infl = maskSquare(i.worldPos.xyz, _PainterPosition, max(_BrushSize.xy, 1e-5.xx), saturate(_Hardness), i.worldNormal);
                }
                else // Texture brush
                {
                    float2 sz = any(_BrushSize.xy == 0) ? (_Radius * 2.0).xx : _BrushSize.xy;
                    infl = maskTexture(i.worldPos.xyz, _PainterPosition, _Radius, sz, _BrushTex, i.worldNormal);
                }

                infl = saturate(infl * _Strength);
                return lerp(baseCol, _PainterColor, infl);
            }
            ENDCG
        }
    }
}