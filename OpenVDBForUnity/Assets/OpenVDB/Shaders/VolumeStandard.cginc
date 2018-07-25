#ifndef __VOLUME_RENDERING_INCLUDED__
#define __VOLUME_RENDERING_INCLUDED__

#include "UnityCG.cginc"

#ifndef ITERATIONS
#define ITERATIONS 100
#endif

half4 _Color;
uniform sampler3D _Volume;
half _Intensity;
half _ShadowSteps;
float _ShadowDensity;
float _StepDistance;

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

bool intersect(Ray r, AABB aabb, out float t0, out float t1)
{
  float3 invR = 1.0 / r.dir;
  float3 tbot = invR * (aabb.min - r.origin);
  float3 ttop = invR * (aabb.max - r.origin);
  float3 tmin = min(ttop, tbot);
  float3 tmax = max(ttop, tbot);
  float2 t = max(tmin.xx, tmin.yz);
  t0 = max(t.x, t.y);
  t = min(tmax.xx, tmax.yz);
  t1 = min(t.x, t.y);
  return t0 <= t1;
}

float3 localize(float3 p) 
{
    return mul(unity_WorldToObject, float4(p, 1)).xyz;
}

float3 get_uv(float3 p) 
{
    return (p + 0.5);
}

float sample_volume(float3 uv)
{
    return tex3D(_Volume, uv).r;
}

float ComputeDepth(float4 clippos)
{
#if defined(SHADER_TARGET_GLSL) || defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)
    return (clippos.z / clippos.w) * 0.5 + 0.5;
#else
    return clippos.z / clippos.w;
#endif
}

struct appdata
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2f
{
    float4 vertex : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 world : TEXCOORD1;
};

struct fragOutput 
{
    fixed4 color : SV_Target;
    float depth : SV_Depth;
};

v2f vert(appdata v)
{
  v2f o;
  o.vertex = UnityObjectToClipPos(v.vertex);
  o.uv = v.uv;
  o.world = mul(unity_ObjectToWorld, v.vertex).xyz;
  return o;
}

fragOutput frag(v2f i)
{
    Ray ray;
    ray.origin = localize(i.world);

    // world space direction to object space
    float3 cameraVec = i.world - _WorldSpaceCameraPos;
    float cameraDist = length(cameraVec);
    float3 cameraDir = normalize(cameraVec);

    float3 cameraForward = -UNITY_MATRIX_V[2].xyz;
    float stepDistRatio = 1.0 / dot(cameraDir, cameraForward);


    ray.dir = normalize(mul((float3x3) unity_WorldToObject, cameraDir));

    AABB aabb;
    aabb.min = float3(-0.5, -0.5, -0.5);
    aabb.max = float3(0.5, 0.5, 0.5);

    float tnear;
    float tfar;
    intersect(ray, aabb, tnear, tfar);

    tnear = max(0.0, tnear);

    float stepDist = _StepDistance * stepDistRatio;
    float startOffset = fmod(cameraDist, stepDist);

    float3 start = ray.origin + ray.dir * (stepDist - startOffset);
    float3 end = ray.origin + ray.dir * tfar;
    float dist = abs(tfar - tnear);
    half stepCount = dist / stepDist;
    float3 ds = ray.dir * stepDist;

    // shadow parameter
    // directional light
    float shadowstepsize = 1.0 / (float)_ShadowSteps;
    float3 lightVec = normalize(mul((float3x3) unity_WorldToObject, _WorldSpaceLightPos0.xyz))  * shadowstepsize;
    float shadowDensity = _ShadowDensity * shadowstepsize;

    // threshold for shadow density
    float shadowthreshold = -log(0.001) / shadowDensity;

    float3 p = start;
    float3 depth = end;
    bool depthtest = true;

    float curdensity = 0.0;
    float transmittance = 1;
    float3 lightenergy = 0;

    [unroll]
    for (int iter = 0; iter < ITERATIONS; iter++)
    {
        // sampling voxel
        float3 uv = get_uv(p);
        float cursample = sample_volume(uv);

        if(cursample > 0.01)
        {
            float3 lpos = p;
            float shadowdist = 0;

            if(depthtest)
            {
                depth = p;
                depthtest = false;
            }

            [loop]
            for (int s = 0; s < _ShadowSteps; s++)
            {
                lpos += lightVec;
                float3 luv = get_uv(lpos);
                float lsample = sample_volume(saturate(luv));

                float3 shadowboxtest = floor( 0.5 + ( abs( 0.5 - luv ) ) );
                float exitshadowbox = shadowboxtest .x + shadowboxtest .y + shadowboxtest .z;

                shadowdist += lsample;

                // check to exit shadow box
                if(shadowdist > shadowthreshold || exitshadowbox >= 1)
                {
                    break;
                }
            }
            curdensity = saturate(cursample * _Intensity);
            float shadowterm = exp(-shadowdist * shadowDensity);
            float3 absorbedlight = shadowterm * curdensity;
            lightenergy += absorbedlight * transmittance;
            transmittance *= 1-curdensity;
        }
        p += ds;

        if(iter >= stepCount)
        {
            break;
        }

        if (transmittance < 0.01)
        {
            break;
        }
    }
    if(depthtest)
    {
        clip(-1);
    }

    float3 world_depth = mul(unity_ObjectToWorld, depth).xyz;

    fragOutput o;
    o.color = float4( lightenergy, 1-transmittance);
    o.depth = ComputeDepth(mul(UNITY_MATRIX_VP, float4(world_depth, 1.0)));
    return o;
}

#endif 