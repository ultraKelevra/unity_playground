#if !defined(MY_LIGHTING_INCLUDED)
#define MY_LIGHTING_INCLUDED

#include "UnityPBSLighting.cginc"
#include "AutoLight.cginc"

half4 _Tint;
sampler2D _MainTex;
float4 _MainTex_ST;
float _Smoothness;
float _Metallic;
sampler2D _BumpMap;
float4 _BumpMap_ST;
sampler2D _ToonRamp;

struct VertexData {
    float4 position : POSITION;
    float3 normal : NORMAL;
    float2 uv : TEXCOORD0;
    float4 tangent : TANGENT;
};

struct Interpolators {
    float4 position : SV_POSITION;
    float4 uv : TEXCOORD0;
    float3 worldPos : TEXCOORD1;
    #if defined(VERTEXLIGHT_ON)
		float3 vertexLightColor : TEXCOORD2;
	#endif
	
    half3 tspace0 : TEXCOORD3; // tangent.x, bitangent.x, normal.x
    half3 tspace1 : TEXCOORD4; // tangent.y, bitangent.y, normal.y
    half3 tspace2 : TEXCOORD5; // tangent.z, bitangent.z, normal.z
};

UnityLight CreateLight (Interpolators i,float3 wNormal) {
	UnityLight light;
	
	#if defined(POINT) || defined(POINT_COOKIE) || defined(SPOT)
		light.dir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);
	#else
		light.dir = _WorldSpaceLightPos0.xyz;
	#endif
	
	UNITY_LIGHT_ATTENUATION(attenuation, 0, i.worldPos);
	light.color = _LightColor0.rgb * attenuation;
	light.ndotl = DotClamped(wNormal, light.dir);
	return light;
}

void ComputeVertexLightColor (inout Interpolators i, float3 wNormal) {
    #if defined(VERTEXLIGHT_ON)
		i.vertexLightColor = Shade4PointLights(
			unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
			unity_LightColor[0].rgb, unity_LightColor[1].rgb,
			unity_LightColor[2].rgb, unity_LightColor[3].rgb,
			unity_4LightAtten0, i.worldPos, wNormal
		);
	#endif
}

Interpolators MyVertexProgram (VertexData v) {
    Interpolators i;
    i.position = UnityObjectToClipPos(v.position);
    i.worldPos = mul(unity_ObjectToWorld, v.position);
    float3 wNormal = UnityObjectToWorldNormal(v.normal);
    i.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
    i.uv.zw = TRANSFORM_TEX(v.uv, _BumpMap);
    
     half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
     // compute bitangent from cross product of normal and tangent
     half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
     half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
     // output the tangent space matrix
     i.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
     i.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
     i.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);
    
    ComputeVertexLightColor(i,wNormal);
    return i;
}

UnityIndirect CreateIndirectLight (Interpolators i, float3 wNormal) {
	UnityIndirect indirectLight;
	indirectLight.diffuse = 0;
	indirectLight.specular = 0;

	#if defined(FORWARD_BASE_PASS)
		indirectLight.diffuse += max(0, ShadeSH9(float4(wNormal, 1)));
	#endif
	
	indirectLight.diffuse += max(0, ShadeSH9(float4(wNormal, 1)));

	return indirectLight;
}


float4 MyFragmentProgram (Interpolators i) : SV_TARGET {
    // sample the normal map, and decode from the Unity encoding
    half3 tnormal = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
    // transform normal from tangent to world space
    half3 worldNormal;
    worldNormal.x = dot(i.tspace0, tnormal);
    worldNormal.y = dot(i.tspace1, tnormal);
    worldNormal.z = dot(i.tspace2, tnormal);

    float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
    
    half4 texSample = tex2D(_MainTex, i.uv.xy) * _Tint;
    
    float3 albedo = texSample.rgb;

	float3 specularTint;
	float oneMinusReflectivity;
	albedo = DiffuseAndSpecularFromMetallic(
		albedo, _Metallic, specularTint, oneMinusReflectivity
	);
	
	half lightContribution = UNITY_BRDF_PBS(
		albedo, specularTint,
		oneMinusReflectivity, _Smoothness,
		worldNormal, viewDir,
		CreateLight(i,worldNormal), CreateIndirectLight(i,worldNormal)
	);
	half toonContribution = tex2D(_ToonRamp,half2(lightContribution,0));
	return texSample * toonContribution;
}
#endif