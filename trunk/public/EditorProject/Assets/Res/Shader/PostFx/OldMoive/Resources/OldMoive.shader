// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/OldMoive"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SnowTex("SnowTex",2D) = "white"{}
		_SnowValueX("SnowValueX",float) = 1
		_SnowValueY("SnowValueY",float) = 1

		_SnowPower("SnowPower",float) = 1
		_Saturation("Saturation",float) = 1
		_RandomBrightValue("RandomBrightValue",float) = 1
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

			uniform sampler2D _MainTex;
			float4 _MainTex_ST;

			uniform sampler2D _SnowTex;

			fixed _RandomValue;
			fixed _SnowValueX;
			fixed _SnowValueY;

			fixed _SnowPower;
			fixed _Saturation;
			fixed _RandomBrightValue;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 srcCol = tex2D(_MainTex,i.uv);

				fixed4 bgCol = srcCol;// * (1 - isDong) + bgCol1 * isDong;

				//fixed4 maskCol = tex2D(_MaskTex,i.uv);
				half snowUVX = i.uv.x + _RandomValue * _SinTime.z * _SnowValueX;
				half snowUVY = i.uv.y + _RandomValue * _SinTime.z * _SnowValueY;
				fixed4 snowCol = tex2D(_SnowTex,half2(snowUVX,snowUVY));

//				fixed lum = dot(fixed3(0.299,0.587,0.114),bgCol.rgb);
//				fixed4 finalColor = lum + lerp(_Color, _Color + fixed4(0.1f, 0.1f, 0.1f, 0.1f), _RandomValue);

//				finalColor = lerp(finalColor,finalColor * maskCol,_VignetteAmount);
				fixed4 finalColor = lerp(bgCol,bgCol * snowCol, _SnowPower);
				//finalColor *= _RandomBrightValue;

				//finalColor = lerp(finalColor,finalColor,_EffectAmount);
				fixed lum = Luminance(finalColor.rgb);
				finalColor.rgb = lerp(fixed3(lum,lum,lum), finalColor.rgb, _RandomBrightValue);
				return finalColor;
			}
			ENDCG
		}
	}
}
