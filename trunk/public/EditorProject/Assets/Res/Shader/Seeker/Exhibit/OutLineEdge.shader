Shader "Seeker/Exhibit/OutLineEdge"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_LightTex("LightTex",2D) = "white" {}
		_OutlineColor ("Outline color", Color) = (0,1,1,1)
		_OutlineWidth ("Outlines width", Range (0.0, 2.0)) = 0.02
		_Factor("Factor",float) = 2
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	float _OutlineWidth;
	float4 _OutlineColor;
	fixed _Factor;
	ENDCG

	SubShader
	{
		pass{

			Stencil {  
                Ref 254 
                Comp Always
                Pass Replace
                ZFail Keep
            }
			Tags{ "Queue" = "Transparent"}
			//ZTest LEqual
			Blend SrcAlpha OneMinusSrcAlpha 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float3 normal : NORMAL;
				
			};
			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float4 pos : SV_POSITION;
				fixed3 worldPos : TEXCOORD2;
				fixed3 worldNormal : TEXCOORD3;
			};

			sampler2D _LightTex;
			float4 _LightTex_ST;

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			v2f vert(appdata v)
			{
				
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv1 = TRANSFORM_TEX(v.uv1, _LightTex);
				o.worldPos = mul(unity_ObjectToWorld,v.vertex);
				o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);
				return o;

			}

			half4 frag(v2f i) : COLOR
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed3 colLight = DecodeLightmap(tex2D(_LightTex, i.uv1));

				fixed3 worldView = _WorldSpaceCameraPos.xyz - i.worldPos;
				float rim = 1 - max(0,dot(normalize(i.worldNormal),normalize(worldView)));
				fixed factor = pow(rim,1 / _Factor);
				//col = factor * _OutlineColor;
				col = lerp(col,factor * _OutlineColor,saturate(factor));
				//col.a = 0;
				return col * fixed4(colLight,1) * _Color;
			}
			ENDCG
		}

		Pass //Outline
		{
			Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" }
			Stencil {  
                Ref 254         
                Comp NotEqual             
                Pass Keep 
                ZFail Keep
            }
			
			//ZTest Greater
			ZWrite Off
			Cull Back
			Blend SrcAlpha One 
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				
			};

			struct v2f
			{
				float4 pos : POSITION;
				fixed3 worldPos : TEXCOORD1;
				fixed3 worldNormal : TEXCOORD2;
			};

			v2f vert(appdata v)
			{
				appdata original = v;
				v.vertex.xyz += _OutlineWidth * normalize(v.vertex.xyz);

				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				//o.worldPos = mul(unity_ObjectToWorld,v.vertex);
				//o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);
				return o;

			}

			half4 frag(v2f i) : COLOR
			{
				//fixed3 worldView = _WorldSpaceCameraPos.xyz - i.worldPos;
				//float rim = 1 - max(0,dot(normalize(i.worldNormal),normalize(worldView)));
				//fixed factor = pow(rim,1 / _Factor);
				//fixed4 col = factor * _OutlineColor;
				return _OutlineColor;
			}

			ENDCG
		}

		//Pass
		//{
		//	ZTest LEqual
		//	Stencil {  
  //              Ref 254         
  //              Comp NotEqual             
  //              Pass Keep 
  //              ZFail Keep
  //          }
		//	CGPROGRAM
		//	#pragma vertex vert
		//	#pragma fragment frag
			
		//	#include "UnityCG.cginc"

		//	struct appdata
		//	{
		//		float4 vertex : POSITION;
		//		float2 uv : TEXCOORD0;
		//		float2 uv1 : TEXCOORD1;
		//	};

		//	struct v2f
		//	{
		//		float2 uv : TEXCOORD0;
		//		float2 uv1 : TEXCOORD1;
		//		float4 vertex : SV_POSITION;
		//	};

		//	//sampler2D _MainTex;
		//	//float4 _MainTex_ST;

		//	//sampler2D _LightTex;
		//	//float4 _LightTex_ST;

		//	//fixed4 _Color;
			
		//	v2f vert (appdata v)
		//	{
		//		v2f o;
		//		o.vertex = UnityObjectToClipPos(v.vertex);
		//		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		//		return o;
		//	}
			
		//	fixed4 frag (v2f i) : SV_Target
		//	{
		//		//fixed3 colLight = DecodeLightmap(tex2D(_LightTex, i.uv1));
		//		fixed4 col = tex2D(_MainTex, i.uv);
		//		return col; 
		//	}
		//	ENDCG
		//}

		
		

		
	}
	Fallback "Diffuse"
}