Shader "Unlit/line"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_LineInclination("Incl",Range(-5,5))=0
		_LineXOffset("Offset",Range(0,1))=0
		_LineYOffset("Offset",Range(0,1))=0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			half _LineXOffset;
			half _LineYOffset;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			half _LineInclination;
			fixed4 frag (v2f i) : SV_Target
			{
			half yInThisX = (i.uv.x*2-1)*_LineInclination+_LineYOffset;
			if((i.uv.y*2-1)<yInThisX)
    			clip(-1);
				fixed4 col = tex2D(_MainTex, i.uv);
				
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
