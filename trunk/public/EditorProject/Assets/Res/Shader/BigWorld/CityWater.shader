Shader "Seeker/CityWater"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		//_NoiseTex("NoiseTex",2D) = "white" {}
		_MaskTex("MaskTex",2D) = "white"{}
		_NormalTex("NormalTex",2D) = "white"{}
		_BumpPower("BumpPower",Range(-1,2)) = .6//法线强度
		_EdgeTex("EdgeTex",2D) = "white"{}
		/*_NoiseSpeedX("NoiseSpeedX",float) = 1
		_NoiseSpeedY("NoiseSpeedY",float) = 1
		_NoiseWave("NoiseWave",float) = 1*/
		_WaveSize("WaveSize",Range(0.01,1)) = .25//波纹大小
		_WaveOffset("WaveOffset(xy&zw)",vector) = (.1,.2,-.2,-.1)//波纹流动方向
		_FarColor("FarColor",Color) = (.2,1,1,.3)//反射颜色
		_NearColor("NearColor",Color) = (1,1,1,1)
		_LightColor("LightColor",Color) = (1,1,1,1)//光源颜色
		_LightVector("LightVector(xyz for lightDir,w for power)",vector) = (.5,.5,.5,100)//光源方向
		_LightPower("LightPower",Range(0,1)) = 1
		_EdgeColor("EdgeColor",Color) = (1,1,1,1)
		_WaveSpeed("WaveSpeed",Range(0,1)) = 1
		_WavePower("WavePower",Range(0,1)) = 1
		
		_WaveTex("海浪周期贴图",2D) = "white" {}//海浪周期贴图
		_NoiseTex("海浪躁波贴图", 2D) = "white" {} //海浪躁波
		_NoiseRange("海浪躁波强度", Range(0,10)) = 1//海浪躁波强度
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
				half3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
#ifdef LIGHTMAP_ON
				half2 uvLM : TEXCOORD1;
#endif
				half3 viewDir:TEXCOORD2;
				half3 normal:TEXCOORD3;
				float4 waveUV : TEXCOORD4;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _MaskTex;
			sampler2D _NormalTex;
			sampler2D _EdgeTex;

			float4 _LightVector;
			fixed4 _FarColor;
			fixed4 _LightColor;

			half _WaveSize;
			half4 _WaveOffset;

			half _BumpPower;
			fixed _LightPower;
			fixed4 _EdgeColor;
			fixed _WaveSpeed;
			fixed _WavePower;

			sampler2D _NoiseTex;
			sampler2D _WaveTex;
			half _NoiseRange;

			fixed4 _NearColor;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.viewDir = WorldSpaceViewDir(v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);

				float4 wPos = mul(unity_ObjectToWorld, v.vertex);
				o.waveUV.xy = wPos.xz * _WaveSize + _WaveOffset.xy * _Time.y;
				o.waveUV.zw = wPos.xz * _WaveSize + _WaveOffset.zw * _Time.y;
#ifdef LIGHTMAP_ON
				o.uvLM = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#endif
				return o;
			}
			
			half2 fract(half2 val)
			{
				return val - floor(val);
			}
			fixed4 frag (v2f i) : SV_Target
			{

				fixed4 maskCol = tex2D(_MaskTex, i.uv);
				fixed depth = maskCol.g;

				fixed4 col = tex2D(_MainTex, i.uv);
#ifdef LIGHTMAP_ON
				fixed3 dayCol = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLM.xy));
				col.rgb *= dayCol;
#endif
				
				fixed mask = maskCol.r;
				half3 nor = UnpackNormal((tex2D(_NormalTex, fract(i.waveUV.xy)) + tex2D(_NormalTex, fract(i.waveUV.zw * 1.2)))*0.5);
				nor = normalize(i.normal + nor.xyz *half3(1, 1, 0) * _BumpPower);
				
				half spec = max(0, dot(nor, normalize(normalize(_LightVector.xyz) + normalize(i.viewDir))));
				spec = pow(spec, _LightVector.w) * _LightPower;

				half fresnel = 1 - saturate(dot(nor, normalize(i.viewDir)));
				col = lerp(col, _FarColor, fresnel * mask);
				col = lerp(col,_NearColor, (1-depth) * mask);
				//海浪
				
				fixed noise = tex2D(_NoiseTex, i.uv).r;
				fixed wave = tex2D(_WaveTex, fract(half2(_Time.y*_WaveSpeed + depth + noise * _NoiseRange, 0.5))).r;
				fixed edge = saturate((tex2D(_EdgeTex, i.waveUV.xy * 5).r + tex2D(_EdgeTex, i.waveUV.zw * 2).r)*0.5) * wave;

				
				//fixed wave = tex2D(_EdgeTex, half2(frac(_Time.y*_WaveSpeed + depth), 0.5)).r;
				//fixed edge = saturate((tex2D(_EdgeTex, i.waveUV.xy * 5).r + tex2D(_EdgeTex, i.waveUV.zw * 2).r)*0.5);
				col.rgb += _EdgeColor * edge *(1 - depth) * _WavePower * mask;
				col.rgb += _LightColor * spec * mask;

				return col;
			}
			ENDCG
		}
	}
}
