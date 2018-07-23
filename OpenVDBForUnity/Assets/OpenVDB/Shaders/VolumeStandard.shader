Shader "OpenVDB/Standard"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _Volume ("Volume", 3D) = "" {}
        _Intensity ("Intensity", Range(0.0, 100.0)) = 1.2
        _Threshold ("Threshold", Range(0.0, 1.0)) = 0.95
        _StepDistance ("StepDistance", Range(0.01, 0.1)) = 0.02
        _ShadowSteps ("ShadowSteps", Range(1, 32)) = 16
        _ShadowDensity ("ShadowDensity", Range(1, 10)) = 5
    }

    CGINCLUDE

    ENDCG

    SubShader {
        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha
        // ZTest Always

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