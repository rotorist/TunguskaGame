// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.12 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.12;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,nrsp:0,limd:1,spmd:1,grmd:0,uamb:True,mssp:True,bkdf:True,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:0,bsrc:0,bdst:1,culm:0,dpts:2,wrdp:True,dith:0,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:36850,y:32595,varname:node_1,prsc:2|diff-257-OUT,spec-639-OUT,gloss-665-OUT,normal-945-OUT,emission-1162-OUT;n:type:ShaderForge.SFN_Multiply,id:15,x:34341,y:33833,varname:node_15,prsc:2|A-2383-OUT,B-617-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:17,x:35131,y:33804,cmnt:NORMAL DIRECTION,varname:node_17,prsc:2,min:0,max:1|IN-2667-OUT;n:type:ShaderForge.SFN_Tex2d,id:21,x:33643,y:32872,ptovrint:False,ptlb:B_Dif,ptin:_B_Dif,varname:node_6863,prsc:2,ntxv:2,isnm:False;n:type:ShaderForge.SFN_NormalVector,id:251,x:33799,y:33769,prsc:2,pt:False;n:type:ShaderForge.SFN_Lerp,id:257,x:36038,y:32433,cmnt:DIFFUSE,varname:node_257,prsc:2|A-1396-OUT,B-1023-OUT,T-17-OUT;n:type:ShaderForge.SFN_OneMinus,id:377,x:33960,y:33769,varname:node_377,prsc:2|IN-251-OUT;n:type:ShaderForge.SFN_Tex2d,id:490,x:34837,y:32273,ptovrint:False,ptlb:A_Dif,ptin:_A_Dif,varname:node_5291,prsc:2,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:617,x:33965,y:33939,ptovrint:False,ptlb:A_Flood,ptin:_A_Flood,varname:node_2282,prsc:2,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Lerp,id:639,x:35170,y:32872,cmnt:SPEC,varname:node_639,prsc:2|A-2006-OUT,B-1037-OUT,T-17-OUT;n:type:ShaderForge.SFN_Multiply,id:656,x:34562,y:32566,varname:node_656,prsc:2|A-1395-OUT,B-658-OUT;n:type:ShaderForge.SFN_Multiply,id:657,x:34418,y:33115,varname:node_657,prsc:2|A-2215-OUT,B-659-OUT;n:type:ShaderForge.SFN_ValueProperty,id:658,x:34393,y:32630,ptovrint:False,ptlb:A_Spec_Amound,ptin:_A_Spec_Amound,varname:node_2442,prsc:2,glob:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:659,x:34213,y:33178,ptovrint:False,ptlb:B_Spec_Amound,ptin:_B_Spec_Amound,varname:node_19,prsc:2,glob:False,v1:0.2;n:type:ShaderForge.SFN_Slider,id:665,x:36054,y:32660,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:node_2616,prsc:2,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Tex2d,id:679,x:35178,y:34192,ptovrint:False,ptlb:A_Normal,ptin:_A_Normal,varname:node_6195,prsc:2,tex:5e2ba7ad893425f4ca5e546f0a779195,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Power,id:923,x:34543,y:33886,varname:node_923,prsc:2|VAL-15-OUT,EXP-1355-OUT;n:type:ShaderForge.SFN_Tex2d,id:944,x:35223,y:34640,ptovrint:False,ptlb:B_Normal,ptin:_B_Normal,varname:node_6171,prsc:2,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Lerp,id:945,x:36133,y:33785,cmnt:NORMAL MAP,varname:node_945,prsc:2|A-2244-OUT,B-1116-OUT,T-17-OUT;n:type:ShaderForge.SFN_Multiply,id:1023,x:34146,y:32855,varname:node_1023,prsc:2|A-21-RGB,B-1087-RGB;n:type:ShaderForge.SFN_Multiply,id:1031,x:34819,y:32721,varname:node_1031,prsc:2|A-656-OUT,B-1080-RGB;n:type:ShaderForge.SFN_Multiply,id:1037,x:34651,y:33093,varname:node_1037,prsc:2|A-657-OUT,B-1074-RGB;n:type:ShaderForge.SFN_Color,id:1074,x:34389,y:33275,ptovrint:False,ptlb:B_Spec_Color,ptin:_B_Spec_Color,varname:node_6826,prsc:2,glob:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:1080,x:34556,y:32714,ptovrint:False,ptlb:A_Spec_Color,ptin:_A_Spec_Color,varname:node_205,prsc:2,glob:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:1086,x:34840,y:32451,ptovrint:False,ptlb:A_Dif_Color,ptin:_A_Dif_Color,varname:node_9617,prsc:2,glob:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:1087,x:33885,y:32927,ptovrint:False,ptlb:B_Dif_Color,ptin:_B_Dif_Color,varname:node_4538,prsc:2,glob:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:1100,x:35222,y:34813,ptovrint:False,ptlb:B_Normal_Detail,ptin:_B_Normal_Detail,varname:node_7535,prsc:2,tex:5e2ba7ad893425f4ca5e546f0a779195,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:1101,x:35020,y:34361,ptovrint:False,ptlb:A_Normal_Detail,ptin:_A_Normal_Detail,varname:node_6360,prsc:2,tex:5e2ba7ad893425f4ca5e546f0a779195,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Multiply,id:1114,x:35469,y:34840,varname:node_1114,prsc:2|A-1100-RGB,B-1115-OUT;n:type:ShaderForge.SFN_Vector3,id:1115,x:35218,y:34973,varname:node_1115,prsc:2,v1:1,v2:1,v3:0;n:type:ShaderForge.SFN_Add,id:1116,x:35516,y:34663,varname:node_1116,prsc:2|A-944-RGB,B-1114-OUT;n:type:ShaderForge.SFN_Multiply,id:1122,x:35200,y:34417,varname:node_1122,prsc:2|A-1101-RGB,B-1123-OUT;n:type:ShaderForge.SFN_Vector3,id:1123,x:35019,y:34514,varname:node_1123,prsc:2,v1:1,v2:1,v3:0;n:type:ShaderForge.SFN_Add,id:1124,x:35445,y:34266,varname:node_1124,prsc:2|A-679-RGB,B-1122-OUT;n:type:ShaderForge.SFN_Lerp,id:1162,x:36477,y:32738,cmnt:Emmission,varname:node_1162,prsc:2|A-1168-OUT,B-1170-OUT,T-17-OUT;n:type:ShaderForge.SFN_Multiply,id:1163,x:36001,y:33159,varname:node_1163,prsc:2|A-1023-OUT,B-1164-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1164,x:35822,y:33302,ptovrint:False,ptlb:B_Em_Amound,ptin:_B_Em_Amound,varname:node_1769,prsc:2,glob:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:1165,x:35537,y:32919,ptovrint:False,ptlb:A_Em_Amound,ptin:_A_Em_Amound,varname:node_7005,prsc:2,glob:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:1166,x:35756,y:32796,varname:node_1166,prsc:2|A-1395-OUT,B-1165-OUT;n:type:ShaderForge.SFN_Color,id:1167,x:35743,y:32962,ptovrint:False,ptlb:A_Em_Color,ptin:_A_Em_Color,varname:node_6975,prsc:2,glob:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:1168,x:35953,y:32891,varname:node_1168,prsc:2|A-1166-OUT,B-1167-RGB;n:type:ShaderForge.SFN_Color,id:1169,x:35998,y:33313,ptovrint:False,ptlb:B_Em_Color,ptin:_B_Em_Color,varname:node_6396,prsc:2,glob:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:1170,x:36211,y:33233,varname:node_1170,prsc:2|A-1163-OUT,B-1169-RGB;n:type:ShaderForge.SFN_Slider,id:1355,x:33967,y:34036,ptovrint:False,ptlb:A_B_Amound,ptin:_A_B_Amound,varname:node_2027,prsc:2,min:0,cur:20,max:40;n:type:ShaderForge.SFN_Multiply,id:1383,x:32736,y:32372,varname:node_1383,prsc:2|A-21-R,B-21-R;n:type:ShaderForge.SFN_Multiply,id:1389,x:32933,y:32372,varname:node_1389,prsc:2|A-1383-OUT,B-1383-OUT,C-1918-OUT;n:type:ShaderForge.SFN_Power,id:1393,x:33364,y:32273,varname:node_1393,prsc:2|VAL-1947-OUT,EXP-1927-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:1395,x:33546,y:32273,cmnt:FallOff,varname:node_1395,prsc:2,min:0,max:1|IN-1393-OUT;n:type:ShaderForge.SFN_Lerp,id:1396,x:35017,y:32092,varname:node_1396,prsc:2|A-1023-OUT,B-2108-OUT,T-1395-OUT;n:type:ShaderForge.SFN_OneMinus,id:1666,x:32933,y:32248,varname:node_1666,prsc:2|IN-1389-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1918,x:32736,y:32520,ptovrint:False,ptlb:FallOff,ptin:_FallOff,varname:node_8400,prsc:2,glob:False,v1:3;n:type:ShaderForge.SFN_ValueProperty,id:1927,x:33170,y:32372,ptovrint:False,ptlb:FallOffStrength,ptin:_FallOffStrength,varname:node_4887,prsc:2,glob:False,v1:10;n:type:ShaderForge.SFN_If,id:1947,x:33170,y:32203,varname:node_1947,prsc:2|A-1948-OUT,B-1959-OUT,GT-1389-OUT,EQ-1389-OUT,LT-1666-OUT;n:type:ShaderForge.SFN_Vector1,id:1948,x:32922,y:32107,varname:node_1948,prsc:2,v1:0.5;n:type:ShaderForge.SFN_ValueProperty,id:1959,x:32933,y:32191,ptovrint:False,ptlb:FallOff_Invert(0=off/1=on),ptin:_FallOff_Invert0off1on,varname:node_7353,prsc:2,glob:False,v1:1;n:type:ShaderForge.SFN_Add,id:2006,x:34983,y:32778,varname:node_2006,prsc:2|A-1031-OUT,B-2016-OUT;n:type:ShaderForge.SFN_Multiply,id:2016,x:34819,y:32850,varname:node_2016,prsc:2|A-1037-OUT,B-2036-OUT;n:type:ShaderForge.SFN_Slider,id:2036,x:34438,y:32948,ptovrint:False,ptlb:Add_B_SpecTo_A,ptin:_Add_B_SpecTo_A,varname:node_508,prsc:2,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:2108,x:35030,y:32267,varname:node_2108,prsc:2|A-490-RGB,B-1086-RGB;n:type:ShaderForge.SFN_Desaturate,id:2215,x:34225,y:33020,varname:node_2215,prsc:2|COL-21-RGB;n:type:ShaderForge.SFN_Lerp,id:2244,x:36135,y:34090,varname:node_2244,prsc:2|A-1124-OUT,B-2496-OUT,T-2615-OUT;n:type:ShaderForge.SFN_ComponentMask,id:2383,x:34121,y:33769,varname:node_2383,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-377-OUT;n:type:ShaderForge.SFN_Vector1,id:2490,x:35459,y:34483,varname:node_2490,prsc:2,v1:0.5;n:type:ShaderForge.SFN_If,id:2496,x:35743,y:34420,varname:node_2496,prsc:2|A-2511-OUT,B-2490-OUT,GT-1116-OUT,EQ-1116-OUT,LT-1124-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2511,x:35459,y:34427,ptovrint:False,ptlb:Add_Normal_B_to_A(0=0ff/1=on),ptin:_Add_Normal_B_to_A00ff1on,varname:node_8283,prsc:2,glob:False,v1:0;n:type:ShaderForge.SFN_OneMinus,id:2569,x:35377,y:33996,varname:node_2569,prsc:2|IN-1395-OUT;n:type:ShaderForge.SFN_Vector1,id:2602,x:35701,y:34087,varname:node_2602,prsc:2,v1:0.5;n:type:ShaderForge.SFN_If,id:2615,x:35923,y:34146,varname:node_2615,prsc:2|A-2616-OUT,B-2602-OUT,GT-2569-OUT,EQ-2618-OUT,LT-2617-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2616,x:35701,y:34032,ptovrint:False,ptlb:FallOff Normal_B_on_A(0=A/0.5=A&B/1=B,ptin:_FallOffNormal_B_on_A0A05AB1B,varname:node_3245,prsc:2,glob:False,v1:1;n:type:ShaderForge.SFN_OneMinus,id:2617,x:35377,y:34128,varname:node_2617,prsc:2|IN-2569-OUT;n:type:ShaderForge.SFN_Vector1,id:2618,x:35701,y:34146,varname:node_2618,prsc:2,v1:1;n:type:ShaderForge.SFN_OneMinus,id:2652,x:34543,y:34024,varname:node_2652,prsc:2|IN-923-OUT;n:type:ShaderForge.SFN_If,id:2667,x:34948,y:33804,varname:node_2667,prsc:2|A-2703-OUT,B-2668-OUT,GT-923-OUT,EQ-923-OUT,LT-2652-OUT;n:type:ShaderForge.SFN_Vector1,id:2668,x:34775,y:33838,varname:node_2668,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Vector1,id:2703,x:34755,y:33767,varname:node_2703,prsc:2,v1:1;proporder:490-1086-21-1087-1355-617-1927-1918-1959-1080-658-1074-659-2036-665-679-1101-944-1100-2511-2616-1165-1167-1164-1169;pass:END;sub:END;*/

Shader "Crowsfield/DualShader" {
    Properties {
        _A_Dif ("A_Dif", 2D) = "white" {}
        _A_Dif_Color ("A_Dif_Color", Color) = (1,1,1,1)
        _B_Dif ("B_Dif", 2D) = "black" {}
        _B_Dif_Color ("B_Dif_Color", Color) = (1,1,1,1)
        _A_B_Amound ("A_B_Amound", Range(0, 40)) = 20
        _A_Flood ("A_Flood", Range(0, 5)) = 1
        _FallOffStrength ("FallOffStrength", Float ) = 10
        _FallOff ("FallOff", Float ) = 3
        _FallOff_Invert0off1on ("FallOff_Invert(0=off/1=on)", Float ) = 1
        _A_Spec_Color ("A_Spec_Color", Color) = (1,1,1,1)
        _A_Spec_Amound ("A_Spec_Amound", Float ) = 1
        _B_Spec_Color ("B_Spec_Color", Color) = (1,1,1,1)
        _B_Spec_Amound ("B_Spec_Amound", Float ) = 0.2
        _Add_B_SpecTo_A ("Add_B_SpecTo_A", Range(0, 1)) = 0
        _Gloss ("Gloss", Range(0, 1)) = 0.5
        _A_Normal ("A_Normal", 2D) = "bump" {}
        _A_Normal_Detail ("A_Normal_Detail", 2D) = "bump" {}
        _B_Normal ("B_Normal", 2D) = "bump" {}
        _B_Normal_Detail ("B_Normal_Detail", 2D) = "bump" {}
        _Add_Normal_B_to_A00ff1on ("Add_Normal_B_to_A(0=0ff/1=on)", Float ) = 0
        _FallOffNormal_B_on_A0A05AB1B ("FallOff Normal_B_on_A(0=A/0.5=A&B/1=B", Float ) = 1
        _A_Em_Amound ("A_Em_Amound", Float ) = 0
        _A_Em_Color ("A_Em_Color", Color) = (1,1,1,1)
        _B_Em_Amound ("B_Em_Amound", Float ) = 0
        _B_Em_Color ("B_Em_Color", Color) = (1,1,1,1)
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
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 
            #pragma target 3.0
            uniform sampler2D _B_Dif; uniform float4 _B_Dif_ST;
            uniform sampler2D _A_Dif; uniform float4 _A_Dif_ST;
            uniform float _A_Flood;
            uniform float _A_Spec_Amound;
            uniform float _B_Spec_Amound;
            uniform float _Gloss;
            uniform sampler2D _A_Normal; uniform float4 _A_Normal_ST;
            uniform sampler2D _B_Normal; uniform float4 _B_Normal_ST;
            uniform float4 _B_Spec_Color;
            uniform float4 _A_Spec_Color;
            uniform float4 _A_Dif_Color;
            uniform float4 _B_Dif_Color;
            uniform sampler2D _B_Normal_Detail; uniform float4 _B_Normal_Detail_ST;
            uniform sampler2D _A_Normal_Detail; uniform float4 _A_Normal_Detail_ST;
            uniform float _B_Em_Amound;
            uniform float _A_Em_Amound;
            uniform float4 _A_Em_Color;
            uniform float4 _B_Em_Color;
            uniform float _A_B_Amound;
            uniform float _FallOff;
            uniform float _FallOffStrength;
            uniform float _FallOff_Invert0off1on;
            uniform float _Add_B_SpecTo_A;
            uniform float _Add_Normal_B_to_A00ff1on;
            uniform float _FallOffNormal_B_on_A0A05AB1B;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD10;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
            #endif
            #ifdef DYNAMICLIGHTMAP_ON
                o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
            #endif
            o.normalDir = UnityObjectToWorldNormal(v.normal);
            o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
            o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
            o.posWorld = mul(unity_ObjectToWorld, v.vertex);
            float3 lightColor = _LightColor0.rgb;
            o.pos = UnityObjectToClipPos(v.vertex);
            UNITY_TRANSFER_FOG(o,o.pos);
            TRANSFER_VERTEX_TO_FRAGMENT(o)
            return o;
        }
        float4 frag(VertexOutput i) : COLOR {
            i.normalDir = normalize(i.normalDir);
            float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
/// Vectors:
            float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
            float3 _A_Normal_var = UnpackNormal(tex2D(_A_Normal,TRANSFORM_TEX(i.uv0, _A_Normal)));
            float3 _A_Normal_Detail_var = UnpackNormal(tex2D(_A_Normal_Detail,TRANSFORM_TEX(i.uv0, _A_Normal_Detail)));
            float3 node_1124 = (_A_Normal_var.rgb+(_A_Normal_Detail_var.rgb*float3(1,1,0)));
            float node_2496_if_leA = step(_Add_Normal_B_to_A00ff1on,0.5);
            float node_2496_if_leB = step(0.5,_Add_Normal_B_to_A00ff1on);
            float3 _B_Normal_var = UnpackNormal(tex2D(_B_Normal,TRANSFORM_TEX(i.uv0, _B_Normal)));
            float3 _B_Normal_Detail_var = UnpackNormal(tex2D(_B_Normal_Detail,TRANSFORM_TEX(i.uv0, _B_Normal_Detail)));
            float3 node_1116 = (_B_Normal_var.rgb+(_B_Normal_Detail_var.rgb*float3(1,1,0)));
            float node_2615_if_leA = step(_FallOffNormal_B_on_A0A05AB1B,0.5);
            float node_2615_if_leB = step(0.5,_FallOffNormal_B_on_A0A05AB1B);
            float node_1947_if_leA = step(0.5,_FallOff_Invert0off1on);
            float node_1947_if_leB = step(_FallOff_Invert0off1on,0.5);
            float4 _B_Dif_var = tex2D(_B_Dif,TRANSFORM_TEX(i.uv0, _B_Dif));
            float node_1383 = (_B_Dif_var.r*_B_Dif_var.r);
            float node_1389 = (node_1383*node_1383*_FallOff);
            float node_1395 = clamp(pow(lerp((node_1947_if_leA*(1.0 - node_1389))+(node_1947_if_leB*node_1389),node_1389,node_1947_if_leA*node_1947_if_leB),_FallOffStrength),0,1); // FallOff
            float node_2569 = (1.0 - node_1395);
            float node_2667_if_leA = step(1.0,0.5);
            float node_2667_if_leB = step(0.5,1.0);
            float node_923 = pow(((1.0 - i.normalDir).g*_A_Flood),_A_B_Amound);
            float node_17 = clamp(lerp((node_2667_if_leA*(1.0 - node_923))+(node_2667_if_leB*node_923),node_923,node_2667_if_leA*node_2667_if_leB),0,1); // NORMAL DIRECTION
            float3 normalLocal = lerp(lerp(node_1124,lerp((node_2496_if_leA*node_1124)+(node_2496_if_leB*node_1116),node_1116,node_2496_if_leA*node_2496_if_leB),lerp((node_2615_if_leA*(1.0 - node_2569))+(node_2615_if_leB*node_2569),1.0,node_2615_if_leA*node_2615_if_leB)),node_1116,node_17);
            float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
            float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
            float3 lightColor = _LightColor0.rgb;
            float3 halfDirection = normalize(viewDirection+lightDirection);
// Lighting:
            float attenuation = LIGHT_ATTENUATION(i);
            float3 attenColor = attenuation * _LightColor0.xyz;
///// Gloss:
            float gloss = _Gloss;
            float specPow = exp2( gloss * 10.0+1.0);
/// GI Data:
            UnityLight light;
            #ifdef LIGHTMAP_OFF
                light.color = lightColor;
                light.dir = lightDirection;
                light.ndotl = LambertTerm (normalDirection, light.dir);
            #else
                light.color = half3(0.f, 0.f, 0.f);
                light.ndotl = 0.0f;
                light.dir = half3(0.f, 0.f, 0.f);
            #endif
            UnityGIInput d;
            d.light = light;
            d.worldPos = i.posWorld.xyz;
            d.worldViewDir = viewDirection;
            d.atten = attenuation;
            #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                d.ambient = 0;
                d.lightmapUV = i.ambientOrLightmapUV;
            #else
                d.ambient = i.ambientOrLightmapUV;
            #endif
            UnityGI gi = UnityGlobalIllumination (d, 1, gloss, normalDirection);
            lightDirection = gi.light.dir;
            lightColor = gi.light.color;
// Specular:
            float NdotL = max(0, dot( normalDirection, lightDirection ));
            float3 node_1037 = ((dot(_B_Dif_var.rgb,float3(0.3,0.59,0.11))*_B_Spec_Amound)*_B_Spec_Color.rgb);
            float3 specularColor = lerp((((node_1395*_A_Spec_Amound)*_A_Spec_Color.rgb)+(node_1037*_Add_B_SpecTo_A)),node_1037,node_17);
            float3 directSpecular = 1 * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
            float3 specular = directSpecular;
/// Diffuse:
            NdotL = max(0.0,dot( normalDirection, lightDirection ));
            float3 directDiffuse = max( 0.0, NdotL) * attenColor;
            float3 indirectDiffuse = float3(0,0,0);
            indirectDiffuse += gi.indirect.diffuse;
            float3 node_1023 = (_B_Dif_var.rgb*_B_Dif_Color.rgb);
            float4 _A_Dif_var = tex2D(_A_Dif,TRANSFORM_TEX(i.uv0, _A_Dif));
            float3 diffuseColor = lerp(lerp(node_1023,(_A_Dif_var.rgb*_A_Dif_Color.rgb),node_1395),node_1023,node_17);
            float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
// Emissive:
            float3 emissive = lerp(((node_1395*_A_Em_Amound)*_A_Em_Color.rgb),((node_1023*_B_Em_Amound)*_B_Em_Color.rgb),node_17);
// Final Color:
            float3 finalColor = diffuse + specular + emissive;
            fixed4 finalRGBA = fixed4(finalColor,1);
            UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
            return finalRGBA;
        }
        ENDCG
    }
    Pass {
        Name "FORWARD_DELTA"
        Tags {
            "LightMode"="ForwardAdd"
        }
        Blend One One
        
        
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #define UNITY_PASS_FORWARDADD
        #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
        #include "UnityCG.cginc"
        #include "AutoLight.cginc"
        #include "Lighting.cginc"
        #include "UnityPBSLighting.cginc"
        #include "UnityStandardBRDF.cginc"
        #pragma multi_compile_fwdadd_fullshadows
        #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
        #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
        #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
        #pragma multi_compile_fog
        #pragma exclude_renderers xbox360 ps3 
        #pragma target 3.0
        uniform sampler2D _B_Dif; uniform float4 _B_Dif_ST;
        uniform sampler2D _A_Dif; uniform float4 _A_Dif_ST;
        uniform float _A_Flood;
        uniform float _A_Spec_Amound;
        uniform float _B_Spec_Amound;
        uniform float _Gloss;
        uniform sampler2D _A_Normal; uniform float4 _A_Normal_ST;
        uniform sampler2D _B_Normal; uniform float4 _B_Normal_ST;
        uniform float4 _B_Spec_Color;
        uniform float4 _A_Spec_Color;
        uniform float4 _A_Dif_Color;
        uniform float4 _B_Dif_Color;
        uniform sampler2D _B_Normal_Detail; uniform float4 _B_Normal_Detail_ST;
        uniform sampler2D _A_Normal_Detail; uniform float4 _A_Normal_Detail_ST;
        uniform float _B_Em_Amound;
        uniform float _A_Em_Amound;
        uniform float4 _A_Em_Color;
        uniform float4 _B_Em_Color;
        uniform float _A_B_Amound;
        uniform float _FallOff;
        uniform float _FallOffStrength;
        uniform float _FallOff_Invert0off1on;
        uniform float _Add_B_SpecTo_A;
        uniform float _Add_Normal_B_to_A00ff1on;
        uniform float _FallOffNormal_B_on_A0A05AB1B;
        struct VertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float4 tangent : TANGENT;
            float2 texcoord0 : TEXCOORD0;
            float2 texcoord1 : TEXCOORD1;
            float2 texcoord2 : TEXCOORD2;
        };
        struct VertexOutput {
            float4 pos : SV_POSITION;
            float2 uv0 : TEXCOORD0;
            float2 uv1 : TEXCOORD1;
            float2 uv2 : TEXCOORD2;
            float4 posWorld : TEXCOORD3;
            float3 normalDir : TEXCOORD4;
            float3 tangentDir : TEXCOORD5;
            float3 bitangentDir : TEXCOORD6;
            LIGHTING_COORDS(7,8)
        };
        VertexOutput vert (VertexInput v) {
            VertexOutput o = (VertexOutput)0;
            o.uv0 = v.texcoord0;
            o.uv1 = v.texcoord1;
            o.uv2 = v.texcoord2;
            o.normalDir = UnityObjectToWorldNormal(v.normal);
            o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
            o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
            o.posWorld = mul(unity_ObjectToWorld, v.vertex);
            float3 lightColor = _LightColor0.rgb;
            o.pos = UnityObjectToClipPos(v.vertex);
            TRANSFER_VERTEX_TO_FRAGMENT(o)
            return o;
        }
        float4 frag(VertexOutput i) : COLOR {
            i.normalDir = normalize(i.normalDir);
            float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
/// Vectors:
            float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
            float3 _A_Normal_var = UnpackNormal(tex2D(_A_Normal,TRANSFORM_TEX(i.uv0, _A_Normal)));
            float3 _A_Normal_Detail_var = UnpackNormal(tex2D(_A_Normal_Detail,TRANSFORM_TEX(i.uv0, _A_Normal_Detail)));
            float3 node_1124 = (_A_Normal_var.rgb+(_A_Normal_Detail_var.rgb*float3(1,1,0)));
            float node_2496_if_leA = step(_Add_Normal_B_to_A00ff1on,0.5);
            float node_2496_if_leB = step(0.5,_Add_Normal_B_to_A00ff1on);
            float3 _B_Normal_var = UnpackNormal(tex2D(_B_Normal,TRANSFORM_TEX(i.uv0, _B_Normal)));
            float3 _B_Normal_Detail_var = UnpackNormal(tex2D(_B_Normal_Detail,TRANSFORM_TEX(i.uv0, _B_Normal_Detail)));
            float3 node_1116 = (_B_Normal_var.rgb+(_B_Normal_Detail_var.rgb*float3(1,1,0)));
            float node_2615_if_leA = step(_FallOffNormal_B_on_A0A05AB1B,0.5);
            float node_2615_if_leB = step(0.5,_FallOffNormal_B_on_A0A05AB1B);
            float node_1947_if_leA = step(0.5,_FallOff_Invert0off1on);
            float node_1947_if_leB = step(_FallOff_Invert0off1on,0.5);
            float4 _B_Dif_var = tex2D(_B_Dif,TRANSFORM_TEX(i.uv0, _B_Dif));
            float node_1383 = (_B_Dif_var.r*_B_Dif_var.r);
            float node_1389 = (node_1383*node_1383*_FallOff);
            float node_1395 = clamp(pow(lerp((node_1947_if_leA*(1.0 - node_1389))+(node_1947_if_leB*node_1389),node_1389,node_1947_if_leA*node_1947_if_leB),_FallOffStrength),0,1); // FallOff
            float node_2569 = (1.0 - node_1395);
            float node_2667_if_leA = step(1.0,0.5);
            float node_2667_if_leB = step(0.5,1.0);
            float node_923 = pow(((1.0 - i.normalDir).g*_A_Flood),_A_B_Amound);
            float node_17 = clamp(lerp((node_2667_if_leA*(1.0 - node_923))+(node_2667_if_leB*node_923),node_923,node_2667_if_leA*node_2667_if_leB),0,1); // NORMAL DIRECTION
            float3 normalLocal = lerp(lerp(node_1124,lerp((node_2496_if_leA*node_1124)+(node_2496_if_leB*node_1116),node_1116,node_2496_if_leA*node_2496_if_leB),lerp((node_2615_if_leA*(1.0 - node_2569))+(node_2615_if_leB*node_2569),1.0,node_2615_if_leA*node_2615_if_leB)),node_1116,node_17);
            float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
            float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
            float3 lightColor = _LightColor0.rgb;
            float3 halfDirection = normalize(viewDirection+lightDirection);
// Lighting:
            float attenuation = LIGHT_ATTENUATION(i);
            float3 attenColor = attenuation * _LightColor0.xyz;
///// Gloss:
            float gloss = _Gloss;
            float specPow = exp2( gloss * 10.0+1.0);
// Specular:
            float NdotL = max(0, dot( normalDirection, lightDirection ));
            float3 node_1037 = ((dot(_B_Dif_var.rgb,float3(0.3,0.59,0.11))*_B_Spec_Amound)*_B_Spec_Color.rgb);
            float3 specularColor = lerp((((node_1395*_A_Spec_Amound)*_A_Spec_Color.rgb)+(node_1037*_Add_B_SpecTo_A)),node_1037,node_17);
            float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
            float3 specular = directSpecular;
/// Diffuse:
            NdotL = max(0.0,dot( normalDirection, lightDirection ));
            float3 directDiffuse = max( 0.0, NdotL) * attenColor;
            float3 node_1023 = (_B_Dif_var.rgb*_B_Dif_Color.rgb);
            float4 _A_Dif_var = tex2D(_A_Dif,TRANSFORM_TEX(i.uv0, _A_Dif));
            float3 diffuseColor = lerp(lerp(node_1023,(_A_Dif_var.rgb*_A_Dif_Color.rgb),node_1395),node_1023,node_17);
            float3 diffuse = directDiffuse * diffuseColor;
// Final Color:
            float3 finalColor = diffuse + specular;
            return fixed4(finalColor * 1,0);
        }
        ENDCG
    }
    Pass {
        Name "Meta"
        Tags {
            "LightMode"="Meta"
        }
        Cull Off
        
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #define UNITY_PASS_META 1
        #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
        #include "UnityCG.cginc"
        #include "Lighting.cginc"
        #include "UnityPBSLighting.cginc"
        #include "UnityStandardBRDF.cginc"
        #include "UnityMetaPass.cginc"
        #pragma fragmentoption ARB_precision_hint_fastest
        #pragma multi_compile_shadowcaster
        #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
        #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
        #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
        #pragma multi_compile_fog
        #pragma exclude_renderers xbox360 ps3 
        #pragma target 3.0
        uniform sampler2D _B_Dif; uniform float4 _B_Dif_ST;
        uniform sampler2D _A_Dif; uniform float4 _A_Dif_ST;
        uniform float _A_Flood;
        uniform float _A_Spec_Amound;
        uniform float _B_Spec_Amound;
        uniform float _Gloss;
        uniform float4 _B_Spec_Color;
        uniform float4 _A_Spec_Color;
        uniform float4 _A_Dif_Color;
        uniform float4 _B_Dif_Color;
        uniform float _B_Em_Amound;
        uniform float _A_Em_Amound;
        uniform float4 _A_Em_Color;
        uniform float4 _B_Em_Color;
        uniform float _A_B_Amound;
        uniform float _FallOff;
        uniform float _FallOffStrength;
        uniform float _FallOff_Invert0off1on;
        uniform float _Add_B_SpecTo_A;
        struct VertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float2 texcoord0 : TEXCOORD0;
            float2 texcoord1 : TEXCOORD1;
            float2 texcoord2 : TEXCOORD2;
        };
        struct VertexOutput {
            float4 pos : SV_POSITION;
            float2 uv0 : TEXCOORD0;
            float2 uv1 : TEXCOORD1;
            float2 uv2 : TEXCOORD2;
            float4 posWorld : TEXCOORD3;
            float3 normalDir : TEXCOORD4;
        };
        VertexOutput vert (VertexInput v) {
            VertexOutput o = (VertexOutput)0;
            o.uv0 = v.texcoord0;
            o.uv1 = v.texcoord1;
            o.uv2 = v.texcoord2;
            o.normalDir = UnityObjectToWorldNormal(v.normal);
            o.posWorld = mul(unity_ObjectToWorld, v.vertex);
            o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
            return o;
        }
        float4 frag(VertexOutput i) : SV_Target {
            i.normalDir = normalize(i.normalDir);
/// Vectors:
            float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
            float3 normalDirection = i.normalDir;
            UnityMetaInput o;
            UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
            
            float node_1947_if_leA = step(0.5,_FallOff_Invert0off1on);
            float node_1947_if_leB = step(_FallOff_Invert0off1on,0.5);
            float4 _B_Dif_var = tex2D(_B_Dif,TRANSFORM_TEX(i.uv0, _B_Dif));
            float node_1383 = (_B_Dif_var.r*_B_Dif_var.r);
            float node_1389 = (node_1383*node_1383*_FallOff);
            float node_1395 = clamp(pow(lerp((node_1947_if_leA*(1.0 - node_1389))+(node_1947_if_leB*node_1389),node_1389,node_1947_if_leA*node_1947_if_leB),_FallOffStrength),0,1); // FallOff
            float3 node_1023 = (_B_Dif_var.rgb*_B_Dif_Color.rgb);
            float node_2667_if_leA = step(1.0,0.5);
            float node_2667_if_leB = step(0.5,1.0);
            float node_923 = pow(((1.0 - i.normalDir).g*_A_Flood),_A_B_Amound);
            float node_17 = clamp(lerp((node_2667_if_leA*(1.0 - node_923))+(node_2667_if_leB*node_923),node_923,node_2667_if_leA*node_2667_if_leB),0,1); // NORMAL DIRECTION
            o.Emission = lerp(((node_1395*_A_Em_Amound)*_A_Em_Color.rgb),((node_1023*_B_Em_Amound)*_B_Em_Color.rgb),node_17);
            
            float4 _A_Dif_var = tex2D(_A_Dif,TRANSFORM_TEX(i.uv0, _A_Dif));
            float3 diffColor = lerp(lerp(node_1023,(_A_Dif_var.rgb*_A_Dif_Color.rgb),node_1395),node_1023,node_17);
            float3 node_1037 = ((dot(_B_Dif_var.rgb,float3(0.3,0.59,0.11))*_B_Spec_Amound)*_B_Spec_Color.rgb);
            float3 specColor = lerp((((node_1395*_A_Spec_Amound)*_A_Spec_Color.rgb)+(node_1037*_Add_B_SpecTo_A)),node_1037,node_17);
            float roughness = 1.0 - _Gloss;
            o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
            
            return UnityMetaFragment( o );
        }
        ENDCG
    }
}
FallBack "Diffuse"
CustomEditor "ShaderForgeMaterialInspector"
}
