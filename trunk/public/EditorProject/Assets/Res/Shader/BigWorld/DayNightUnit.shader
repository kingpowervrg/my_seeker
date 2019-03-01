// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Seeker/DayNightUnit" {
Properties {
    _Color ("Tint Color", Color) = (1,1,1,1)
	_NextColor("NextColor",Color) = (0.2,0.2,0.2,1)
    _MainTex ("Particle Texture", 2D) = "white" {}
	_lerp("lerp",Range(0,1)) = 0
}

Category {
    Tags { "RenderType"="Opaque" }

    SubShader {
        Pass {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _Color;
			fixed4 _NextColor;
			fixed _lerp;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            float4 _MainTex_ST;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                return o;
            }


            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 finalCol = lerp(_Color,_NextColor,_lerp);
				fixed4 texCol = tex2D(_MainTex,i.texcoord);
                return finalCol * texCol;
            }
            ENDCG
        }
    }
}
}
