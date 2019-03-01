/********************************************************************
	created:  2018-6-15 10:6:26
	filename: ImageBlur.shader
	author:	  songguangze@outlook.com
	
	purpose:  Image,Sprite 虚化效果
*********************************************************************/
Shader "SeekerGame/ImageBlur"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}  
        _Color ("Tint", Color) = (1,1,1,1)  
          
        [HideInInspector]_StencilComp ("Stencil Comparison", Float) = 8  
        [HideInInspector]_Stencil ("Stencil ID", Float) = 0  
        [HideInInspector]_StencilOp ("Stencil Operation", Float) = 0  
        [HideInInspector]_StencilWriteMask ("Stencil Write Mask", Float) = 255  
        [HideInInspector]_StencilReadMask ("Stencil Read Mask", Float) = 255  
  
        [HideInInspector]_ColorMask ("Color Mask", Float) = 15  
  
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0  
          
        _BlurRadius ("BlurRadius", Range(0, 5)) = 0
	}
	SubShader
	{
        Tags  
        {   
            "Queue"="Transparent"   
            "IgnoreProjector"="True"   
            "RenderType"="Transparent"   
            "PreviewType"="Plane"  
            "CanUseSpriteAtlas"="True"  
        }  
          
        Stencil  
        {  
            Ref [_Stencil]  
            Comp [_StencilComp]  
            Pass [_StencilOp]   
            ReadMask [_StencilReadMask]  
            WriteMask [_StencilWriteMask]  
        }  

        
        Cull Off  
        Lighting Off  
        ZWrite Off  
        ZTest [unity_GUIZTestMode]  
        Blend SrcAlpha OneMinusSrcAlpha  
        ColorMask [_ColorMask]  

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
            #include "UnityCG.cginc"  
            #include "UnityUI.cginc"  
  
            #pragma multi_compile __ UNITY_UI_ALPHACLIP  

            struct appdata_t  
            {  
                float4 vertex   : POSITION;  
                float4 color    : COLOR;  
                float2 texcoord : TEXCOORD0;  
                UNITY_VERTEX_INPUT_INSTANCE_ID  
            };  

			struct v2f  
            {  
                float4 vertex   : SV_POSITION;  
                fixed4 color    : COLOR;  
                float2 texcoord  : TEXCOORD0;  
                float4 worldPosition : TEXCOORD1;  
                UNITY_VERTEX_OUTPUT_STEREO  
            };

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			float _BlurRadius;
			float4 _ClipRect;
			float4 _Color;

			
   			v2f vert(appdata_t IN)  
            {  
                v2f OUT;  
                UNITY_SETUP_INSTANCE_ID(IN);  
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);  
                OUT.worldPosition = IN.vertex;  
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);  
  
                OUT.texcoord = IN.texcoord;  
                  
                OUT.color = IN.color * _Color;  
                return OUT;  
            }  
			
			fixed4 frag (v2f i) : SV_Target
			{
				float step1 = 0.00390625f * _BlurRadius*0.5;
                float step2 = step1*2;
                                                        
                float4 result = float4 (0,0,0,0);                            
                float2 texCoord=float2(0,0);

                texCoord = i.texcoord.xy + float2( -step2, -step2 ); result += tex2D(_MainTex,texCoord);
                texCoord = i.texcoord.xy + float2( -step1, -step2 ); result += 4.0 * tex2D(_MainTex,texCoord);
                texCoord = i.texcoord.xy + float2(      0, -step2 ); result += 6.0 * tex2D(_MainTex,texCoord);
                texCoord = i.texcoord.xy + float2(  step1, -step2 ); result += 4.0 * tex2D(_MainTex,texCoord);
                texCoord = i.texcoord.xy + float2(  step2, -step2 ); result += tex2D(_MainTex,texCoord);

                texCoord = i.texcoord.xy + float2( -step2, -step1 ); result += 4.0 * tex2D(_MainTex,texCoord);
                texCoord = i.texcoord.xy + float2( -step1, -step1 ); result += 16.0 * tex2D(_MainTex,texCoord);
                texCoord = i.texcoord.xy + float2(      0, -step1 ); result += 24.0 * tex2D(_MainTex,texCoord);
                texCoord = i.texcoord.xy + float2(  step1, -step1 ); result += 16.0 * tex2D(_MainTex,texCoord);
                texCoord = i.texcoord.xy + float2(  step2, -step1 ); result += 4.0 * tex2D(_MainTex,texCoord);

                texCoord = i.texcoord.xy + float2( -step2,      0 ); result += 6.0 * tex2D(_MainTex,texCoord);
                texCoord = i.texcoord.xy + float2( -step1,      0 ); result += 24.0 * tex2D(_MainTex,texCoord);
                texCoord = i.texcoord.xy; result += 36.0 * tex2D(_MainTex,texCoord);
                texCoord = i.texcoord.xy + float2(  step1,      0 ); result += 24.0 * tex2D(_MainTex,texCoord);
                texCoord = i.texcoord.xy + float2(  step2,      0 ); result += 6.0 * tex2D(_MainTex,texCoord);

                texCoord = i.texcoord.xy + float2( -step2,  step1 ); result += 4.0 * tex2D(_MainTex,texCoord);
                texCoord = i.texcoord.xy + float2( -step1,  step1 ); result += 16.0 * tex2D(_MainTex,texCoord);
                texCoord = i.texcoord.xy + float2(      0,  step1 ); result += 24.0 * tex2D(_MainTex,texCoord);
                texCoord = i.texcoord.xy + float2(  step1,  step1 ); result += 16.0 * tex2D(_MainTex,texCoord);
                texCoord = i.texcoord.xy + float2(  step2,  step1 ); result += 4.0 * tex2D(_MainTex,texCoord);

                texCoord = i.texcoord.xy + float2( -step2,  step2 ); result += tex2D(_MainTex,texCoord);
                texCoord = i.texcoord.xy + float2( -step1,  step2 ); result += 4.0 * tex2D(_MainTex,texCoord);
                texCoord = i.texcoord.xy + float2(      0,  step2 ); result += 6.0 * tex2D(_MainTex,texCoord);
                texCoord = i.texcoord.xy + float2(  step1,  step2 ); result += 4.0 * tex2D(_MainTex,texCoord);
                texCoord = i.texcoord.xy + float2(  step2,  step2 ); result +=  tex2D(_MainTex,texCoord);


                float4 sum=float4(0,0,0,1);
                sum.rgb=result.rgb*0.00390625;
                sum*= i.color.rgba ;
				sum.a *=  UnityGet2DClipping(i.worldPosition.xy, _ClipRect);  
                #ifdef UNITY_UI_ALPHACLIP  
                clip (sum.a - 0.001);  
                #endif  

                return sum; 
			}
			ENDCG
		}
	}
}
