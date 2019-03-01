Shader "LJ/Water/water_low"
{

    Properties {        
    _WaterColor("WaterColor",Color) = (0,.25,.4,1)
    _FarColor("FarColor",Color)=(.2,1,1,.3)
    _BumpMap("BumpMap", 2D) = "white" {}
    _BumpPower("BumpPower",Range(-1,1))=.6
    _WaveSize("WaveSize",Range(0.01,10))=.25
    _WaveOffset("WaveOffset(xy&zw)",vector)=(.1,.2,-.2,-.1)
    _LightColor("LightColor",Color)=(1,1,1,1)
    _LightVector("LightVector(xyz for lightDir,w for power)",vector)=(.5,.5,.5,100)
    }
        SubShader{
                Tags{ 
                "RenderType" = "Opaque" 
                "Queue" = "Transparent"
                }
                Blend SrcAlpha OneMinusSrcAlpha
                LOD 200
        Pass{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile DEPTH_ON DEPTH_OFF
            #include "UnityCG.cginc"

        fixed4 _WaterColor;
        fixed4 _FarColor;

        sampler2D _BumpMap;
        half _BumpPower;

        half _WaveSize;
        half4 _WaveOffset;


        fixed4 _LightColor;
        half4 _LightVector;

        struct a2v {
            float4 vertex:POSITION;
            half3 normal : NORMAL;
			float2 uv : TEXCOORD0;
        };
        struct v2f
        {
            half4 pos : POSITION;
            half3 normal:TEXCOORD1;
            half3 viewDir:TEXCOORD2;
            half4 uv : TEXCOORD3;
        };

        half2 fract(half2 val)
        {
            return val - floor(val);
        }

        v2f vert(a2v v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            float4 wPos = mul(unity_ObjectToWorld,v.vertex);
            o.uv.xy = wPos.xz * _WaveSize + _WaveOffset.xy * _Time.y;
            o.uv.zw = wPos.xz * _WaveSize + _WaveOffset.zw * _Time.y;

            o.normal = UnityObjectToWorldNormal(v.normal);
            o.viewDir = WorldSpaceViewDir(v.vertex);
            return o;
        }


        fixed4 frag(v2f i):COLOR {

            fixed4 col=_WaterColor;
            half3 nor = UnpackNormal((tex2D(_BumpMap,fract(i.uv.xy)) + tex2D(_BumpMap,fract(i.uv.zw * 1.2)))*0.5);  
            nor= normalize(i.normal + nor.xyz *half3(1,1,0)* _BumpPower);  
            half spec =max(0,dot(nor,normalize(normalize(_LightVector.xyz)+normalize(i.viewDir))));  
            spec = pow(spec,_LightVector.w); 
            half fresnel=1-saturate(dot(nor,normalize(i.viewDir))); 
            col=lerp(col,_FarColor,fresnel); 
            col.rgb+= _LightColor*spec;  
            return col;  
}
        ENDCG
    }
    }
    FallBack OFF
}