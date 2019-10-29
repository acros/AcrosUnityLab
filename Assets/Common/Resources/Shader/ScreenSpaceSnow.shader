// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//http://blog.theknightsofunity.com/make-it-snow-fast-screen-space-snow-shader/
Shader "TKoU/ScreenSpaceSnow"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _CameraDepthNormalsTexture;
			sampler2D _MainTex;
			float4x4 _CamToWorld;

			sampler2D _SnowTex;
			float _SnowTexScale;

			half4 _SnowColor;

			fixed _BottomThreshold;
			fixed _TopThreshold;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			/*
			// Reconstruct view-space position from UV and depth.
			// p11_22 = (unity_CameraProjection._11, unity_CameraProjection._22)
			// p13_31 = (unity_CameraProjection._13, unity_CameraProjection._23)
			float3 ReconstructViewPos(float2 uv, float depth, float2 p11_22, float2 p13_31)
			{
				return float3((uv * 2.0 - 1.0 - p13_31) / p11_22 * CheckPerspective(depth), depth);
			}
			*/

			fixed4 frag(v2f i) : SV_Target
			{
				half3 normal;
				float depth;

				DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, i.uv), depth, normal);
				normal = mul((float3x3)_CamToWorld, normal);

				// find out snow amount
				half snowAmount = normal.g;
				half scale = (_BottomThreshold + 1 - _TopThreshold) / 1 + 1;
				snowAmount = saturate((snowAmount - _BottomThreshold) * scale);

				//////////////////////////////////////////////////////////////
				//[Key] - Find a mapping method for snow texture
				// find out snow color
				float2 p11_22 = float2(unity_CameraProjection._11, unity_CameraProjection._22);
				float3 vpos = float3((i.uv * 2 - 1) / p11_22, -1) * depth;	//Viewport pos of the pixel
				float4 wpos = mul(_CamToWorld, float4(vpos, 1));			//World pos
				
				wpos += (float4(_WorldSpaceCameraPos, 0) / _ProjectionParams.z);
				wpos *= (_SnowTexScale * _ProjectionParams.z);		//  _ProjectionParams.z - Camera's far clip 

				//Wpos.wz - Horizental Plane
				half4 snowColor = tex2D(_SnowTex, wpos.xz) * _SnowColor;
				//////////////////////////////////////////////////////////////

				// get color and lerp to snow texture
				half4 col = tex2D(_MainTex, i.uv);
				return lerp(col, snowColor, snowAmount);
			}
			ENDCG
		}
	}
}