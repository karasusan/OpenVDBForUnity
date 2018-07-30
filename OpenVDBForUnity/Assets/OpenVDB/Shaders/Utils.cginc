#ifndef _UTILS_H_
#define _UTILS_H_

struct Ray
{
    float3 origin;
    float3 dir;
};

struct AABB
{
    float3 min;
    float3 max;
};

inline bool IsInnerCube(float3 pos)
{
    return all(max(0.5 - abs(pos), 0.0));
}

inline float Intersect(Ray r, AABB aabb)
{
  float3 invR = 1.0 / r.dir;
  float3 tbot = invR * (aabb.min - r.origin);
  float3 ttop = invR * (aabb.max - r.origin);
  float3 tmax = max(ttop, tbot);
  float2 t = min(tmax.xx, tmax.yz);
  return min(t.x, t.y);
}

inline float3 Localize(float3 p)
{
    return mul(unity_WorldToObject, float4(p, 1)).xyz;
}

inline float3 GetUV(float3 p)
{
    return (p + 0.5);
}

inline float ComputeDepth(float4 clippos)
{
#if defined(SHADER_TARGET_GLSL) || defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)
    return (clippos.z / clippos.w) * 0.5 + 0.5;
#else
    return clippos.z / clippos.w;
#endif
}

#endif
