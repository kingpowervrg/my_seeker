// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Seeker/Exhibit/Cutoff"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_LightTex("LightTex",2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}
	SubShader
	{
		Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout" }
		LOD 100
		ColorMask RGB
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
				float2 uv1 : TEXCOORD1;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _LightTex;
			float4 _LightTex_ST;

			fixed4 _Color;
			fixed _Cutoff;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv1 = TRANSFORM_TEX(v.uv1, _LightTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed3 colLight = DecodeLightmap(tex2D(_LightTex, i.uv1));
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 finalColor = col * fixed4(colLight,1) * _Color;
				clip(finalColor.a - _Cutoff);
				return finalColor; 
			}
			ENDCG
		}
	}
}
