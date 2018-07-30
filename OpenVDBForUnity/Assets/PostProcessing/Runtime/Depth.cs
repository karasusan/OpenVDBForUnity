using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace PostProcessing
{
    #region Effect settings

    [System.Serializable]
    [PostProcess(typeof(DepthRenderer), PostProcessEvent.AfterStack, "Kazuki/Depth")]
    public sealed class Depth : PostProcessEffectSettings
    {
    }

    #endregion

    #region Effect renderer

    sealed class DepthRenderer : PostProcessEffectRenderer<Depth>
    {
        static class ShaderIDs
        {
            internal static readonly int Opacity = Shader.PropertyToID("_Opacity");
            internal static readonly int Direction = Shader.PropertyToID("_Direction");
        }

        public override void Init()
        {
        }

        public override void Render(PostProcessRenderContext context)
        {

            var cmd = context.command;
            cmd.BeginSample("Depth");

            var sheet = context.propertySheets.Get(Shader.Find("Hidden/Kazuki/PostProcessing/Depth"));
            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);

            cmd.EndSample("Depth");
        }
    }

    #endregion
}
