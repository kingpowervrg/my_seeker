// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/SpiritPressure"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_LineTex("_LineTex", 2D) = "white" {}

		_Flags("_Flags", 2D) = "white" {}
		_ShakeFactor("xy(实体的振幅和频率),zw(虚影的振幅和频率)",Vector)=(0.01,20,0.0125,20)

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
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			sampler2D _LineTex;
			float4 _LineTex_ST;
			int _RowCount;
			int _ColumCount;
			float _Speed;
			sampler2D _Flags;
			float4 _ShakeFactor;
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
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv.xy = v.uv;

#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
				{
					o.uv.y = 1 - o.uv.y;
				}
#endif
				return o;
			}
			

			fixed4 frag (v2f i) : SV_Target
			{
				float x1 = _ShakeFactor.x*sin(_Time.y * _ShakeFactor.y)+1;
				float x2 = _ShakeFactor.z*sin(_Time.y * _ShakeFactor.w)+1;

				float3 mainRGB = tex2D(_MainTex, i.uv *x1).rgb;
				float3 shadowRGB = tex2D(_MainTex, i.uv *x2).rgb;


				float2 perCent = float2(1.0, 1.0) / float2(_ColumCount, _RowCount);
				int index = fmod(_Time.y*_Speed, _RowCount*_ColumCount);

				float indexY = floor(index / _RowCount);
				float indexX = index - _ColumCount * indexY;
				float2 uv = (i.uv + float2(indexX, indexY)) * perCent;


				float4 lineTex = tex2D(_LineTex, TRANSFORM_TEX( uv, _LineTex));
		

				
				float flag = tex2D(_Flags,i.uv).a;

				//flag = 1;
				float3 emissive = (mainRGB*0.7 + shadowRGB*0.3);
				float3 finalColor =lerp(emissive, lineTex.rgb, lineTex.a);
				//finalColor = emissive*(1 - flag) + lerp(emissive*flag, lineTex.rgb*flag, lineTex.a);
				//finalColor = emissive.rgb;
				return fixed4(finalColor,1);
			}
			ENDCG
		}
	}
}
