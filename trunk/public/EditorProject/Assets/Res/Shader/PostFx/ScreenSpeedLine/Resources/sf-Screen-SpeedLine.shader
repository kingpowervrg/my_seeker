// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.30 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.30;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vlit:False,simplebake:False,suppproj:False,simplepbl:False,sh:False,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,ugoelinearfog:False,goelinearintensity:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:33348,y:32714,varname:node_3138,prsc:2|custl-752-OUT;n:type:ShaderForge.SFN_Tex2d,id:2241,x:32401,y:32380,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_2241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:2779,x:32411,y:32773,ptovrint:False,ptlb:LineTex,ptin:_LineTex,varname:node_2779,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:ebb4ba6a1076f2c40af54e46ef0196ef,ntxv:0,isnm:False|UVIN-3553-UVOUT;n:type:ShaderForge.SFN_Color,id:7625,x:32401,y:32587,ptovrint:False,ptlb:LineColor,ptin:_LineColor,varname:node_7625,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:1592,x:32411,y:32983,ptovrint:False,ptlb:CutOff (R) ,ptin:_CutOffR,varname:node_1592,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Subtract,id:9954,x:32659,y:32755,varname:node_9954,prsc:2|A-1592-OUT,B-2779-R;n:type:ShaderForge.SFN_TexCoord,id:7035,x:31736,y:32863,varname:node_7035,prsc:2,uv:0;n:type:ShaderForge.SFN_Time,id:3662,x:31736,y:33349,varname:node_3662,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:6202,x:31736,y:33058,ptovrint:False,ptlb:Row,ptin:_Row,varname:node_6202,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Add,id:752,x:33052,y:32528,varname:node_752,prsc:2|A-2241-RGB,B-1255-OUT;n:type:ShaderForge.SFN_Multiply,id:1255,x:32822,y:32589,varname:node_1255,prsc:2|A-7625-RGB,B-2779-RGB,C-9954-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4236,x:31736,y:33144,ptovrint:False,ptlb:Col,ptin:_Col,varname:node_4236,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:6545,x:31736,y:33255,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_6545,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_UVAnimation,id:3553,x:32206,y:32891,varname:node_3553,prsc:2|UVIN-7035-UVOUT,ROWIN-6202-OUT,COLIN-4236-OUT,SPEED-6545-OUT,TIME-3662-T;proporder:2241-2779-7625-6202-4236-6545-1592;pass:END;sub:END;*/

Shader "sf/Screen-SpeedLine" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _LineTex ("LineTex", 2D) = "white" {}
        _LineColor ("LineColor", Color) = (1,1,1,1)
        _Row ("Row", Float ) = 1
        _Col ("Col", Float ) = 1
        _Speed ("Speed", Float ) = 0
        _CutOffR ("CutOff (R) ", Float ) = 1
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _LineTex; uniform float4 _LineTex_ST;
            uniform float4 _LineColor;
            uniform float _CutOffR;
            uniform float _Row;
            uniform float _Col;
            uniform float _Speed;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float4 node_3662 = _Time + _TimeEditor;
                float node_3553_row = floor(node_3662.g * _Speed / _Col);
                float node_3553_col = floor(node_3662.g * _Speed - node_3553_row * _Col);
                float node_3553_source_u = 1.0 / _Col;
                float node_3553_source_v = 1.0 / _Row;
                float2 node_3553_uv = i.uv0;
                node_3553_uv.x *= node_3553_source_u;
                node_3553_uv.y *= node_3553_source_v;
                node_3553_uv.x += node_3553_col * node_3553_source_u;
                node_3553_uv.y = 1 - node_3553_row * node_3553_source_v - node_3553_uv.y;
                float2 node_3553 = node_3553_uv;
                float4 _LineTex_var = tex2D(_LineTex,TRANSFORM_TEX(node_3553, _LineTex));
                float3 finalColor = (_MainTex_var.rgb+(_LineColor.rgb*_LineTex_var.rgb*(_CutOffR-_LineTex_var.r)));
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
