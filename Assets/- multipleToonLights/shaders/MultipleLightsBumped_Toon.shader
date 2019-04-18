Shader "Custom/Multiplelights_Bumped_Toon" {
    Properties {
		_Tint ("Tint", Color) = (1, 1, 1, 1)
		_MainTex ("Albedo", 2D) = "white" {}
		_Metallic ("Metallic", Range(0, 1)) = 0
		_Smoothness ("Smoothness", Range(0, 1)) = 0.1
		_BumpMap("bumpMap",2D) = "white" {}
		_ToonRamp("ToonRamp",2D) = "gray" {}
	}
	SubShader {

		Pass {
			Tags {
				"LightMode" = "ForwardBase"
			}

			CGPROGRAM

			#pragma target 3.0

			#pragma multi_compile _ VERTEXLIGHT_ON

			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram

			#define FORWARD_BASE_PASS

			#include "My Lighting Bumped_Toon.cginc"

			ENDCG
		}

		Pass {
			Tags {
				"LightMode" = "ForwardAdd"
			}
            Blend One One
            
			CGPROGRAM

			#pragma target 3.0

			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram

            //#pragma multi_compile DIRECTIONAL DIRECTIONAL_COOKIE POINT SPOT
            #pragma multi_compile_fwdadd
            
			#include "My Lighting Bumped_Toon.cginc"

			ENDCG
		}
	}
}