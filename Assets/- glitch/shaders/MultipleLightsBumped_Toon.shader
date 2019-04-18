Shader "Custom/Multiplelights_Bumped_Toon" {
    Properties {
		_Tint ("Tint", Color) = (1, 1, 1, 1)
		_MainTex ("Albedo", 2D) = "white" {}
		_Metallic ("Metallic", Range(0, 1)) = 0
		_Smoothness ("Smoothness", Range(0, 1)) = 0.1
		[Toggle(BUMP)] _Bump("Use BumpMap", Float) = 0
		_BumpMap("bumpMap", 2D) = "white" {}
		[Toggle(TOON)] _UseToonRamp("Light by Toon ramp", Float)=0
		_ToonRamp("ToonRamp", 2D) = "gray" {}
		_Cutoff("Cutoff", Range(0,1)) = 0.5
		[Toggle(AUTOBUMPUV)] _AutoUV("AutoBumpUV", Float) = 0
		[Toggle(VERTEXCOLOR)] _UseVertexColor("Use Vertex Color", Float) = 0
		[Toggle(MASK)] _UseMask("UseMask", Float)=0
		_Mask("Mask",2D)="white"{}
		_MaskClip("MaskClip",Range(0,1))=0.5
	}
	//SPRITE COLOR WITH 
	//--DEPTH WRITE
	SubShader {
	    Tags{"Border"="Solid"}
		Pass {
            Tags
            {
                "Queue"="AlphaTest"
                "IgnoreProjector"="True"
                "PreviewType"="Plane"
                "CanUseSpriteAtlas"="True"
                "LightMode" = "ForwardBase"
            }
            
            Cull Off
            ZWrite On
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#pragma target 3.0

			#pragma multi_compile _VERTEXLIGHT_ON
            #pragma multi_compile _PIXELSNAP_ON
			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram
            #pragma shader_feature VERTEXCOLOR
            #pragma shader_feature MASK
			#define FORWARD_BASE_PASS

			#include "ToonLighting.cginc"

			ENDCG
		}

		Pass
		{
			Tags {"LightMode" = "ForwardAdd"}
			
            Blend One One
            Cull Off
			CGPROGRAM

			#pragma target 3.0

			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram

            #pragma multi_compile_fwdadd
            #pragma shader_feature AUTOBUMPUV
            #pragma shader_feature VERTEXCOLOR
            #pragma shader_feature BUMP
            #pragma shader_feature MASK
            #pragma shader_feature TOON
            
			#include "ToonLighting.cginc"

			ENDCG
		}
		
		Pass
		{
			Tags
			{
				"LightMode" = "ShadowCaster"
				"Queue"="AlphaTest"
				"IgnoreProjector"="True"
				"CanUseSpriteAtlas"="True"
			}

			ZWrite On
			ZTest LEqual
			Cull Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile_shadowcaster
			#include "UnityCG.cginc"
			
			#include "SpriteDepthWrite.cginc"
			
			ENDCG
		}
	}
}