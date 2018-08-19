#ifndef __VOLUME_STANDARD_INCLUDED__
#define __VOLUME_STANDARD_INCLUDED__

#include "UnityCG.cginc"
#include "Camera.cginc"
#include "Utils.cginc"

#ifndef ITERATIONS
#define ITERATIONS 100
#endif

uniform sampler3D _Volume;

#ifdef ENABLE_TRACE_DISTANCE_LIMITED
uniform sampler2D _CameraDepthTexture;
#endif

half _Intensity;
half _ShadowSteps;
half _ShadowThreshold;
half3 _ShadowDensity;
float _StepDistance;

#ifdef ENABLE_AMBIENT_LIGHT
half3 _AmbientColor;
float _AmbientDensity;
#endif

float SampleVolume(float3 uv)
{
    return tex3D(_Volume, uv).r;
}

struct appdata
{
    float4 vertex : POSITION;
};

struct v2f
{
    float4 vertex : SV_POSITION;
    float3 world : TEXCOORD1;
    float4 pos : TEXCOORD2;
    float4 screenPos : TEXCOORD3;
};

struct fragOutput
{
    fixed4 color : SV_Target0;
    float depth : SV_Depth;
};

v2f vert(appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.pos = o.vertex;
    o.screenPos = ComputeScreenPos(o.vertex);
    o.world = mul(unity_ObjectToWorld, v.vertex).xyz;
    return o;
}

fragOutput frag(v2f i)
{
    Ray ray;

    float3 cameraDir = GetCameraDirection(i.pos);
    ray.dir = normalize(mul((float3x3) unity_WorldToObject, cameraDir));

    float3 rayOriginWorld = i.world;
    float3 cameraPos = GetCameraPosition();

    #ifdef ENABLE_CAMERA_INSIDE_CUBE
    // get near camera position in object space
    float3 nearCameraPos = cameraPos + (GetCameraNearClip() + 0.01) * cameraDir;
    float3 nearCameraPosLocal = Localize(nearCameraPos);

    // If camera inside volume cube, change the original position of the ray.
    if(IsInnerCube(nearCameraPosLocal))
    {
        rayOriginWorld = nearCameraPos;
    }
    #endif
    ray.origin = Localize(rayOriginWorld);

    AABB aabb;
    aabb.min = float3(-0.5, -0.5, -0.5);
    aabb.max = float3(0.5, 0.5, 0.5);

    float tfar = Intersect(ray, aabb);

    // calculate start offset
    #ifdef ENABLE_SAMPLING_START_OFFSET
    float3 cameraForward = GetCameraForward();
    float stepDist = _StepDistance / dot(cameraDir, cameraForward);

    float cameraDist = length(rayOriginWorld - cameraPos);
    float startOffset = stepDist - fmod(cameraDist, stepDist);
    float3 start = ray.origin + mul((float3x3) unity_WorldToObject, cameraDir * startOffset);
    #else
    float stepDist = _StepDistance;
    float3 start = ray.origin;
    #endif

    // sampling parameter (start, end, stepcount)
    float3 end = ray.origin + ray.dir * tfar;

    #ifdef ENABLE_TRACE_DISTANCE_LIMITED
    //Get the distance to the camera from the depth buffer for this point
    float2 uv = i.screenPos.xy / i.screenPos.w;
    float sceneDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv));

    float tfar2 = length(ray.origin - Localize(sceneDepth * cameraDir + cameraPos));
    end = ray.origin + ray.dir * min(tfar, tfar2);
    #endif

    float dist = length(end - start);
    half stepCount = dist / stepDist;
    float3 ds = ray.dir * stepDist;

    // shadow parameter
    // directional light
    float shadowstepsize = 1.0 / (float)_ShadowSteps;
    float3 lightVec = normalize(mul((float3x3) unity_WorldToObject, _WorldSpaceLightPos0.xyz))  * shadowstepsize;
    float3 shadowDensity = 1.0 / _ShadowDensity * shadowstepsize;

    // threshold for shadow density
    float shadowThreshold = -log(_ShadowThreshold) / length(shadowDensity);

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

            #ifdef ENABLE_DIRECTIONAL_LIGHT
            [loop]
            for (int s = 0; s < _ShadowSteps; s++)
            {
                lpos += lightVec;
                float3 luv = GetUV(lpos);
                float lsample = SampleVolume(saturate(luv));

                shadowdist += lsample;

                float3 shadowboxtest = floor( 0.5 + ( abs( 0.5 - luv ) ) );
                float exitshadowbox = shadowboxtest .x + shadowboxtest .y + shadowboxtest .z;

                // check to exit shadow box
                if(shadowdist > shadowThreshold || exitshadowbox >= 1)
                {
                    break;
                }
            }
            #endif

            curdensity = saturate(cursample * _Intensity);
            float3 shadowterm = exp(-shadowdist * shadowDensity);
            float3 absorbedlight = shadowterm * curdensity;
            lightenergy += absorbedlight * transmittance;
            transmittance *= 1-curdensity;

            #ifdef ENABLE_AMBIENT_LIGHT
            shadowdist = 0;

            float3 luv = uv + float3(0,0,0.05);
            shadowdist += SampleVolume(saturate(luv));
            luv = uv + float3(0,0,0.1);
            shadowdist += SampleVolume(saturate(luv));
            luv = uv + float3(0,0,0.2);
            shadowdist += SampleVolume(saturate(luv));
            lightenergy += exp(-shadowdist * _AmbientDensity) * curdensity * _AmbientColor * transmittance;
            #endif
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

    fragOutput o;
    o.color = float4(lightenergy, 1-transmittance);
    o.depth = ComputeDepth(UnityObjectToClipPos(float4(depth, 1.0)));
    return o;
}

#endif
