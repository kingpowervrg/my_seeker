Shader "Seeker/DayNightDiffuse"
{
	Properties
	{
		_Color("Color",Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_NightTex("NightTex",2D) = "white" {}
		_lerp("lerp",Range(0,1)) = 0
		_isGray("isGray",float) = 0
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
			#pragma multi_compile LIGHTMAP_ON
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				#ifdef LIGHTMAP_ON
				half2 uvLM : TEXCOORD4;
				#endif
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _NightTex;
			float4 _NightTex_ST;

			fixed _lerp;
			fixed4 _Color;
			half _isGray;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				#ifdef LIGHTMAP_ON
				o.uvLM = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				#ifdef LIGHTMAP_ON
				fixed3 dayCol = DecodeLightmap (UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLM.xy));
				fixed3 nightCol = DecodeLightmap (tex2D(_NightTex, i.uvLM.xy));
				fixed3 lm = lerp(dayCol,nightCol,_lerp);
				col.rgb*=lm;
				if(_isGray > 0)
				{
					col.rgb = col.r * 0.299 + col.g * 0.587 + col.b * 0.114;
				}
				//return fixed4(1,0,0,1);
				#endif
				// sample the texture
				return col * _Color;
			}
			ENDCG
		}
	}
}
