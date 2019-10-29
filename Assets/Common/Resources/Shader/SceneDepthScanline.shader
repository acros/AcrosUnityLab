Shader "Acros/SceneDepthScanline"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Cull Off ZWrite Off ZTest Always

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
			sampler2D _CameraDepthNormalsTexture;
			sampler2D _scanTex;
			float4 _MainTex_ST;

			float _lineDepthMin;
			float _lineDepthMax;

			float2 _scale;

			float4x4 _CamToWorld;
			half4 _lineColor;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				half3 normal;
				float depth;

				DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, i.uv), depth, normal);

				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				float depthWeight = ceil(depth - _lineDepthMin);
				float depthMaxWeight = 1 - ceil(depth - _lineDepthMax);

				//if (depth >= _lineDepthMin && depth <= _lineDepthMax)
				{
					float2 newUV = i.uv * _scale;
					half4 lineColor = depthWeight * depthMaxWeight * depthWeight * tex2D(_scanTex, newUV) * _lineColor;
					col += lineColor;
				}

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
