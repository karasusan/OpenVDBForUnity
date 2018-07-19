using UnityEngine;
using UnityEditor.ShaderGraph;
using System.Reflection;

namespace OpenVDB
{
    [Title("Volume", "ObjectSpaceDistance")]
    public class ObjectSpaceDistanceNode : CodeFunctionNode
    {
        public ObjectSpaceDistanceNode()
        {
            name = "Object Space Distance Node";
        }

        public override string documentationURL
        {
            get { return "https://github.com/karasusan/OpenVDBForUnity/wiki/RayMarching-Node"; }
        }

        protected override MethodInfo GetFunctionToConvert()
        {
            return GetType().GetMethod("ObjectSpaceDistance",
                BindingFlags.Static | BindingFlags.NonPublic);
        }

        static string ObjectSpaceDistance(
            [Slot(1, Binding.None)] Vector3 In,
            [Slot(2, Binding.None)] out Vector1 Out)
        {
            return 
                @"{ Out = DistanceFunction(In); }";
        }

        public override void GenerateNodeFunction(FunctionRegistry registry, GraphContext graphContext, GenerationMode generationMode)
        {
            registry.ProvideFunction("roundBox", s => s.Append(@"
float roundBox(float3 pos, float3 size, float round)
{
    return length(max(abs(pos) - size * 0.5, 0.0)) - round;
}
"));

            registry.ProvideFunction("mod", s => s.Append(@"
float3 mod(float3 a, float3 b)
{
    return frac(abs(a / b)) * abs(b);
}
"));
            registry.ProvideFunction("repeat", s => s.Append(@"
float3 repeat(float3 pos, float3 span)
{
    return mod(pos, span) - span * 0.5;
}
"));
            
            registry.ProvideFunction("DistanceFunction", s => s.Append(@"
float DistanceFunction(float3 pos)
{
    pos = repeat(pos, 0.3);
    return roundBox(pos, 0.1, 0.01);
    return 0.0;
}
"));
            base.GenerateNodeFunction(registry, graphContext, generationMode);
        }
    }
}