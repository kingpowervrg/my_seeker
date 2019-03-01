// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Seeker/Exhibit/ExhibitStreamer"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_LightTex("LightTex",2D) = "white" {}
		_StreamerTex("_StreamerTex",2D) = "white" {}
		_speed("_speed",float) = 1
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
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
				float2 uv1 : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _LightTex;
			float4 _LightTex_ST;

			sampler2D _StreamerTex;
			float4 _StreamerTex_ST;
			fixed _speed;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv1 = TRANSFORM_TEX(v.uv1, _LightTex);
				o.uv2 = TRANSFORM_TEX(v.uv2, _StreamerTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed3 colLight = DecodeLightmap(tex2D(_LightTex, i.uv1));
				fixed4 col = tex2D(_MainTex, i.uv);
				i.uv2 += fixed2(_speed*_Time.y,0);
				fixed4 StreamerCol = tex2D(_StreamerTex, i.uv2);
				return fixed4(col.rgb * colLight + StreamerCol.rgb,col.a);
			}
			ENDCG
		}
	}
}
