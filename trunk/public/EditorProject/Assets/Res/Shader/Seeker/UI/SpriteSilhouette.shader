/*
	图片轮廓内描边
*/
Shader "Seeker/SpriteSilhouette"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Tint",Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[MaterialToggle] EnableSilhouette("Enable Silouette",Float) = 1
		_SilhouetteColor("Silhoette Color",Color) = (1,1,1,1)
		_SilhouetteWidth("Silhoette Width",Range(1,10)) = 1
		_SilhouetteIntensity("Silhoette Intensity",Range(1,10)) = 1
		_AlphaThreshold("Alpha Threshold",Range(0,1)) = 0.01

		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
	}
	
	SubShader
	{
		  Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha			//UI Sprite 多数使用的混合方式

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#pragma multi_compile _ ENABLESILHOUETTE_ON

			//迭代次数
			#ifndef SAMPLE_DEPTH_LIMIT
            #define SAMPLE_DEPTH_LIMIT 10
            #endif

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

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;

			float _SilhouetteWidth;
		    float4 _SilhouetteColor;			//传入超过1的值，用于HDR获取辉光
		    float _SilhouetteIntensity;			//强度

			float _AlphaThreshold;

			float4 _Color;

			sampler2D _AlphaTex;
			float _EnableExternalAlpha;

			//内描边
			float isDrawInsideSilhoette(float2 uv,float silhoetteSize,float alphaThreshold)
			{

				for(int i = 1; i <= SAMPLE_DEPTH_LIMIT;++i)
				{
					//如果当前像素上方的像素alpha<threshold 说明当前点是边
					float2 pixelUpUV = uv + float2(0,i*_MainTex_TexelSize.y);
					fixed pixelUpAlpha = step(pixelUpUV.y,1.0) * tex2D(_MainTex,pixelUpUV).a;
					if(pixelUpAlpha <= alphaThreshold)
						return 1.0;

					float2 pixelDownUV = uv + float2(0.0,i*-_MainTex_TexelSize.y);
					fixed pixelDownAlpha = step(0.0,pixelDownUV.y) * tex2D(_MainTex,pixelDownUV).a;
					if(pixelDownAlpha <= alphaThreshold)
						return 1.0;

					float2 pixelLeftUV = uv + float2(i*-_MainTex_TexelSize.x,0);
					fixed pixelLeftAlpha = step(0.0,pixelLeftUV.x) * tex2D(_MainTex,pixelLeftUV).a;
					if(pixelLeftAlpha <= alphaThreshold)
						return 1.0;

					float2 pixelRightUV = uv + float2(i*_MainTex_TexelSize.x,0);
					fixed pixelRightAlpha = step(pixelRightUV.x,1.0) * tex2D(_MainTex,pixelRightUV).a;
					if(pixelRightAlpha <= alphaThreshold)
						return 1.0;

					if(i > silhoetteSize)
						break;
				}

				return 0.0;

			}



			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				
				#ifdef PIXELSNAP_ON
				o.vertex = UnityPixelSnap(o.vertex);
				#endif

				return o;
			}


			float4 frag(v2f i) : SV_Target
			{
				float4 color = tex2D(_MainTex,i.uv);

				#if ETC1_EXTERNAL_ALPHA
				float4 alpha = tex2D(_AlphaTex,i.uv);
				color.a = lerp(color.a,alpha.r,_EnableExternalAlpha);
				#endif


				#ifdef ENABLESILHOUETTE_ON
				float silhoetteFactor = isDrawInsideSilhoette(i.uv,_SilhouetteWidth,_AlphaThreshold);
				color.rgb = lerp(color.rgb,_SilhouetteColor.rgb * _SilhouetteIntensity,silhoetteFactor);
				#endif
				
				color.rgb *= color.a;

				return color;

			}
		ENDCG
	}
	}
}
