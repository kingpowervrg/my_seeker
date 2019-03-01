Shader "GOE/BlurIconShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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
				float4 vertex : SV_POSITION;
			};

			fixed4 Blur(fixed4 blured_color, float2 uv, float2 delta_xy);

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			uniform fixed4 _MainTex_TexelSize;
			fixed4 frag(v2f i) : SV_Target
			{
				return fixed4(1,0,0,1);

				//	fixed4 col = tex2D(_MainTex, i.uv);

				//float2 delta_xy = (_MainTex_TexelSize.x, _MainTex_TexelSize.y);

				////上
				//float2 delta_uv = (0, delta_xy.y);

				//fixed4 col_blur = col + tex2D(_MainTex, i.uv + delta_uv);

				////下
				//delta_uv = (0, -delta_xy.y);

				//col_blur = col_blur + tex2D(_MainTex, i.uv + delta_uv);

				////左
				//delta_uv = (-delta_xy.x,0);

				//col_blur = col_blur + tex2D(_MainTex, i.uv + delta_uv);

				////右
				//delta_uv = (delta_xy.x, 0);

				//col_blur = col_blur + tex2D(_MainTex, i.uv + delta_uv);

				////左上
				//delta_uv = (-delta_xy.x, delta_xy.y);

				//col_blur = col_blur + tex2D(_MainTex, i.uv + delta_uv);

				////右上
				//delta_uv = (delta_xy.x, delta_xy.y);

				//col_blur = col_blur + tex2D(_MainTex, i.uv + delta_uv);

				////左下
				//delta_uv = (-delta_xy.x, -delta_xy.y);

				//col_blur = col_blur + tex2D(_MainTex, i.uv + delta_uv);

				////右下
				//delta_uv = (delta_xy.x, -delta_xy.y);

				//col_blur = col_blur + tex2D(_MainTex, i.uv + delta_uv);

				//col_blur /= 9;
				//// just invert the colors
				////col.rgb = 1 - col.rgb;
				fixed4 col = tex2D(_MainTex, i.uv);
			fixed4 col_blur = col;
			//float2 delta_xy = (_MainTex_TexelSize.x * 10, _MainTex_TexelSize.y * 10);
			float2 delta_xy = (0.01, 0.01);

			col_blur = Blur(col_blur, i.uv, delta_xy);

			col_blur = Blur(col_blur, i.uv, delta_xy);
			col_blur = Blur(col_blur, i.uv, delta_xy);
			col_blur = Blur(col_blur, i.uv, delta_xy);
			//col_blur += Blur(i);
			//col_blur += Blur(i);
			//col_blur += Blur(i);

			return  col_blur;
			}

			fixed4 Blur(fixed4 blured_color, float2 uv, float2 delta_xy)
			{
				//上
				float2 delta_uv = (0, delta_xy);

				blured_color += tex2D(_MainTex, uv + delta_uv);

				//下
				delta_uv = (0, -delta_xy);

				blured_color += tex2D(_MainTex, uv + delta_uv);

				//左
				delta_uv = (-delta_xy, 0);

				blured_color += tex2D(_MainTex, uv + delta_uv);

				//右
				delta_uv = (delta_xy, 0);

				blured_color += tex2D(_MainTex, uv + delta_uv);

				//左上
				delta_uv = (-delta_xy, delta_xy);

				blured_color += tex2D(_MainTex, uv + delta_uv);

				//右上
				delta_uv = (delta_xy, delta_xy);

				blured_color += tex2D(_MainTex, uv + delta_uv);

				//左下
				delta_uv = (-delta_xy, -delta_xy);

				blured_color += tex2D(_MainTex, uv + delta_uv);

				//右下
				delta_uv = (delta_xy, -delta_xy);

				blured_color += tex2D(_MainTex, uv + delta_uv);

				blured_color /= 9;

				return blured_color;
			}
			ENDCG
		}
	}
}
