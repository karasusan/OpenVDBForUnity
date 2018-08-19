Shader "OpenVDB/Standard"
{
    Properties
    {
        _Volume ("Volume", 3D) = "" {}
        _Intensity ("Intensity", Range(0.1, 2.0)) = 0.3
        _StepDistance ("StepDistance", Range(0.01, 0.05)) = 0.01
        _ShadowSteps ("ShadowSteps", Range(1, 64)) = 32
        _ShadowDensity ("ShadowDensity", Color) = (0.4, 0.4, 0.4, 1)
        _ShadowThreshold ("ShadowThreshold", Range(0.001, 0.1)) = 0.001
        _AmbientColor ("AmbientColor", Color) = (0.4, 0.4, 0.5, 1)
        _AmbientDensity ("AmbientDensity", Range(0, 1)) = 0.2
        [KeywordEnum(Off, Front, Back)] _Cull("Culling", Int) = 0
    }

    CGINCLUDE

    #define ENABLE_CAMERA_INSIDE_CUBE
    #define ENABLE_SAMPLING_START_OFFSET
    #define ENABLE_TRACE_DISTANCE_LIMITED
    #define ENABLE_DIRECTIONAL_LIGHT
    #define ENABLE_AMBIENT_LIGHT

    ENDCG

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
        }
        Cull [_Cull]
        Blend SrcAlpha OneMinusSrcAlpha


        Pass
        {
            Tags
            {
                "RenderPipeline" = "HDRenderPipeline"
                "RenderType"="Transparent"
            }

            ZWrite On

            CGPROGRAM

            #include "./VolumeStandard.cginc"
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }

        // TODO:: Implement the ShadowCaster Pass for HDRP
        /*
        Pass
        {
            Tags{ "LightMode" = "ShadowCaster" }

            CGPROGRAM
            #include "./VolumeShadowCaster.cginc"
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            // #pragma fragmentoption ARB_precision_hint_fastest
            ENDCG
        }
        */
    }
}
