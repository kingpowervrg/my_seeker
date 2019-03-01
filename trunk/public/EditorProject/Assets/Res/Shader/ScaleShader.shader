Shader "Custom/ScaleShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_ShadowScale("Shadow Scale", Range(1, 5)) = 1
	}
		SubShader
		{
			Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

			Cull Off
			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha			//UI Sprite 多数使用的混合方式

			Pass
		{
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#pragma multi_compile _ ENABLESHADOW_ON



#include "UnityCG.cginc"

			struct Input
		{
			float4 vertex : POSITION;
		};

		struct v2f
		{
			float2 uv : TEXCOORD0;
			float4 vertex : SV_POSITION;
		};

		sampler2D _MainTex;
		float4 _MainTex_ST;
		float4 _MainTex_TexelSize;

		float _SilhouetteWidth;
		float4 _SilhouetteColor;			//传入超过1的值，用于HDR获取辉光
		float _SilhouetteIntensity;			//强度

		float _AlphaThreshold;

		float4 _Color;

		sampler2D _AlphaTex;
		float _EnableExternalAlpha;

		float _ShadowScale;

	
		
		float4x4 Scale(float scale)
		{
			return float4x4 (scale,0,0,0,
								0, scale,0,0,
								0,0, scale,0,
								0,0,0,1 );
		}


		v2f vert(Input i)
		{
			v2f o;
			i.vertex = mul(Scale(_ShadowScale), i.vertex);
			o.vertex = UnityObjectToClipPos(i.vertex);
			//o.uv = TRANSFORM_TEX(v.uv, _MainTex);

			return o;
		}


		float4 frag(v2f i) : SV_Target
		{
			return fixed4(0,1,0,1);
		float4 color = tex2D(_MainTex,i.uv);

#if ETC1_EXTERNAL_ALPHA
		float4 alpha = tex2D(_AlphaTex,i.uv);
		color.a = lerp(color.a,alpha.r,_EnableExternalAlpha);
#endif


#ifdef ENABLESILHOUETTE_ON
		float silhoetteFactor = isDrawInsideSilhoette(i.uv,_SilhouetteWidth,_AlphaThreshold);
		color.rgb = lerp(color.rgb,_SilhouetteColor.rgb * _SilhouetteIntensity,silhoetteFactor);
#endif

		color.rgb *= color.a;

		return color;

		}
			ENDCG
		}
		}
	FallBack "Diffuse"
}
