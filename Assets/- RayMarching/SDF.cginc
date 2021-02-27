#if !defined(SDFS)
#define SDFS

float sdSphere( float3 p, float s ) {
    return length(p)-s;
}

float sdBox( float3 p, float3 b ) {
    float3 d = abs(p) - b;
    return length(max(d,0.0)) + min(max(d.x,max(d.y,d.z)),0.0);
    // remove this line for an only partially signed sdf
}

float sdRoundBox( float3 p, float3 b, float r ) {
    float3 d = abs(p) - b;
    return length(max(d,0.0)) - r + min(max(d.x,max(d.y,d.z)),0.0);
    // remove this line for an only partially signed sdf
}

float sdTorus( float3 p, float2 t ) {
    float2 q = float2(length(p.xz)-t.x,p.y);
    return length(q)-t.y;
}

float sdCylinder( float3 p, float3 c ) {
    return length(p.xz-c.xy)-c.z;
}

float sdCone( float3 p, float2 c ) {
    // c must be normalized
    float q = length(p.xy);
    return dot(c,float2(q,p.z));
}

#endif