Shader "Unlit/RoundMesh"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MinNL("Min N.L", Range(0,1)) = 0
        _MaxNL("Max N.L", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

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
                half4 color: COLOR;
                half3 normal: NORMAL;
            };

            struct v2f
            {
                UNITY_FOG_COORDS(0)
                float4 vertex : SV_POSITION;
                half4 color: COLOR;
                half3 normal: NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

	        float _HorizontalScale;
	        float _VerticalScale;
	        float4 _SphereOffset;
	        float _Radius;
	        
	        float _PivotRotationX;
	        float _PivotRotationY;
	        float _PivotRotationZ;
	        
	        float _CoordinateInterpolation;
	        float4 _WorldOffset;
	        
	        inline float3x3 xRotation3dRadians(float rad) {
                float s = sin(rad);
                float c = cos(rad);
                return float3x3(
                    1, 0, 0,
                    0, c, s,
                    0, -s, c);
            }
             
            inline float3x3 yRotation3dRadians(float rad) {
                float s = sin(rad);
                float c = cos(rad);
                return float3x3(
                    c, 0, -s,
                    0, 1, 0,
                    s, 0, c);
            }
             
            inline float3x3 zRotation3dRadians(float rad) {
                float s = sin(rad);
                float c = cos(rad);
                return float3x3(
                    c, s, 0,
                    -s, c, 0,
                    0, 0, 1);
            }

	        
            v2f vert (appdata v)
            {
                v2f o;
                
                float3 wPos = mul(unity_ObjectToWorld, v.vertex);
                
                float3 polar = float3((wPos.z + _WorldOffset.z)/_HorizontalScale, wPos.y * _VerticalScale + _Radius, (-wPos.x + _WorldOffset.x)/_HorizontalScale);
                float3 cartesian = float3
                (cos(polar.z) * sin(polar.x),
                cos(polar.x),
                sin(polar.z) * sin(polar.x))
                * polar.y;
                
                
                float3 finalPoint = cartesian;
                finalPoint = mul(xRotation3dRadians(_PivotRotationX*3.141592f), finalPoint);
                finalPoint = mul(yRotation3dRadians(_PivotRotationY*3.141592f), finalPoint);
                finalPoint = mul(zRotation3dRadians(_PivotRotationZ*3.141592f), finalPoint);
                
                finalPoint += _SphereOffset;
                
                finalPoint = lerp(wPos, finalPoint, _CoordinateInterpolation);
                
                o.normal = normalize(mul(unity_ObjectToWorld, v.normal));
                o.vertex = mul(UNITY_MATRIX_VP,float4(finalPoint,1));
                o.color = v.color;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            half4 _Color;
            half _MinNL;
            half _MaxNL;
            
            fixed4 frag (v2f i) : SV_Target
            {
                
				fixed4 col = (i.color * _Color);
				half directionalLightInfluence = max(0,dot(i.normal, _WorldSpaceLightPos0.xyz));
				directionalLightInfluence = lerp(_MinNL, _MaxNL, directionalLightInfluence * directionalLightInfluence * directionalLightInfluence);
                col*=directionalLightInfluence;
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
