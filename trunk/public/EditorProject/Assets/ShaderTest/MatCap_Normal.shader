// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Seeker/MatCap_Normal" {
	Properties {
		//_Color ("Main Color", Color) = (0.5,0.5,0.5,1)
		_MainTex("MainTex",2D) = "white" {}

		_NormalTex("NormalTex",2D) = "white"{}

		//_MatCapDiffuseTex("MatCapDiffuse",2D) = "white" {} //普通光照图
		//_MatCapFactor("MatCapFactor",float) = 1
		_MatCapSpecularTex ("MatCap (RGB)", 2D) = "white" {} //高光图
		_SpecularFactor("SpecularFactor",Range(0,2)) = 1
		_MaskTex("MaskTex",2D) = "white" {}  //遮罩图

		//_AOTex("AOTex",2D) = "white" {}  //AO遮罩图
		_BumpValue("_BumpValue",float) = 1
		//_fresnelPow("fresnelPow",float) = 1
		//_fresnelColor("fresnelColor",COLOR) = (1,1,1,1)
		
	}
	
	Subshader {
		Tags { "RenderType"="Opaque" }
		
		Pass {
		//Tags { "LightMode" = "Always" }
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile LIGHTMAP_ON
				#include "UnityCG.cginc"
				
			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
				float4 tangent : TANGENT;
				float2 texcoord1 : TEXCOORD1;
			};

				struct v2f { 
					float4 pos : SV_POSITION;
					float4	uv : TEXCOORD0;
					float3	TtoV0 : TEXCOORD1;
					float3	TtoV1 : TEXCOORD2;
					#ifdef LIGHTMAP_ON
					half2 uvLM : TEXCOORD3;
					#endif
					//fixed3 worldNormal : TEXCOORD4;
					//fixed3 worldPos: TEXCOOR5;
				};
				
				sampler2D _MainTex;
				float4 _MainTex_ST;
				sampler2D _NormalTex;
				float4 _NormalTex_ST;

				sampler2D _AOTex;
				v2f vert (appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos (v.vertex);
					o.uv.xy = TRANSFORM_TEX(v.texcoord,_MainTex);
					o.uv.zw = TRANSFORM_TEX(v.texcoord,_NormalTex);

					#ifdef LIGHTMAP_ON
					o.uvLM = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
					#endif

					//切线空间转视角空间
					TANGENT_SPACE_ROTATION;
					o.TtoV0 = mul(rotation, UNITY_MATRIX_IT_MV[0].xyz);
					o.TtoV1 = mul(rotation, UNITY_MATRIX_IT_MV[1].xyz);
					
					//o.uv.z = dot(normalize(UNITY_MATRIX_IT_MV[0].xyz), normalize(v.normal));
					//o.uv.w = dot(normalize(UNITY_MATRIX_IT_MV[1].xyz), normalize(v.normal));

					//o.worldNormal = UnityObjectToWorldNormal(v.normal);
					//o.worldPos = mul(unity_ObjectToWorld, v.vertex);
					
					return o;
				}
				
				//fixed4 _Color;

				sampler2D _MatCapSpecularTex;
				float _SpecularFactor;

				//sampler2D _MatCapDiffuseTex;
				//float _MatCapFactor;

				sampler2D _MaskTex;

				float _BumpValue;
				//fixed _fresnelPow;
				//fixed4 _fresnelColor;
				float4 frag (v2f i) : COLOR
				{
					fixed3 normalColor = UnpackNormal(tex2D(_NormalTex,i.uv.zw));

					float4 mainColor = tex2D(_MainTex, i.uv.xy);//UnpackNormal();
					fixed4 maskColor = tex2D(_MaskTex, i.uv.xy);
					
					//fixed4 aoColor = tex2D(_AOTex,i.uv.xy);
					normalColor.xy *= _BumpValue;
                    normalColor.z = sqrt(1.0- saturate(dot(normalColor.xy ,normalColor.xy)));
                    normalColor = normalize(normalColor);

					fixed2 normalUV;
					normalUV.x = dot(i.TtoV0,normalColor);
					normalUV.y = dot(i.TtoV1,normalColor);
					normalUV = normalUV*0.5 + 0.5;

					fixed4 SpecColor = tex2D(_MatCapSpecularTex, normalUV) * _SpecularFactor;
					//fixed4 diffuseColor = tex2D(_MatCapDiffuseTex, normalUV) * _MatCapFactor;
					#ifdef LIGHTMAP_ON
					fixed3 dayCol = DecodeLightmap (UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLM.xy));
					mainColor *=fixed4(dayCol,1);
					//return fixed4(1,0,0,1);
					#endif
					//添加菲涅尔
					//float3 worldView = UnityWorldSpaceViewDir(i.worldPos);
					//float fresenl = pow(1 - max(0,dot(normalize(worldView),normalize(i.worldNormal))),_fresnelPow);
					return  mainColor + SpecColor * maskColor.r;//lerp(SpecColor * maskColor.r,_fresnelColor,saturate(fresenl));
				}
			ENDCG
		}
	}
}