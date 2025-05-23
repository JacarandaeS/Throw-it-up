#include "Assets/ShaderLibrary/Lighting.hlsl"


void GetLighting_float(float3 WorldPos, out float3 Direction, out float3 Color, out float Attenuation)
{
#if defined(SHADERGRAPH_PREVIEW)
        Direction = float3(0.5, 0.5, 0.0);
        Color = float3(1.0, 1.0, 1.0);
        Attenuation = 1.0;
#else
    Light mainLight = GetMainLight();
    Direction = mainLight.direction;
    Color = mainLight.color;
    Attenuation = mainLight.shadowAttenuation;
#endif
}
