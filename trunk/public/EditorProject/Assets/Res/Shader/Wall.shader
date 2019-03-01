Shader "Seeker/Wall"
{
	Properties
	{
		_Color("Color",Color) = (1,1,1,1)
		_MainTex("MainTex",2D) = "white" {}
		_GlowTex ("GlowTex", 2D) = "white" {}
		
		//_Noise("Noise",2D) = "white"{}
		_SpeedX("SpeedX",float) = 0
		_SpeedY("SpeedY",float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		LOD 100
		Cull off
		ZWrite off
		//Blend SrcAlpha OneMinusSrcAlpha
		Blend One One
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
				float2 uv1 : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;


			sampler2D _GlowTex;
			float4 _GlowTex_ST;
			fixed4 _Color;
			float _SpeedX;
			float _SpeedY;

			//sampler2D _Noise;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _GlowTex);
				o.uv1 = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 mainCol = tex2D(_MainTex,i.uv1 +  frac(_Time.y * float2(_SpeedX,_SpeedY)));
				//fixed4 noiseCol = tex2D(_Noise,i.uv + _Time.y * float2(_SpeedX,_SpeedY));
				// sample the texture
				fixed4 col = tex2D(_GlowTex,i.uv);
				return col * _Color * mainCol;
			}
			ENDCG
		}
	}
}
