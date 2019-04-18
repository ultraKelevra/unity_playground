#if !defined(SPRITE_DEPTHWRITE_INCLUDED)
#define SPRITE_DEPTHWRITE_INCLUDED

struct appdata_t
{
    float4 vertex   : POSITION;
    float2 texcoord : TEXCOORD0;
};

struct v2f
{
    float4 vertex   : SV_POSITION;
    half2 texcoord  : TEXCOORD0;
};

half _Cutoff;
half _LineSort;

v2f vert(appdata_t v)
{
    v2f OUT;
    OUT.vertex = UnityObjectToClipPos(v.vertex);
    OUT.texcoord.xy = v.texcoord;
    #ifdef PIXELSNAP_ON
    OUT.vertex = UnityPixelSnap (OUT.vertex);
    #endif
    return OUT;
}

sampler2D _MainTex;

fixed4 frag(v2f IN) : SV_Target
{
    clip(tex2D(_MainTex, IN.texcoord.xy).a-_Cutoff);
    return _LineSort;
}
#endif