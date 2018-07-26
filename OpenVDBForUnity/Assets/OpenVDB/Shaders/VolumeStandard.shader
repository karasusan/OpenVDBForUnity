Shader "OpenVDB/Standard"
{
    Properties
    {
        _Volume ("Volume", 3D) = "" {}
        _Intensity ("Intensity", Range(0.1, 2.0)) = 1
        _StepDistance ("StepDistance", Range(0.01, 0.1)) = 0.02
        _ShadowSteps ("ShadowSteps", Range(1, 64)) = 16
        _ShadowDensity ("ShadowDensity", Color) = (1, 1, 1, 1)
        _AmbientColor ("AmbientColor", Color) = (1, 1, 1, 1)
        _AmbientDensity ("AmbientDensity", Range(0, 1)) = 0.7
        [KeywordEnum(Off, Front, Back)] _Cull("Culling", Int) = 2
    }

    CGINCLUDE

    ENDCG

    SubShader 
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        Cull [_Cull]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

            #define ITERATIONS 100
            #include "./VolumeStandard.cginc"
            #pragma vertex vert
            #pragma fragment frag

            ENDCG
        }
    }
}