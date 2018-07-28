Shader "OpenVDB/Standard"
{
    Properties
    {
        _Volume ("Volume", 3D) = "" {}
        _Intensity ("Intensity", Range(0.1, 2.0)) = 0.3
        _StepDistance ("StepDistance", Range(0.01, 0.05)) = 0.01
        _ShadowSteps ("ShadowSteps", Range(1, 64)) = 32
        _ShadowDensity ("ShadowDensity", Color) = (0.4, 0.4, 0.4, 1)
        _AmbientColor ("AmbientColor", Color) = (0.4, 0.4, 0.5, 1)
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


        Pass
        {
            ZWrite Off

            CGPROGRAM
            #include "./VolumeStandard.cginc"
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }

        Pass
        {
            Tags{ "LightMode" = "ShadowCaster" }
            ZWrite On ZTest LEqual

            CGPROGRAM
            #include "./VolumeShadowCaster.cginc"
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            ENDCG
        }
    }
}