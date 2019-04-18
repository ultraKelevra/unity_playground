#if !defined(TOON_LIGHTING_INCLUDED)
#define TOON_LIGHTING_INCLUDED

#include "UnityPBSLighting.cginc"
#include "AutoLight.cginc"

half4 _Tint;
sampler2D _MainTex;
float4 _MainTex_ST;
half _Smoothness;
half _Metallic;
#if BUMP
sampler2D _BumpMap;
float4 _BumpMap_ST;
#endif
#if TOON
sampler2D _ToonRamp;
#endif
half _Cutoff;
#if MASK
sampler2D _Mask;
float4 _Mask_ST;
half _MaskClip;
#endif


struct VertexData {
    float4 position : POSITION;
    float3 normal : NORMAL;
    float2 uv : TEXCOORD0;
    
    #if BUMP
    float4 tangent : TANGENT;
    #endif
    
    #if VERTEXCOLOR
    half4 color: COLOR;
    #endif
};

struct Interpolators {
    float4 position : SV_POSITION;
    #if BUMP
    float4 uv : TEXCOORD0;
    #else
    float2 uv:TEXCOORD0;
    #endif
    float3 worldPos : TEXCOORD1;
    
    #if defined(VERTEXLIGHT_ON)
		float3 vertexLightColor : TEXCOORD2;
	#endif
	
	#if VERTEXCOLOR
	half4 color: COLOR;
	#endif
	
	#if BUMP
    half3 tspace0 : TEXCOORD3; // tangent.x, bitangent.x, normal.x
    half3 tspace1 : TEXCOORD4; // tangent.y, bitangent.y, normal.y
    half3 tspace2 : TEXCOORD5; // tangent.z, bitangent.z, normal.z
    #else
    half3 normal: TEXCOORD3;
    #endif
    
    #if MASK
    float2 maskUV: TEXCOORD6;
    #endif
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
    
    #ifdef PIXELSNAP_ON
	i.vertex = UnityPixelSnap (i.vertex);
	#endif
		
    i.worldPos = mul(unity_ObjectToWorld, v.position);
    float3 wNormal = UnityObjectToWorldNormal(v.normal);
    i.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);

    #if VERTEXCOLOR
    i.color=v.color*_Tint;
    #endif

    #if BUMP && AUTOBUMPUV
    i.uv.zw = TRANSFORM_TEX(v.position.xy, _BumpMap);
    #elif BUMP
    i.uv.zw = TRANSFORM_TEX(v.uv, _BumpMap);
    #endif

    #if BUMP
    half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
    // compute bitangent from cross product of normal and tangent
    half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
    half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
    // output the tangent space matrix
    i.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
    i.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
    i.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);
    #else
    i.normal=UnityObjectToWorldNormal(v.normal);
    #endif
    #if MASK
    i.maskUV=TRANSFORM_TEX(v.uv, _Mask);
    #endif
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
    #if VERTEXCOLOR
    half4 texSample = tex2D(_MainTex, i.uv.xy) * i.color;
    #else
    half4 texSample = tex2D(_MainTex, i.uv.xy);
    #endif
    
    //clip
    clip(texSample.a-_Cutoff);
    #if MASK
    clip(tex2D(_Mask,i.maskUV).r-_MaskClip);
    #endif
    
    #if BUMP
    // sample the normal map, and decode from the Unity encoding
    half3 tnormal = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
    // transform normal from tangent to world space
    half3 worldNormal;
    worldNormal.x = dot(i.tspace0, tnormal);
    worldNormal.y = dot(i.tspace1, tnormal);
    worldNormal.z = dot(i.tspace2, tnormal);
    #else
    half3 worldNormal=i.normal;
    #endif
    
    float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
    
    float3 albedo = texSample.rgb;

	float3 specularTint;
	float oneMinusReflectivity;
	albedo = DiffuseAndSpecularFromMetallic(
		albedo, _Metallic, specularTint, oneMinusReflectivity
	);
	
	half3 BRDF_light = UNITY_BRDF_PBS(
		albedo, specularTint,
		oneMinusReflectivity, _Smoothness,
		worldNormal, viewDir,
		CreateLight(i,worldNormal), CreateIndirectLight(i,worldNormal)
	);
	#if TOON
	half grayContribution = BRDF_light;
	grayContribution=tex2D(_ToonRamp,half2(grayContribution,0));
	half3 colorContribution = normalize(BRDF_light.rgb);
	return fixed4(texSample.rgb * grayContribution * colorContribution, texSample.a);
	#else
	return fixed4(BRDF_light,texSample.a);
	#endif
}
#endif