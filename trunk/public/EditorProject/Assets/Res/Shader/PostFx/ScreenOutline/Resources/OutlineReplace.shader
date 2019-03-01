// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOE/Outline/OutlineReplace" 
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,0,1)
	}

	SubShader
	{
		Tags{ "Queue" = "Geometry" "IgnoreProjector" = "True"}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			uniform float4 _Color;

			struct appdata_t 
			{
				float4 vertex : POSITION;
			};
			struct v2f 
			{
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				return _Color;
			}
			ENDCG
		}
	}
}