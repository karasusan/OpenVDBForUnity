#ifndef __VOLUME_SHADOWCASTER_INCLUDED__
#define __VOLUME_SHADOWCASTER_INCLUDED__

#include "UnityCG.cginc"
#include "Camera.cginc"
#include "Utils.cginc"

#ifndef ITERATIONS
#define ITERATIONS 100
#endif

uniform sampler3D _Volume;
float _StepDistance;
float _ShadowExtraBias = 0.01;

inline float3 DecodeNormal(float3 normal)
{
    return 2.0 * normal - 1.0;
}

inline float EncodeDepth(float4 pos)
{
    float z = pos.z / pos.w;
#if defined(SHADER_API_GLCORE) || \
    defined(SHADER_API_OPENGL) || \
    defined(SHADER_API_GLES) || \
    defined(SHADER_API_GLES3)
    return z * 0.5 + 0.5;
#else
    return z;
#endif
}

float SampleVolume(float3 uv)
{
    return tex3D(_Volume, uv).r;
}

struct appdata
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
    float3 normal : NORMAL;
};

struct v2f
{
    V2F_SHADOW_CASTER;
    float3 world : TEXCOORD1;
    float4 screenPos : TEXCOORD2;
    float3 normal : TEXCOORD3;
};

struct fragOutput
{
    fixed4 color : SV_Target;
    float depth : SV_Depth;
};

float3 deformVertex(float3 original)
{
    original.y += 0.5;
    original.xz *= original.y;
    return original;
}

v2f vert(appdata v)
{
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.screenPos = o.pos;
    o.world = mul(unity_ObjectToWorld, v.vertex).xyz;
    o.normal = mul(unity_ObjectToWorld, v.normal);
    return o;
}

float4 ApplyLinearShadowBias(float4 clipPos)
{
#if defined(UNITY_REVERSED_Z)
    clipPos.z += max(-1.0, min((unity_LightShadowBias.x - _ShadowExtraBias) / clipPos.w, 0.0));
    float clamped = min(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
#else
    clipPos.z += saturate((unity_LightShadowBias.x + _ShadowExtraBias) / clipPos.w);
    float clamped = max(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
#endif
    clipPos.z = lerp(clipPos.z, clamped, unity_LightShadowBias.y);
    return clipPos;
}

void frag(v2f i, out float4 outColor : SV_Target, out float outDepth : SV_Depth)
{
    Ray ray;
    ray.origin = Localize(i.world);

    // light direction of directional light
    float3 lightDir = -UNITY_MATRIX_V[2].xyz;
    ray.dir = normalize(mul((float3x3) unity_WorldToObject, lightDir));

    AABB aabb;
    aabb.min = float3(-0.5, -0.5, -0.5);
    aabb.max = float3(0.5, 0.5, 0.5);

    float tfar = Intersect(ray, aabb);

    float3 start = ray.origin;
    float3 end = ray.origin + ray.dir * tfar;

    float dist = length(end - start);
    half stepCount = dist / _StepDistance;
    float3 ds = ray.dir * _StepDistance;

    float3 p = start;
    float3 depth = end;

    [loop]
    for (int iter = 0; iter < ITERATIONS; iter++)
    {
        // sampling voxel
        float3 uv = GetUV(p);
        float cursample = SampleVolume(uv);

        if(cursample > 0.01)
        {
            depth = p;
            break;
        }
        p += ds;

        if(iter >= stepCount)
        {
            clip(-1);
            break;
        }
    }

    float4 opos = UnityClipSpaceShadowCasterPos(depth, i.normal);
    opos = UnityApplyLinearShadowBias(opos);

    outColor = outDepth = opos.z / opos.w;
}

#endif
