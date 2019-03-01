Shader "Seeker/DimianUnLock"
{
	Properties
	{
		_Color("Color",COLOR) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		//_NoiseTex("NoiseTex",2D) = "white" {}
		_EdgeColor("EdgeColor",COLOR) = (1,1,1,1)
		_maxDis("MaxDis",float) = 1
		_Factor("Factor",Range(0,1)) = 0
		_centerWorldPos("centerWorldPos",Vector) = (0,0,0,0)
		_LightMapTex("LightMapTex",2D) = "white" {}
		_LightLerp("LightLerp",Range(0,1)) = 0
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
				#ifdef LIGHTMAP_ON
				fixed2 uv1 : TEXCOORD1;
				#endif
				//fixed2 noiseUV : TEXCOORD2;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				fixed2 uv1 : TEXCOORD1;
				//fixed2 noiseUV : TEXCOORD2;
				float3 worldVertex:TEXCOORD3;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _LightMapTex;
			float4 _LightMapTex_ST;

			fixed _LightLerp;

			//sampler2D _NoiseTex;
			//float4 _NoiseTex_ST;

			float4 _centerWorldPos;
			fixed4 _EdgeColor;
			fixed4 _Color;
			float _maxDis;
			fixed _Factor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				#ifdef LIGHTMAP_ON
				o.uv1 = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				//o.noiseUV = TRANSFORM_TEX(v.noiseUV,_NoiseTex);
				o.worldVertex.xyz = mul(unity_ObjectToWorld,v.vertex);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float dis = distance(i.worldVertex.xyz,_centerWorldPos.xyz);
				fixed disClamp = dis / _maxDis;
				
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed lerpFactor = saturate((_Factor - disClamp) * 20);
				fixed4 finalCol = lerp(_EdgeColor,col,lerpFactor); //_Color * (1 - step(0.1,lerpFactor));
				finalCol = step(0.1,lerpFactor) * finalCol + lerp(_Color,finalCol,lerpFactor * 10) * (1 - step(0.1,lerpFactor));
				//finalCol = lerp(_EdgeColor,finalCol,lerpFactor);
				#ifdef LIGHTMAP_ON
				fixed3 lm = DecodeLightmap (UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv1.xy));
				fixed3 nightCol = DecodeLightmap (tex2D(_LightMapTex, i.uv1.xy));
				fixed3 lmNight = lerp(lm,nightCol,_LightLerp);
				finalCol = finalCol * fixed4(lmNight,1);
				//return fixed4(lm,1);
				#endif
				
				//clip(_Factor - disClamp);
				return finalCol;
			}
			ENDCG
		}
	}
}
