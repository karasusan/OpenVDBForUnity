#ifndef __VOLUME_STANDARD_INCLUDED__
#define __VOLUME_STANDARD_INCLUDED__

#include "UnityCG.cginc"
#include "Camera.cginc"
#include "Utils.cginc"

#ifndef ITERATIONS
#define ITERATIONS 100
#endif

half3 _AmbientColor;
uniform sampler3D _Volume;
half _Intensity;
half _ShadowSteps;
half3 _ShadowDensity;
float _StepDistance;
float _AmbientDensity;

float SampleVolume(float3 uv)
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
    float4 screenPos : TEXCOORD2;
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
  o.screenPos = o.vertex;
  o.uv = v.uv;
  o.world = mul(unity_ObjectToWorld, v.vertex).xyz;
  return o;
}

fragOutput frag(v2f i)
{
    Ray ray;
    ray.origin = Localize(i.world);

    float3 cameraDir = GetCameraDirection(i.screenPos);
    ray.dir = normalize(mul((float3x3) unity_WorldToObject, cameraDir));

    float3 cameraPos = GetCameraPosition();
    float3 nearCameraPos = cameraPos + (GetCameraNearClip() + 0.01) * cameraDir;
    nearCameraPos = Localize(nearCameraPos);

    // If camera inside volume cube, change the original position of the ray.
    if(IsInnerCube(nearCameraPos))
    {
        ray.origin = nearCameraPos;
    }

    AABB aabb;
    aabb.min = float3(-0.5, -0.5, -0.5);
    aabb.max = float3(0.5, 0.5, 0.5);

    float tfar = Intersect(ray, aabb);

    // calculate start offset
    float3 cameraForward = GetCameraForward();
    float stepDistRatio = 1.0 / dot(cameraDir, cameraForward);

    float cameraDist = length(i.world - cameraPos);
    float stepDist = _StepDistance * stepDistRatio;
    float startOffset = fmod(cameraDist, stepDist);

    // sampling parameter (start, end, stepcount)
    float3 start = ray.origin + ray.dir * (stepDist - startOffset);
    float3 end = ray.origin + ray.dir * tfar;
    float dist = length(end - start);
    half stepCount = dist / stepDist;
    float3 ds = ray.dir * stepDist;

    // shadow parameter
    // directional light
    float shadowstepsize = 1.0 / (float)_ShadowSteps;
    float3 lightVec = normalize(mul((float3x3) unity_WorldToObject, _WorldSpaceLightPos0.xyz))  * shadowstepsize;
    float3 shadowDensity = 1.0 / _ShadowDensity * shadowstepsize;

    // threshold for shadow density
    float shadowthreshold = -log(0.001) / length(shadowDensity);

    float3 p = start;
    float3 depth = end;
    bool depthtest = true;

    float curdensity = 0.0;
    float transmittance = 1;
    float3 lightenergy = 0;

    //[unroll]
    [loop]
    for (int iter = 0; iter < ITERATIONS; iter++)
    {
        // sampling voxel
        float3 uv = GetUV(p);
        float cursample = SampleVolume(uv);

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
                float3 luv = GetUV(lpos);
                float lsample = SampleVolume(saturate(luv));

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
            float3 shadowterm = exp(-shadowdist * shadowDensity);
            float3 absorbedlight = shadowterm * curdensity;
            lightenergy += absorbedlight * transmittance;
            transmittance *= 1-curdensity;

            // sky ambient lighting
            shadowdist = 0;

            float3 luv = uv + float3(0,0,0.05);
            shadowdist = SampleVolume(saturate(luv));
            luv = uv + float3(0,0,0.1);
            shadowdist += SampleVolume(saturate(luv));
            luv = uv + float3(0,0,0.2);
            shadowdist += SampleVolume(saturate(luv));
            lightenergy += exp(-shadowdist * _AmbientDensity) * curdensity * _AmbientColor * transmittance;
            // sky ambient lighting
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