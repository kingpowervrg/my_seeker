/*
	Descriptor: 画笔笔刷Shader (有透明混合)
*/
Shader "SeekGame/PainterBrushTransparency"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_MainColor("Main Color",Color) = (1,1,1,1)
		_FadeoutFactor("Fadeout Factor",Range(0,1)) = 1
	}
		SubShader
	{
		Tags{ "Queue" = "Transparent" }
		ZTest Always
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

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

	sampler2D _MainTex;
	float4 _MainTex_ST;

	float4 _MainColor;
	fixed _FadeoutFactor;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 col = tex2D(_MainTex, i.uv);

	fixed4 finalCol =lerp(float4(0,0,0,1), col * _MainColor,_FadeoutFactor);
	finalCol.a =pow( col.r,2);

	return finalCol;
	}
		ENDCG
	}
	}
}
