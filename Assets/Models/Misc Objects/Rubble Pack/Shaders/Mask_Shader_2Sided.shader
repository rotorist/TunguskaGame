// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.12 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.12;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,nrsp:0,limd:1,spmd:1,grmd:0,uamb:True,mssp:False,bkdf:True,rprd:True,enco:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:2,dpts:2,wrdp:False,dith:0,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:34962,y:32713,varname:node_1,prsc:2|diff-3865-OUT,spec-63-OUT,gloss-69-OUT,normal-48-OUT,emission-66-OUT,amspl-3918-OUT,alpha-4-R;n:type:ShaderForge.SFN_Tex2d,id:2,x:33395,y:32479,ptovrint:False,ptlb:DIF,ptin:_DIF,varname:node_8871,prsc:2,tex:e40d535b0adf50f4ca15d5ad7ef6bb8e,ntxv:0,isnm:False|UVIN-35-OUT;n:type:ShaderForge.SFN_Tex2d,id:3,x:34374,y:33400,ptovrint:False,ptlb:NRM,ptin:_NRM,varname:node_9626,prsc:2,tex:c2b17506b5081f24b82e4d9f4ede6502,ntxv:3,isnm:True|UVIN-35-OUT;n:type:ShaderForge.SFN_Tex2d,id:4,x:34601,y:33171,ptovrint:False,ptlb:MASK,ptin:_MASK,varname:node_8023,prsc:2,tex:52a252d371220834b9d8321353afe9b0,ntxv:0,isnm:False;n:type:ShaderForge.SFN_TexCoord,id:34,x:32936,y:32958,varname:node_34,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:35,x:33134,y:33006,varname:node_35,prsc:2|A-34-UVOUT,B-36-OUT;n:type:ShaderForge.SFN_ValueProperty,id:36,x:32936,y:33137,ptovrint:False,ptlb:Tile_DIF/NRM,ptin:_Tile_DIFNRM,varname:node_9980,prsc:2,glob:False,v1:1;n:type:ShaderForge.SFN_Tex2d,id:40,x:34109,y:33874,ptovrint:False,ptlb:NRM_DETAIL,ptin:_NRM_DETAIL,varname:node_2661,prsc:2,tex:5e2ba7ad893425f4ca5e546f0a779195,ntxv:3,isnm:True|UVIN-44-OUT;n:type:ShaderForge.SFN_TexCoord,id:42,x:33668,y:33753,varname:node_42,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:44,x:33864,y:33874,varname:node_44,prsc:2|A-42-UVOUT,B-46-OUT;n:type:ShaderForge.SFN_ValueProperty,id:46,x:33666,y:33985,ptovrint:False,ptlb:Tile_NRM_DETAIL,ptin:_Tile_NRM_DETAIL,varname:node_9251,prsc:2,glob:False,v1:5;n:type:ShaderForge.SFN_ComponentMask,id:47,x:34313,y:33887,varname:node_47,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-40-RGB;n:type:ShaderForge.SFN_Add,id:48,x:34625,y:33417,varname:node_48,prsc:2|A-3-RGB,B-47-OUT;n:type:ShaderForge.SFN_Desaturate,id:57,x:33599,y:32659,varname:node_57,prsc:2|COL-2-R;n:type:ShaderForge.SFN_Multiply,id:58,x:33843,y:32685,varname:node_58,prsc:2|A-57-OUT,B-60-OUT;n:type:ShaderForge.SFN_ValueProperty,id:60,x:33599,y:32820,ptovrint:False,ptlb:SPC,ptin:_SPC,varname:node_2647,prsc:2,glob:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:61,x:34339,y:33020,varname:node_61,prsc:2|A-57-OUT,B-62-OUT;n:type:ShaderForge.SFN_ValueProperty,id:62,x:34131,y:33067,ptovrint:False,ptlb:EM,ptin:_EM,varname:node_1259,prsc:2,glob:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:63,x:34043,y:32759,varname:node_63,prsc:2|A-58-OUT,B-64-RGB;n:type:ShaderForge.SFN_Color,id:64,x:33843,y:32836,ptovrint:False,ptlb:Color_SPC,ptin:_Color_SPC,varname:node_4926,prsc:2,glob:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Color,id:65,x:34339,y:33183,ptovrint:False,ptlb:Color_EM,ptin:_Color_EM,varname:node_9519,prsc:2,glob:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:66,x:34601,y:32999,varname:node_66,prsc:2|A-61-OUT,B-65-RGB;n:type:ShaderForge.SFN_Slider,id:69,x:34901,y:32578,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:node_4740,prsc:2,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Tex2dAsset,id:88,x:32914,y:31992,ptovrint:False,ptlb:Detail_Map,ptin:_Detail_Map,varname:node_8341,tex:23e06d0bc06d18149a9ae3d573baa452,ntxv:0,isnm:False;n:type:ShaderForge.SFN_TexCoord,id:3670,x:32743,y:31860,varname:node_3670,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:3672,x:32927,y:31845,varname:node_3672,prsc:2|A-3670-UVOUT,B-3674-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3674,x:32743,y:32024,ptovrint:False,ptlb:Detail_Tile,ptin:_Detail_Tile,varname:node_3818,prsc:2,glob:False,v1:1;n:type:ShaderForge.SFN_Tex2d,id:3676,x:33206,y:31821,varname:node_8024,prsc:2,tex:23e06d0bc06d18149a9ae3d573baa452,ntxv:0,isnm:False|UVIN-3672-OUT,TEX-88-TEX;n:type:ShaderForge.SFN_Power,id:3678,x:33548,y:31786,varname:node_3678,prsc:2|VAL-3726-OUT,EXP-3859-OUT;n:type:ShaderForge.SFN_Multiply,id:3680,x:33729,y:31798,varname:node_3680,prsc:2|A-3678-OUT,B-3678-OUT,C-3782-OUT;n:type:ShaderForge.SFN_Multiply,id:3685,x:34043,y:32199,varname:node_3685,prsc:2|A-3680-OUT,B-2-RGB;n:type:ShaderForge.SFN_Desaturate,id:3726,x:33386,y:31760,varname:node_3726,prsc:2|COL-3676-RGB,DES-3806-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3782,x:33559,y:31966,ptovrint:False,ptlb:Detail_Power,ptin:_Detail_Power,varname:node_9065,prsc:2,glob:False,v1:1;n:type:ShaderForge.SFN_Slider,id:3806,x:33372,y:31645,ptovrint:False,ptlb:Detail_Desaturate,ptin:_Detail_Desaturate,varname:node_164,prsc:2,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Slider,id:3859,x:33203,y:31975,ptovrint:False,ptlb:Detail_Brightness,ptin:_Detail_Brightness,varname:node_4719,prsc:2,min:0,cur:1,max:3;n:type:ShaderForge.SFN_ValueProperty,id:3861,x:34325,y:32407,ptovrint:False,ptlb:Dirt_On/Off(0=OFF/1=ON),ptin:_Dirt_OnOff0OFF1ON,varname:node_3713,prsc:2,glob:False,v1:0;n:type:ShaderForge.SFN_Vector1,id:3863,x:34210,y:32533,varname:node_3863,prsc:2,v1:0.5;n:type:ShaderForge.SFN_If,id:3865,x:34410,y:32533,varname:node_3865,prsc:2|A-3861-OUT,B-3863-OUT,GT-3685-OUT,EQ-2-RGB,LT-2-RGB;n:type:ShaderForge.SFN_ValueProperty,id:3914,x:34485,y:32703,ptovrint:False,ptlb:SAL_On/Off(0=OFF/1=ON),ptin:_SAL_OnOff0OFF1ON,varname:node_6798,prsc:2,glob:False,v1:0;n:type:ShaderForge.SFN_Vector1,id:3916,x:34507,y:32758,varname:node_3916,prsc:2,v1:0.5;n:type:ShaderForge.SFN_If,id:3918,x:34570,y:32829,varname:node_3918,prsc:2|A-3914-OUT,B-3916-OUT,GT-3920-RGB,EQ-3920-RGB,LT-3922-OUT;n:type:ShaderForge.SFN_Cubemap,id:3920,x:34319,y:32780,ptovrint:False,ptlb:SAL_CubeMap,ptin:_SAL_CubeMap,varname:node_792,prsc:2;n:type:ShaderForge.SFN_Vector1,id:3922,x:34350,y:32932,varname:node_3922,prsc:2,v1:0;proporder:4-2-3-36-40-46-69-60-64-3914-3920-62-65-88-3861-3674-3782-3859-3806;pass:END;sub:END;*/

Shader "Crowsfield/Mask_Shader_2Sided" {
    Properties {
        _MASK ("MASK", 2D) = "white" {}
        _DIF ("DIF", 2D) = "white" {}
        _NRM ("NRM", 2D) = "bump" {}
        _Tile_DIFNRM ("Tile_DIF/NRM", Float ) = 1
        _NRM_DETAIL ("NRM_DETAIL", 2D) = "bump" {}
        _Tile_NRM_DETAIL ("Tile_NRM_DETAIL", Float ) = 5
        _Gloss ("Gloss", Range(0, 1)) = 0.5
        _SPC ("SPC", Float ) = 1
        _Color_SPC ("Color_SPC", Color) = (0.5,0.5,0.5,1)
        _SAL_OnOff0OFF1ON ("SAL_On/Off(0=OFF/1=ON)", Float ) = 0
        _SAL_CubeMap ("SAL_CubeMap", Cube) = "_Skybox" {}
        _EM ("EM", Float ) = 0
        _Color_EM ("Color_EM", Color) = (0.5,0.5,0.5,1)
        _Detail_Map ("Detail_Map", 2D) = "white" {}
        _Dirt_OnOff0OFF1ON ("Dirt_On/Off(0=OFF/1=ON)", Float ) = 0
        _Detail_Tile ("Detail_Tile", Float ) = 1
        _Detail_Power ("Detail_Power", Float ) = 1
        _Detail_Brightness ("Detail_Brightness", Range(0, 3)) = 1
        _Detail_Desaturate ("Detail_Desaturate", Range(0, 1)) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 
            #pragma target 3.0
            uniform sampler2D _DIF; uniform float4 _DIF_ST;
            uniform sampler2D _NRM; uniform float4 _NRM_ST;
            uniform sampler2D _MASK; uniform float4 _MASK_ST;
            uniform float _Tile_DIFNRM;
            uniform sampler2D _NRM_DETAIL; uniform float4 _NRM_DETAIL_ST;
            uniform float _Tile_NRM_DETAIL;
            uniform float _SPC;
            uniform float _EM;
            uniform float4 _Color_SPC;
            uniform float4 _Color_EM;
            uniform float _Gloss;
            uniform sampler2D _Detail_Map; uniform float4 _Detail_Map_ST;
            uniform float _Detail_Tile;
            uniform float _Detail_Power;
            uniform float _Detail_Desaturate;
            uniform float _Detail_Brightness;
            uniform float _Dirt_OnOff0OFF1ON;
            uniform float _SAL_OnOff0OFF1ON;
            uniform samplerCUBE _SAL_CubeMap;
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
                UNITY_FOG_COORDS(7)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD8;
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
            return o;
        }
        float4 frag(VertexOutput i) : COLOR {
            i.normalDir = normalize(i.normalDir);
            float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
/// Vectors:
            float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
            float2 node_35 = (i.uv0*_Tile_DIFNRM);
            float3 _NRM_var = UnpackNormal(tex2D(_NRM,TRANSFORM_TEX(node_35, _NRM)));
            float2 node_44 = (i.uv0*_Tile_NRM_DETAIL);
            float3 _NRM_DETAIL_var = UnpackNormal(tex2D(_NRM_DETAIL,TRANSFORM_TEX(node_44, _NRM_DETAIL)));
            float3 normalLocal = (_NRM_var.rgb+float3(_NRM_DETAIL_var.rgb.rg,0.0));
            float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
            
            float nSign = sign( dot( viewDirection, i.normalDir ) ); // Reverse normal if this is a backface
            i.normalDir *= nSign;
            normalDirection *= nSign;
            
            float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
            float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
            float3 lightColor = _LightColor0.rgb;
            float3 halfDirection = normalize(viewDirection+lightDirection);
// Lighting:
            float attenuation = 1;
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
            d.boxMax[0] = unity_SpecCube0_BoxMax;
            d.boxMin[0] = unity_SpecCube0_BoxMin;
            d.probePosition[0] = unity_SpecCube0_ProbePosition;
            d.probeHDR[0] = unity_SpecCube0_HDR;
            d.boxMax[1] = unity_SpecCube1_BoxMax;
            d.boxMin[1] = unity_SpecCube1_BoxMin;
            d.probePosition[1] = unity_SpecCube1_ProbePosition;
            d.probeHDR[1] = unity_SpecCube1_HDR;
            UnityGI gi = UnityGlobalIllumination (d, 1, gloss, normalDirection);
            lightDirection = gi.light.dir;
            lightColor = gi.light.color;
// Specular:
            float NdotL = max(0, dot( normalDirection, lightDirection ));
            float node_3918_if_leA = step(_SAL_OnOff0OFF1ON,0.5);
            float node_3918_if_leB = step(0.5,_SAL_OnOff0OFF1ON);
            float4 _SAL_CubeMap_var = texCUBE(_SAL_CubeMap,viewReflectDirection);
            float4 _DIF_var = tex2D(_DIF,TRANSFORM_TEX(node_35, _DIF));
            float node_57 = dot(_DIF_var.r,float3(0.3,0.59,0.11));
            float3 specularColor = ((node_57*_SPC)*_Color_SPC.rgb);
            float3 directSpecular = 1 * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
            float3 indirectSpecular = (gi.indirect.specular + lerp((node_3918_if_leA*0.0)+(node_3918_if_leB*_SAL_CubeMap_var.rgb),_SAL_CubeMap_var.rgb,node_3918_if_leA*node_3918_if_leB))*specularColor;
            float3 specular = (directSpecular + indirectSpecular);
/// Diffuse:
            NdotL = max(0.0,dot( normalDirection, lightDirection ));
            float3 directDiffuse = max( 0.0, NdotL) * attenColor;
            float3 indirectDiffuse = float3(0,0,0);
            indirectDiffuse += gi.indirect.diffuse;
            float node_3865_if_leA = step(_Dirt_OnOff0OFF1ON,0.5);
            float node_3865_if_leB = step(0.5,_Dirt_OnOff0OFF1ON);
            float2 node_3672 = (i.uv0*_Detail_Tile);
            float4 node_8024 = tex2D(_Detail_Map,TRANSFORM_TEX(node_3672, _Detail_Map));
            float3 node_3678 = pow(lerp(node_8024.rgb,dot(node_8024.rgb,float3(0.3,0.59,0.11)),_Detail_Desaturate),_Detail_Brightness);
            float3 diffuseColor = lerp((node_3865_if_leA*_DIF_var.rgb)+(node_3865_if_leB*((node_3678*node_3678*_Detail_Power)*_DIF_var.rgb)),_DIF_var.rgb,node_3865_if_leA*node_3865_if_leB);
            float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
// Emissive:
            float3 emissive = ((node_57*_EM)*_Color_EM.rgb);
// Final Color:
            float3 finalColor = diffuse + specular + emissive;
            float4 _MASK_var = tex2D(_MASK,TRANSFORM_TEX(i.uv0, _MASK));
            fixed4 finalRGBA = fixed4(finalColor,_MASK_var.r);
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
        Cull Off
        ZWrite Off
        
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #define UNITY_PASS_FORWARDADD
        #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
        #define _GLOSSYENV 1
        #include "UnityCG.cginc"
        #include "AutoLight.cginc"
        #include "Lighting.cginc"
        #include "UnityPBSLighting.cginc"
        #include "UnityStandardBRDF.cginc"
        #pragma multi_compile_fwdadd
        #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
        #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
        #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
        #pragma multi_compile_fog
        #pragma exclude_renderers xbox360 ps3 
        #pragma target 3.0
        uniform sampler2D _DIF; uniform float4 _DIF_ST;
        uniform sampler2D _NRM; uniform float4 _NRM_ST;
        uniform sampler2D _MASK; uniform float4 _MASK_ST;
        uniform float _Tile_DIFNRM;
        uniform sampler2D _NRM_DETAIL; uniform float4 _NRM_DETAIL_ST;
        uniform float _Tile_NRM_DETAIL;
        uniform float _SPC;
        uniform float _EM;
        uniform float4 _Color_SPC;
        uniform float4 _Color_EM;
        uniform float _Gloss;
        uniform sampler2D _Detail_Map; uniform float4 _Detail_Map_ST;
        uniform float _Detail_Tile;
        uniform float _Detail_Power;
        uniform float _Detail_Desaturate;
        uniform float _Detail_Brightness;
        uniform float _Dirt_OnOff0OFF1ON;
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
            float2 node_35 = (i.uv0*_Tile_DIFNRM);
            float3 _NRM_var = UnpackNormal(tex2D(_NRM,TRANSFORM_TEX(node_35, _NRM)));
            float2 node_44 = (i.uv0*_Tile_NRM_DETAIL);
            float3 _NRM_DETAIL_var = UnpackNormal(tex2D(_NRM_DETAIL,TRANSFORM_TEX(node_44, _NRM_DETAIL)));
            float3 normalLocal = (_NRM_var.rgb+float3(_NRM_DETAIL_var.rgb.rg,0.0));
            float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
            
            float nSign = sign( dot( viewDirection, i.normalDir ) ); // Reverse normal if this is a backface
            i.normalDir *= nSign;
            normalDirection *= nSign;
            
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
            float4 _DIF_var = tex2D(_DIF,TRANSFORM_TEX(node_35, _DIF));
            float node_57 = dot(_DIF_var.r,float3(0.3,0.59,0.11));
            float3 specularColor = ((node_57*_SPC)*_Color_SPC.rgb);
            float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
            float3 specular = directSpecular;
/// Diffuse:
            NdotL = max(0.0,dot( normalDirection, lightDirection ));
            float3 directDiffuse = max( 0.0, NdotL) * attenColor;
            float node_3865_if_leA = step(_Dirt_OnOff0OFF1ON,0.5);
            float node_3865_if_leB = step(0.5,_Dirt_OnOff0OFF1ON);
            float2 node_3672 = (i.uv0*_Detail_Tile);
            float4 node_8024 = tex2D(_Detail_Map,TRANSFORM_TEX(node_3672, _Detail_Map));
            float3 node_3678 = pow(lerp(node_8024.rgb,dot(node_8024.rgb,float3(0.3,0.59,0.11)),_Detail_Desaturate),_Detail_Brightness);
            float3 diffuseColor = lerp((node_3865_if_leA*_DIF_var.rgb)+(node_3865_if_leB*((node_3678*node_3678*_Detail_Power)*_DIF_var.rgb)),_DIF_var.rgb,node_3865_if_leA*node_3865_if_leB);
            float3 diffuse = directDiffuse * diffuseColor;
// Final Color:
            float3 finalColor = diffuse + specular;
            float4 _MASK_var = tex2D(_MASK,TRANSFORM_TEX(i.uv0, _MASK));
            return fixed4(finalColor * _MASK_var.r,0);
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
        #define _GLOSSYENV 1
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
        uniform sampler2D _DIF; uniform float4 _DIF_ST;
        uniform float _Tile_DIFNRM;
        uniform float _SPC;
        uniform float _EM;
        uniform float4 _Color_SPC;
        uniform float4 _Color_EM;
        uniform float _Gloss;
        uniform sampler2D _Detail_Map; uniform float4 _Detail_Map_ST;
        uniform float _Detail_Tile;
        uniform float _Detail_Power;
        uniform float _Detail_Desaturate;
        uniform float _Detail_Brightness;
        uniform float _Dirt_OnOff0OFF1ON;
        struct VertexInput {
            float4 vertex : POSITION;
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
        };
        VertexOutput vert (VertexInput v) {
            VertexOutput o = (VertexOutput)0;
            o.uv0 = v.texcoord0;
            o.uv1 = v.texcoord1;
            o.uv2 = v.texcoord2;
            o.posWorld = mul(unity_ObjectToWorld, v.vertex);
            o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
            return o;
        }
        float4 frag(VertexOutput i) : SV_Target {
/// Vectors:
            float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
            UnityMetaInput o;
            UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
            
            float2 node_35 = (i.uv0*_Tile_DIFNRM);
            float4 _DIF_var = tex2D(_DIF,TRANSFORM_TEX(node_35, _DIF));
            float node_57 = dot(_DIF_var.r,float3(0.3,0.59,0.11));
            o.Emission = ((node_57*_EM)*_Color_EM.rgb);
            
            float node_3865_if_leA = step(_Dirt_OnOff0OFF1ON,0.5);
            float node_3865_if_leB = step(0.5,_Dirt_OnOff0OFF1ON);
            float2 node_3672 = (i.uv0*_Detail_Tile);
            float4 node_8024 = tex2D(_Detail_Map,TRANSFORM_TEX(node_3672, _Detail_Map));
            float3 node_3678 = pow(lerp(node_8024.rgb,dot(node_8024.rgb,float3(0.3,0.59,0.11)),_Detail_Desaturate),_Detail_Brightness);
            float3 diffColor = lerp((node_3865_if_leA*_DIF_var.rgb)+(node_3865_if_leB*((node_3678*node_3678*_Detail_Power)*_DIF_var.rgb)),_DIF_var.rgb,node_3865_if_leA*node_3865_if_leB);
            float3 specColor = ((node_57*_SPC)*_Color_SPC.rgb);
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
