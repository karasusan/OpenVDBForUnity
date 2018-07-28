#ifndef __VOLUME_SHADOWCASTER_INCLUDED__
#define __VOLUME_SHADOWCASTER_INCLUDED__

#include "UnityCG.cginc"
#include "Camera.cginc"

struct appdata
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2f
{
    float4 hpos : TEXCOORD0;
    float2 uv : TEXCOORD1;
    float4 pos : SV_POSITION;
};

struct fragOutput 
{
    fixed4 color : SV_Target;
    float depth : SV_Depth;
};

float3 deformVertex(float3 original) 
{
    original.y += 0.5;
    original.xz *= original.y;
    return original;
}

v2f vert(appdata v)
{
    v.vertex.xyz = deformVertex(v.vertex.xyz);
    v2f o;
    TRANSFER_SHADOW_CASTER(o)
    return o;
}

void frag(v2f i, out float4 outColor : SV_Target, out float outDepth : SV_Depth)
{
    /*
    float4 opos = mul(unity_WorldToObject, float4(pos, 1.0));
    opos = UnityClipSpaceShadowCasterPos(opos, i.normal);
    opos = UnityApplyLinearShadowBias(opos);

    outColor = outDepth = opos.z / opos.w;
    */
}

#endif 