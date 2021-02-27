#if !defined(RAYMARCHING)
#define RAYMARCHING

#include "Random.cginc"
#include "SDF.cginc"
static const uint MAX_BOUNCES = 16;
static const uint MAX_STEPS = 1000;
static const uint MAX_DISTANCE = 10000;
static const float MIN_DISTANCE_TO_SURFACE = 0.001f;

struct RayHit{
    float3 position;
    float3 normal;
    float distance;
};

float DistToScene(float3 p){
    //float4 sphere1 = float4(0,1,0,1);
    //float4 sphere2 = float4(1,.5,0,.5);
    //float4 sphere3 = float4(-1,.25,1,.25);
    //float4 sphere4 = float4(1.5,.35,1,.35);
    //float dS1 = length(p-sphere1.xyz) - sphere1.w;
    //float dS2 = length(p-sphere2.xyz) - sphere2.w;
    //float dS3 = length(p-sphere3.xyz) - sphere3.w;
    //float dS4 = length(p-sphere4.xyz) - sphere4.w;
    float dC = sdRoundBox(p, float3(1,1,1), 1);
    float dP = p.y;
    float d = min(dP, dC);
    //float d = min(d,min(dP,min(dS1,min(dS2,min(dS3,dS4)))));
    return d;
}

half3 GetNormal(float3 p){
    float d = DistToScene(p);
    half3 n = half3(
    d - DistToScene(p - float3(MIN_DISTANCE_TO_SURFACE, 0, 0)),
    d - DistToScene(p - float3(0, MIN_DISTANCE_TO_SURFACE, 0)),
    d - DistToScene(p - float3(0, 0, MIN_DISTANCE_TO_SURFACE)));
    return normalize(n);
}

RayHit RayMarch(Ray ray){
    float distance = 0;
    float3 p = ray.origin;

    for(uint i = 0.; i < MAX_STEPS; i++){
        float distanceToScene = DistToScene(p);
        distance += distanceToScene;
        if(distanceToScene < MIN_DISTANCE_TO_SURFACE || distance > MAX_DISTANCE) break;
        p += distanceToScene*ray.direction;
    }

    RayHit hit;
    hit.distance = distance;
    hit.position = p;
    hit.normal = GetNormal(p);
    return hit;
}

float3 TravelScene(Ray ray)
{
    float3 specular = float3(0.6f, 0.6f, 0.6f);
    float3 albedo = float3(0.8f, 0.8f, 0.8f);
    float amplitude =.05f;
    
    float3 result = 0;

    for (int i = 0; i < MAX_BOUNCES; i++)
    {
        RayHit hit = RayMarch(ray);
        
        if (hit.distance < MAX_DISTANCE){//collide something
            ray.origin = hit.position + hit.normal * MIN_DISTANCE_TO_SURFACE;
            ray.direction = reflect(ray.direction,normalize(hit.normal + float3(rand(),rand(),rand())*amplitude));
            ray.energy *= specular;
        }
        else //reached "sky"
        {
            float theta = acos(ray.direction.y) / - PI;
            float phi = atan2(ray.direction.x, - ray.direction.z) / -PI * 0.5f;
            result += ray.energy * _SkyboxTexture.SampleLevel(sampler_SkyboxTexture, float2(phi, theta), 0).xyz;
            break;
        }
     
        if (!any(ray.energy))
            break;
    }
    return result;
}

#endif