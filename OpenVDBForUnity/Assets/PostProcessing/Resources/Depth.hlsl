#include "PostProcessing/Shaders/StdLib.hlsl"
#include "PostProcessing/Shaders/Colors.hlsl"

TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);

half4 Frag(VaryingsDefault i) : SV_Target
{
    return SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord);
}