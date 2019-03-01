Shader "GOE/BlurIconShaderUGUI"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	_IsBlur("Blur", Range(0,1)) = 0
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
#pragma target 3.0


#include "UnityCG.cginc"


		struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
	};

	fixed _IsBlur;

	sampler2D _MainTex;
	uniform fixed4 _MainTex_TexelSize;



	v2f vert(appdata_base  v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;

		return o;
	}


	fixed4 frag(v2f i) : SV_Target
	{
		fixed enableBlur = step(0.5,_IsBlur);

	if (1 == enableBlur)
	{
		float2 dsdx = ddx(i.uv.x) * 10;
		float2 dsdy = ddy(i.uv.y) * 10;

		fixed4 col = tex2D(_MainTex, i.uv, dsdx, dsdy);

		return col;
	}
	else
	{
		fixed4 col = tex2D(_MainTex, i.uv);
		return col;
	}

	}

		ENDCG
	}
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

	fixed _IsBlur;

	fixed4 Blur(fixed4 blured_color, float2 uv, float delta_xy);

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		return o;
	}

	sampler2D _MainTex;
	uniform fixed4 _MainTex_TexelSize;
	fixed4 frag(v2f i) : SV_Target
	{

		fixed4 col = tex2D(_MainTex, i.uv);
	fixed enableBlur = step(0.5,_IsBlur);

	if (1 == enableBlur)
	{
		fixed4 col_blur = col;
		//float2 delta_xy = (_MainTex_TexelSize.x * 10, _MainTex_TexelSize.y * 10);
		float delta_xy = 0.015;//(0.01, 0.01);

		col_blur = Blur(col_blur, i.uv, delta_xy);
		//col_blur = Blur(col_blur, i.uv, delta_xy);
		//col_blur = Blur(col_blur, i.uv, delta_xy);
		//col_blur = Blur(col_blur, i.uv, delta_xy);

		return  col_blur;
	}
	else
	{
		return col;
	}

	}

		fixed4 Blur(fixed4 blured_color, float2 uv, float delta_xy)
	{
		//上
		float2 delta_uv = (0, delta_xy);

		blured_color += tex2D(_MainTex, uv + delta_uv);

		//下
		delta_uv = (0, -delta_xy);

		blured_color += tex2D(_MainTex, uv + delta_uv);

		//左
		delta_uv = (-delta_xy, 0);

		blured_color += tex2D(_MainTex, uv + delta_uv);

		//右
		delta_uv = (delta_xy, 0);

		blured_color += tex2D(_MainTex, uv + delta_uv);

		//左上
		delta_uv = (-delta_xy, delta_xy);

		blured_color += tex2D(_MainTex, uv + delta_uv);

		//右上
		delta_uv = (delta_xy, delta_xy);

		blured_color += tex2D(_MainTex, uv + delta_uv);

		//左下
		delta_uv = (-delta_xy, -delta_xy);

		blured_color += tex2D(_MainTex, uv + delta_uv);

		//右下
		delta_uv = (delta_xy, -delta_xy);

		blured_color += tex2D(_MainTex, uv + delta_uv);

		blured_color /= 9;

		return blured_color;
	}
	ENDCG
	}
	}

		FallBack "Diffuse"
}
