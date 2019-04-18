Shader "Unlit/glitch_1"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Mask("Mask",2D) = "white" {}
		_MaskRange("MaskRange",Range(0,1)) = 0.25
		_CutRange("CutRange",Range(0,1)) = 0.5
		_Tint("Tint",Color)=(1,1,1,1)
		_DisappearMask("Disappear Mask",2D)="white"{}
		_Disappear("Disapear", Range(0,1))=0
		[Toggle(DISAPPEARBYBASEMASK)] _DISAPPEARBYBASEMASK("DisappearByBaseMask",Float)=0
	}
	SubShader
	{
		Tags
		{ 
			"Queue"="AlphaTest" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent"
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		Blend One OneMinusSrcAlpha
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

            #pragma shader_feature DISAPPEARBYBASEMASK

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				half4 vertCol: COLOR;
			};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				
				#if !DISAPPEARBYBASEMASK
				float2 disappearMaskUV: TEXCOORD1;
				#endif
				
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				half4 color: COLOR;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _Mask;
			float4 _Mask_ST;
			float _MaskRange;
			float _CutRange;
			half4 _Tint;
			half _Disappear;
			
			#if !DISAPPEARBYBASEMASK
			sampler2D _DisappearMask;
			float4 _DisappearMask_ST;
			#endif
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.uv, _Mask);
				#if !DISAPPEARBYBASEMASK
				o.disappearMaskUV=TRANSFORM_TEX(v.uv,_DisappearMask);
				#endif
				UNITY_TRANSFER_FOG(o,o.vertex);
				o.color = v.vertCol*_Tint;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{			    
			    half4 maskSample = tex2D(_Mask, i.uv.zw);
			    
			    #if DISAPPEARBYBASEMASK
			    clip(maskSample.r - _Disappear);
			    #else
			    half4 disappearMaskSample = tex2D(_DisappearMask, i.disappearMaskUV.xy);
			    clip(disappearMaskSample.r - _Disappear);
			    #endif
			    
			    fixed4 col;
			    if(maskSample.r < _MaskRange){
			        col = tex2D(_MainTex, i.uv.xy);
			    }
			    else{
			        col = tex2D(_MainTex, half2(_CutRange,i.uv.y));
			    }
			    clip(col.a-0.5);
			    col *= i.color;
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
