using UnityEngine;
using UnityEditor.ShaderGraph;
using System.Reflection;

namespace OpenVDB
{
    [Title("Volume", "WorldToLocal")]
    public class WorldToLocalNode : CodeFunctionNode
    {
        public WorldToLocalNode()
        {
            name = "World To Local Node";
        }

        public override string documentationURL
        {
            get { return "https://github.com/karasusan/OpenVDBForUnity/wiki/RayMarching-Node"; }
        }

        protected override MethodInfo GetFunctionToConvert()
        {
            return GetType().GetMethod("WorldToLocal",
                BindingFlags.Static | BindingFlags.NonPublic);
        }

        static string WorldToLocal(
            [Slot(1, Binding.None)] Vector3 In,
            [Slot(2, Binding.None)] out Vector3 Out)
        {
            Out = Vector3.zero;
            return 
                @"
{ 
    Out = mul(unity_WorldToObject, {precision}4(In, 1.0)).xyz;
}";
        }
    }
}