// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Amplify Color - Advanced Color Grading for Unity Pro
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

Shader "Hidden/AmplifyColor"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "" {}
		_LerpAmount("LerpAmount",Range(0,1))=1.0
		_LerpRgbTex("LerpRGB (RGB)", 2D) = "" {}
	}

		CGINCLUDE
		#pragma vertex vert
		#pragma fragment frag
		
		#include "UnityCG.cginc"
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_TexelSize;
		uniform float _LerpAmount;
		uniform sampler2D _LerpRgbTex;
		uniform float4 _MainTex_ST;
		struct v2f
		{
			float4 pos : SV_POSITION;
			float4 screenPos : TEXCOORD0;
			float4 uv01 : TEXCOORD1;
			float4 uv01Stereo : TEXCOORD2;
		};
		v2f vert(appdata_img v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.screenPos = ComputeScreenPos(o.pos);
			o.uv01.xy = v.texcoord.xy;
			o.uv01.zw = v.texcoord.xy;

			#if defined( UNITY_UV_STARTS_AT_TOP )
					if (_MainTex_TexelSize.y < 0)
						o.uv01.w = 1 - o.uv01.w;
			#endif
			
			#if defined( UNITY_HALF_TEXEL_OFFSET )
					o.uv01.zw += _MaskTex_TexelSize.xy * float2(-0.5, 0.5);
			#endif
			o.uv01Stereo = UnityStereoScreenSpaceUVAdjust(o.uv01, _MainTex_ST);
			return o;
		}

		inline float4 safe_tex2D(sampler2D tex, float2 uv)
		{
			#if ( UNITY_VERSION >= 540 ) && !defined( SHADER_API_D3D11_9X )
					return tex2Dlod(tex, float4(uv, 0, 0));
			#else
					return tex2D(tex, uv);
			#endif
		}
		inline float4 fetch_process_ldr_gamma(v2f i, const bool mobile)
		{
			// fetch
			float4 color = safe_tex2D(_MainTex, i.uv01Stereo.xy);

			// clamp
			if (mobile)
			{
				color.rgb = min((0.999).xxx, color.rgb); // dev/hw compatibility
			}
			return color;
		}

		inline float4 fetch_process_ldr_linear(v2f i, const bool mobile)
		{
			// fetch
			float4 color = safe_tex2D(_MainTex, i.uv01Stereo.xy);

			// convert to gamma
			color.rgb = max(1.055 * pow(color.rgb, 0.416666667) - 0.055, 0);//to srgb

			// clamp
			if (mobile)
			{
				color.rgb = min((0.999).xxx, color.rgb); // dev/hw compatibility
			}
			return color;
		}
		inline float3 apply_lut(float3 color, const bool mobile)
		{
			const float4 coord_scale = float4(0.0302734375, 0.96875, 31.0, 0.0);
			const float4 coord_offset = float4(0.00048828125, 0.015625, 0.0, 0.0);
			const float2 texel_height_X0 = float2(0.03125, 0.0);

			float3 coord = color * coord_scale.rgb + coord_offset.rgb;

			if (mobile)
			{
				float3 coord_floor = floor(coord + 0.5);
				float2 coord_bot = coord.xy + coord_floor.zz * texel_height_X0;

				float3 lerprgb = safe_tex2D(_LerpRgbTex, coord_bot).rgb;
				color.rgb = lerp(color.rgb, lerprgb, _LerpAmount);
			}
			else
			{
				float3 coord_frac = frac(coord);
				float3 coord_floor = coord - coord_frac;

				float2 coord_bot = coord.xy + coord_floor.zz * texel_height_X0;
				float2 coord_top = coord_bot + texel_height_X0;

				float3 lutcol_bot = safe_tex2D(_LerpRgbTex, coord_bot).rgb;
				float3 lutcol_top = safe_tex2D(_LerpRgbTex, coord_top).rgb;

				float3 lerprgb = lerp(lutcol_bot, lutcol_top, coord_frac.z);
				color.rgb = lerp(color.rgb, lerprgb, _LerpAmount);
			}

			return color;
		}

		inline float4 frag_ldr_gamma(v2f i, const bool mobile)
		{
			float4 color = fetch_process_ldr_gamma(i, mobile);
			color.rgb = apply_lut(color, mobile);
			return color;
		}

		inline float4 frag_ldr_linear(v2f i, const bool mobile)
		{
			float4 color = fetch_process_ldr_linear(i, mobile);
			color.rgb = apply_lut(color, mobile);

			//to linear
			color.rgb = color.rgb * (color.rgb * (color.rgb * 0.305306011 + 0.682171111) + 0.012522878);
			return color;
		}
	ENDCG

	Subshader
	{
		ZTest Always Cull Off ZWrite Off Blend Off Fog{ Mode off }

		// -- QUALITY NORMAL --------------------------------------------------------------
		// 0 => LDR GAMMA
		Pass{ CGPROGRAM float4 frag(v2f i) : SV_Target{ return frag_ldr_gamma(i, false); } ENDCG }

			// 1 => LDR LINEAR
		Pass{ CGPROGRAM float4 frag(v2f i) : SV_Target{ return frag_ldr_linear(i, false); } ENDCG }


			// -- QUALITY MOBILE --------------------------------------------------------------
			// 2 => LDR GAMMA
		Pass{ CGPROGRAM float4 frag(v2f i) : SV_Target{ return frag_ldr_gamma(i, true); } ENDCG }

			// 3 => LDR LINEAR
		Pass{ CGPROGRAM float4 frag(v2f i) : SV_Target{ return frag_ldr_linear(i, true); } ENDCG }

	}

		Fallback Off
}
