Shader "Hidden/Kazuki/PostProcessing/Depth"
{
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex VertDefault
            #pragma fragment Frag
            #include "Depth.hlsl"
            ENDHLSL
        }
    }
}
