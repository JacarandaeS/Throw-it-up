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

            float squareMask(float3 position, float3 center, float2 size, float hardness) 
            {
                float2 offset = position.xy - center.xy;
                float2 dist = abs(offset) / (size * 0.5);
                float maxDist = max(dist.x, dist.y);
                return saturate(1.0 - smoothstep(hardness, 1.0, maxDist));
            }

            float textureMask(float3 position, float3 center, float radius, sampler2D brushTex)
            {
                float2 offset = (position.xy - center.xy) / radius;
                offset = offset * 0.5 + 0.5;

                if (offset.x < 0.0 || offset.x > 1.0 || offset.y < 0.0 || offset.y > 1.0)
                    return 0.0;

                float4 brushSample = tex2D(brushTex, offset);
                return brushSample.a; // Use alpha for brush influence
            }

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
    if (_PrepareUV > 0) {
        return float4(0, 0, 1, 1); // For UV prep/debug mode
    }

    float4 baseColor = tex2D(_MainTex, i.uv);
    baseColor.a = max(baseColor.a, 0.001); // Prevent 0 alpha

    float influence = 0;

    if (_BrushMode == 0)
    {
        float falloff = circleMask(i.worldPos.xyz, _PainterPosition, _Radius, _Hardness);
        influence = pow(saturate(falloff * _Strength), 0.5);
    }
    else if (_BrushMode == 1)
    {
        float falloff = squareMask(i.worldPos.xyz, _PainterPosition, float2(_Radius, _Radius), _Hardness);
        influence = pow(saturate(falloff * _Strength), 0.5);
    }
    else if (_BrushMode == 2)
    {
        float texVal = textureMask(i.worldPos.xyz, _PainterPosition, _Radius, _BrushTex);
        influence = saturate(texVal * _Strength);
    }

    float falloffFactor = pow(influence, 0.8); // soft blend curve
    float4 paintColor = _PainterColor;

    // Blend RGB additively for layering
    float3 diff = paintColor.rgb - baseColor.rgb;
    float3 blendedRGB = baseColor.rgb + diff * falloffFactor * paintColor.a;

    // Accumulate alpha softly
    float blendedAlpha = saturate(baseColor.a + falloffFactor * paintColor.a * 0.5);

    return float4(blendedRGB, blendedAlpha);
}

            ENDCG
        }
    }
    FallBack "Diffuse"
}