Shader "Hidden/Dark"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MaskTop("Texture", 2D) = "white"{}
		_MaskBottom("Texture", 2D) = "white"{}
		_Bite("_Bite",Float) = 0
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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _MaskTop;
			sampler2D _MaskBottom;
			float _Bite;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				// just invert the colors
				fixed2 uv = i.uv - fixed2(0.5, 0.5);
				float dist = length(uv);
				float color = smoothstep(0, 0.3, dist);
				float4 maskTop = float4(1., 1., 1., 1.) - tex2D(_MaskTop, fixed2(uv.x * 2., uv.y) + fixed2(0.5, 0.5 - _Bite)
				);
				float4 maskBottom = float4(1., 1., 1., 1.) - tex2D(_MaskBottom, fixed2(uv.x * 2., uv.y) + fixed2(0.5, 0.5 + _Bite)
				);
				return (maskBottom * maskTop ) * lerp(col, float4(0.,0.,0.,0.), color);
			}
			ENDCG
		}
	}
}
