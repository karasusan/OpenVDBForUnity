using UnityEngine;
using UnityEditor.ShaderGraph;
using System.Reflection;

namespace OpenVDB
{
    [Title("Volume", "RayMarching Node")]
    public class RayMarchingNode : CodeFunctionNode
    {
        public RayMarchingNode()
        {
            name = "RayMarching Node";
        }

        public override string documentationURL
        {
            get { return "https://github.com/karasusan/OpenVDBForUnity/wiki/RayMarching-Node"; }
        }

        protected override MethodInfo GetFunctionToConvert()
        {
            return GetType().GetMethod("MyCustomFunction",
                BindingFlags.Static | BindingFlags.NonPublic);
        }

        static string MyCustomFunction(
            [Slot(1, Binding.None)] Texture3D Texture,
            [Slot(2, Binding.None)] Vector1 Intensity,
            [Slot(3, Binding.None)] Vector1 Threshold,
            [Slot(4, Binding.None)] out Vector3 Out)
        {
            Out = Vector3.zero;
            return 
                @"
{ 
    Out = {precision}3(Intensity, Intensity, Intensity); 
}";
        }
    }
}