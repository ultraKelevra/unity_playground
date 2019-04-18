#if !defined(SPRITE_SHADOWCASTER_INCLUDED)
#define SPRITE_SHADOWCASTER_INCLUDED

struct v2f { 
    V2F_SHADOW_CASTER;
};

v2f vert(appdata_base v)
{
    v2f o;
    TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
    return o;
}

float4 frag(v2f i) : SV_Target
{
    SHADOW_CASTER_FRAGMENT(i)
}
#endif