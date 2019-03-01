/*
	Descriptor: 画笔，笔刷Shader
	
	Edit: songguangze
*/
Shader "SeekGame/PainterBrush"
{
	Properties
	{
		_MainColor("Main Color",Color)=(1,1,1,1)
		_MainTex("Main Texture",2D) = "white"{}
		_FadeoutFactor("Fadeout Factor",Range(0,1)) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		Cull Off 
		ZWrite Off 
		ZTest Always
		Blend One One

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 uv:TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float4 _MainColor;
			fixed _FadeoutFactor;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv,_MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float4 brushColor = tex2D(_MainTex,i.uv);
				float4 finalColor = brushColor* _MainColor * _FadeoutFactor;
				finalColor.a *= brushColor.a;
				return finalColor;
			}
			ENDCG
		}
	}
}
