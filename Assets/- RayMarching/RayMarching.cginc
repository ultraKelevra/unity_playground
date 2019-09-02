#if !defined(RAYMARCHING)
#define RAYMARCHING

static const uint MAX_BOUNCES = 8;
static const uint MAX_STEPS = 1000;
static const uint MAX_DISTANCE = 10000;
static const float SURFACE_MIN_DISTANCE = 0.001f;

struct RayHit{
    float3 position;
    float3 normal;
    float distance;
};

float DistToScene(float3 p){
    float4 sphere = float4(0,1,0,1);
    float dS = length(p-sphere.xyz) - sphere.w;
    float dP = p.y;
    float d = min(dS,dP);
    return d;
}

half3 GetNormal(float3 p){
    float d = DistToScene(p);
    half3 n = half3(
    d - DistToScene(p - float3(SURFACE_MIN_DISTANCE, 0, 0)),
    d - DistToScene(p - float3(0, SURFACE_MIN_DISTANCE, 0)),
    d - DistToScene(p - float3(0, 0, SURFACE_MIN_DISTANCE)));
    return normalize(n);
}

RayHit RayMarch(Ray ray){
    float distance = 0;
    float3 p = ray.origin;

    for(uint i = 0.; i < MAX_STEPS; i++){
        float distanceToScene = DistToScene(p);
        distance += distanceToScene;
        if(distanceToScene < SURFACE_MIN_DISTANCE || distance > MAX_DISTANCE) break;
        p += distanceToScene*ray.direction;
    }

    RayHit hit;
    hit.distance = distance;
    hit.position = p;
    hit.normal = GetNormal(p);
    return hit;
}

float3 Shade(inout Ray ray, RayHit hit)
{
    if (hit.distance < MAX_DISTANCE)
    {
        float3 specular = float3(0.6f, 0.6f, 0.6f);
            float3 albedo = float3(0.8f, 0.8f, 0.8f);
        // Reflect the ray and multiply energy with specular reflection
        ray.origin = hit.position + hit.normal * 0.001f;
        ray.direction = reflect(ray.direction, hit.normal);
        ray.energy *= specular;
        // Shadow test ray
        
        bool shadow = false;
        Ray shadowRay = CreateRay(hit.position + hit.normal * 0.001f, -1 * _DirectionalLight.xyz);
        RayHit shadowHit = RayMarch(shadowRay);
        if (shadowHit.distance < MAX_DISTANCE)
        {
            return float3(0.0f, 0.0f, 0.0f);
        }
        // Return diffuse
        return saturate(dot(hit.normal, _DirectionalLight.xyz) * -1) * _DirectionalLight.w * albedo;
    }
    else
    {
        // Erase the ray's energy - the sky doesn't reflect anything
        ray.energy = 0.0f;
        // Sample the skybox and write it
        float theta = acos(ray.direction.y) / -PI;
        float phi = atan2(ray.direction.x, -ray.direction.z) / -PI * 0.5f;
        return _SkyboxTexture.SampleLevel(sampler_SkyboxTexture, float2(phi, theta), 0).xyz;
    }
}
#endif