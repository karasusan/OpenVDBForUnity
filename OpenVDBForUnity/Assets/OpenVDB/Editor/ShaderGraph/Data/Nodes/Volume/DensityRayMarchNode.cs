using UnityEngine;
using UnityEditor.ShaderGraph;
using System.Reflection;

namespace OpenVDB
{
    [Title("Volume", "Density RayMarch Node")]
    public class DensityRayMarchingNode : CodeFunctionNode
    {
        public DensityRayMarchingNode()
        {
            name = "Density RayMarch Node";
        }

        public override string documentationURL
        {
            get { return "https://github.com/karasusan/OpenVDBForUnity/wiki/RayMarching-Node"; }
        }

        protected override MethodInfo GetFunctionToConvert()
        {
            return GetType().GetMethod("DensityRayMarch", BindingFlags.Static | BindingFlags.NonPublic);
        }

        static string DensityRayMarch(
            [Slot(1, Binding.None)] Texture3D Tex,
            [Slot(2, Binding.None)] SamplerState TexSamplerState,
            [Slot(3, Binding.None)] Vector1 MaxSteps,
            [Slot(4, Binding.None)] Vector3 WorldPos,
            [Slot(5, Binding.None)] Vector1 Density,
            [Slot(6, Binding.None)] Vector3 UVW,
            [Slot(7, Binding.None)] out Vector4 Out)
        {
            Out = Vector4.zero;
            return 
                @"
{
    float accumdist = 0;
    float curdensity = 0;
    float transmittance = 1;

    float3 dir = normalize(WorldPos - _WorldSpaceCameraPos);
    float3 localcamvec = normalize(mul((float3x3) unity_WorldToObject, dir));
    float3 localPos = mul(unity_WorldToObject, float4(WorldPos, 1)).xyz;
    float StepSize = 1.0 / MaxSteps;

    Density *= StepSize;

    for (int i = 0; i < MaxSteps; i++)
    {
        float cursample = SAMPLE_TEXTURE3D(Tex, TexSamplerState, saturate(localPos)).r;
        accumdist += cursample * StepSize;
        localPos += -localcamvec * StepSize;

        if(cursample > 0.001)
        {
            curdensity = saturate(cursample * Density);
            transmittance *= 1-curdensity;
        }
    }
    Out = float4( 1, 1, 1, transmittance);
}";
        }
    }
}