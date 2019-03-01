/*
    使用Mask对屏幕后处理   
*/
Shader "Hidden/PostFx/BlendWithMask" 
{
    Properties 
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _MaskTex ("MaskTex", 2D) = "white" {}
		_factor("Factor",Range(0,1)) = 1
    }
    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    sampler2D _MaskTex;
	uniform fixed _factor;
   
    fixed4 frag(v2f_img   i) : SV_Target
    {
        fixed4 color = tex2D(_MainTex, i.uv);
        fixed4 blend = tex2D(_MaskTex, i.uv);

        return lerp(color,float4(0,0,0,0),(1 - blend.r) * _factor);
    }

    ENDCG

    SubShader 
    {
        ZTest Off 
        Cull Off 
        ZWrite Off 
        Blend Off

        Pass{

            CGPROGRAM

            #pragma vertex vert_img
            #pragma fragment frag

            ENDCG

        }

    }

    FallBack Off
}
