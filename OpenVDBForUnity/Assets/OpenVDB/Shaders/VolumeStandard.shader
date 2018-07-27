Shader "OpenVDB/Standard"
{
    Properties
    {
        _Volume ("Volume", 3D) = "" {}
        _Intensity ("Intensity", Range(0.1, 2.0)) = 0.5
        _StepDistance ("StepDistance", Range(0.01, 0.1)) = 0.02
        _ShadowSteps ("ShadowSteps", Range(1, 64)) = 32
        _ShadowDensity ("ShadowDensity", Color) = (0.4, 0.4, 0.4, 1)
        _AmbientColor ("AmbientColor", Color) = (0.2, 0.2, 0.5, 1)
        _AmbientDensity ("AmbientDensity", Range(0, 1)) = 0.2
        [KeywordEnum(Off, Front, Back)] _Cull("Culling", Int) = 0
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

        ZWrite Off

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