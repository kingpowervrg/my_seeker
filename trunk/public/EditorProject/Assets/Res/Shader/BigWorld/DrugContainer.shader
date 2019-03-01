Shader "Seeker/DrugContainer"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_EdgeColor("EdgeColor",Color) = (1,1,1,1)
		_EdgeFactor("EdgeFactor",float) = 3 //光晕长度

		_Clamp("Clamp",float) = 1 //其实是高度

		//_DissolveFactor("DissolveFactor",Range(0,1)) = 0
		_MaxHei("MaxHei",float) = 1  //最大高度
		_MaskColor("MaskColor",Color) = (1,1,1,1)

		_LightMapTex("LightMapTex",2D) = "white" {}
		_LightLerp("LightLerp",Range(0,1)) = 0
	}
	SubShader
	{
		Tags {"LightMode" = "ForwardBase" "RenderType"="Opaque"}
		LOD 100
		Cull off
		Pass
		{
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile LIGHTMAP_ON
			
			// uniform float4 _LightColor0;
			#include "UnityCG.cginc"
			struct appdata
			{
				float4 vertex : POSITION;
				fixed2 uv : TEXCOORD0;
				fixed2 uv1 : TEXCOORD1;
			};

			struct v2f
			{
				fixed2 uv : TEXCOORD0;
				#ifdef LIGHTMAP_ON
				fixed2 uv1 : TEXCOORD1;
				#endif
				float3 worldPos : TEXCOORD2;
				float4 vertex : SV_POSITION;

			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _LightMapTex;
			float4 _LightMapTex_ST;

			fixed _LightLerp;

			fixed4 _EdgeColor;
			float _EdgeFactor;

			float _Clamp;
			//fixed _DissolveFactor;
			float _MaxHei;
			fixed4 _MaskColor;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos.xyz = mul(unity_ObjectToWorld,v.vertex );
				#ifdef LIGHTMAP_ON
				o.uv1 = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float uvClamp = saturate((_Clamp - i.worldPos.y) / _MaxHei);
				uvClamp = saturate(uvClamp * _EdgeFactor);

				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 finalCol = lerp(col,_MaskColor,uvClamp);
				fixed section = step(0.1,uvClamp) * step(uvClamp,0.9);
				uvClamp = step(0.5,uvClamp) * (1 - uvClamp) + step(uvClamp,0.5) * uvClamp;

				finalCol = section * lerp(finalCol,_EdgeColor * uvClamp * 3,uvClamp ) + (1 - section) * finalCol ;

				#ifdef LIGHTMAP_ON
				fixed3 lm = DecodeLightmap (UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv1.xy));
				fixed3 nightCol = DecodeLightmap (tex2D(_LightMapTex, i.uv1.xy));
				fixed3 lmNight = lerp(lm,nightCol,_LightLerp);
				finalCol = finalCol * fixed4(lmNight,1);

				//return fixed4(lm,1);
				#endif
				return finalCol ;
			}
			ENDCG
		}
	}
}
