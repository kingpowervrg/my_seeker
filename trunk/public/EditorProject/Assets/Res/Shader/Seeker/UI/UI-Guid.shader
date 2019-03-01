// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "UI/Guid"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		//_CenterPos("CenterPos",Vector) = (0,0,0,0)
		//_Radius("Radius",float) = 0
		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
		_rectInfo0("RectInfo",vector) = (0,0,0,0)
		_rectInfo1("RectInfo1",vector) = (0,0,0,0)
		_circleInfo("CircleInfo",vector) = (0,0,0,0)
		_softFactor("SoftFactor",float) = 1
	/*	_CircleCenter("CircleCenter",Vector) = (0,0,0,0)
		_CircleRadius("CircleRadius",Float) = 0*/
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
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
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
				//float4 worldPos : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;

			/*fixed2 _CircleCenter;
			fixed _CircleRadius;*/
			//uniform float4 circleCenter[4];
			//uniform float circleRadius[4];

			float4 _rectInfo0;
			float4 _rectInfo1;

			float4 _circleInfo;
			fixed _softFactor;
			//float4 rectInfo[4];

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
				//OUT.worldPos =  mul(unity_ObjectToWorld,v.vertex );
                OUT.texcoord = v.texcoord;

                OUT.color = v.color * _Color;
                return OUT;
            }

            sampler2D _MainTex;

			//float4 _CenterPos;
			//float _Radius;

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif
				//color.a *= step(_Radius,distance(IN.worldPos.xy, _CenterPos.xyz));

				//float alphaFactor = saturate(distance(IN.worldPosition.xy, _circleInfo.xy) / _circleInfo.z) * _softFactor;
				//fixed softStep = step(_softFactor,alphaFactor);
				//alphaFactor = saturate( softStep + (1 - softStep) * alphaFactor);
				//color.a *= alphaFactor;

				float alphaFactor = saturate(distance(IN.worldPosition.xy, _circleInfo.xy) / _circleInfo.z);
				fixed softStep = (alphaFactor - _softFactor) / (1.01 - _softFactor);
				fixed needTransparent = step(_softFactor,alphaFactor);
				alphaFactor = saturate( softStep * needTransparent);
				color.a *= alphaFactor;

				half x0 = step(_rectInfo0.x,IN.worldPosition.x);
				half x1 = step(IN.worldPosition.x,_rectInfo0.z);
				half y0 = step(_rectInfo0.y,IN.worldPosition.y);
				half y1 = step(IN.worldPosition.y,_rectInfo0.w);
				color.a *= (1 - x0 * x1*y0*y1);

				half x00 = step(_rectInfo1.x,IN.worldPosition.x);
				half x11 = step(IN.worldPosition.x,_rectInfo1.z);
				half y01 = step(_rectInfo1.y,IN.worldPosition.y);
				half y11 = step(IN.worldPosition.y,_rectInfo1.w);
				color.a *= (1 - x00 * x11*y01*y11);
				//for (int i = 0;i < 4;i++)
				//{
				//	half x0 = step(rectInfo[i].x,IN.worldPosition.x);
				//	half x1 = step(IN.worldPosition.x,rectInfo[i].z);
				//	half y0 = step(rectInfo[i].y,IN.worldPosition.y);
				//	half y1 = step(IN.worldPosition.y,rectInfo[i].w);
				//	//half x0 = step(rectCenter[i].x - rectWidth[i], IN.worldPosition.x);
				//	//half x1 = step(IN.worldPosition.x, rectCenter[i].x + rectWidth[i]);
				//	//half y0 = step(rectCenter[i].y - rectHeigh[i], IN.worldPosition.y);
				//	//half y1 = step(IN.worldPosition.y, rectCenter[i].y + rectHeigh[i]);
				//	//color = half4(x0,x0,x0,x0);
				//	color.a *= (1 - x0 * x1*y0*y1);
				//}

                return color;
            }
        ENDCG
        }
    }
}
