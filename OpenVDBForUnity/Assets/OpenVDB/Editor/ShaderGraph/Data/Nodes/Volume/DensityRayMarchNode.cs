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
            [Slot(2, Binding.None)] Vector1 MaxSteps,
            [Slot(3, Binding.None)] Vector3 CamDir,
            [Slot(4, Binding.None)] Vector3 LocalPos,
            [Slot(5, Binding.None)] out Vector1 Out)
        {
            return 
                @"
{
    float accumdist = 0;
    //float3 localcamvec = normalize( mul(CamDir, (float3x3)unity_WorldToObject) );
    float3 localcamvec = normalize( CamDir );
    float StepSize = 1.0 / MaxSteps;

    for (int i = 0; i < MaxSteps; i++)
    {
        float cursample = 0.0;
        accumdist += cursample * StepSize;
        LocalPos += -localcamvec * StepSize;
    }
    Out = accumdist;
}";
        }
    }
}