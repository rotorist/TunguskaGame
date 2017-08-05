// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.12 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.12;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,nrsp:0,limd:1,spmd:1,grmd:0,uamb:True,mssp:False,bkdf:True,rprd:True,enco:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,dith:0,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:35525,y:32611,varname:node_1,prsc:2|diff-3812-OUT,spec-3496-OUT,gloss-3510-OUT,normal-3629-OUT,emission-3584-OUT;n:type:ShaderForge.SFN_Lerp,id:2,x:34419,y:31826,varname:node_2,prsc:2|A-5-RGB,B-6-RGB,T-9-R;n:type:ShaderForge.SFN_Tex2d,id:5,x:33749,y:31763,ptovrint:False,ptlb:DIF_01,ptin:_DIF_01,varname:node_5604,prsc:2,tex:f72e9e6f8ba3de343ab6096360874f15,ntxv:0,isnm:False|UVIN-3665-OUT;n:type:ShaderForge.SFN_Tex2d,id:6,x:33738,y:31976,ptovrint:False,ptlb:DIF_02,ptin:_DIF_02,varname:node_6558,prsc:2,tex:23e06d0bc06d18149a9ae3d573baa452,ntxv:0,isnm:False|UVIN-3670-OUT;n:type:ShaderForge.SFN_Tex2d,id:9,x:33170,y:33110,ptovrint:False,ptlb:MASK,ptin:_MASK,varname:node_8124,prsc:2,tex:6a39d5e215ba31a46a057f709d0e1316,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:3496,x:34578,y:32574,varname:node_3496,prsc:2|A-3652-OUT,B-3653-OUT,T-9-R;n:type:ShaderForge.SFN_Desaturate,id:3497,x:34025,y:32151,varname:node_3497,prsc:2|COL-5-RGB;n:type:ShaderForge.SFN_Desaturate,id:3501,x:34002,y:32628,varname:node_3501,prsc:2|COL-6-RGB;n:type:ShaderForge.SFN_Multiply,id:3502,x:34212,y:32192,varname:node_3502,prsc:2|A-3497-OUT,B-3506-OUT;n:type:ShaderForge.SFN_Multiply,id:3503,x:34180,y:32563,varname:node_3503,prsc:2|A-3501-OUT,B-3508-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3506,x:34002,y:32419,ptovrint:False,ptlb:SPC_01,ptin:_SPC_01,varname:node_952,prsc:2,glob:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:3508,x:34002,y:32782,ptovrint:False,ptlb:SPC_02,ptin:_SPC_02,varname:node_743,prsc:2,glob:False,v1:1;n:type:ShaderForge.SFN_Slider,id:3510,x:35545,y:33340,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:node_7297,prsc:2,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Lerp,id:3584,x:35067,y:33059,varname:node_3584,prsc:2|A-3642-OUT,B-3641-OUT,T-9-R;n:type:ShaderForge.SFN_Multiply,id:3590,x:34688,y:32919,varname:node_3590,prsc:2|A-3497-OUT,B-3594-OUT;n:type:ShaderForge.SFN_Multiply,id:3592,x:34691,y:33200,varname:node_3592,prsc:2|A-3501-OUT,B-3596-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3594,x:34500,y:32949,ptovrint:False,ptlb:EM_1,ptin:_EM_1,varname:node_4932,prsc:2,glob:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:3596,x:34505,y:33179,ptovrint:False,ptlb:EM_2,ptin:_EM_2,varname:node_2640,prsc:2,glob:False,v1:0;n:type:ShaderForge.SFN_Tex2d,id:3621,x:34517,y:33475,ptovrint:False,ptlb:NRM_01,ptin:_NRM_01,varname:node_2201,prsc:2,ntxv:3,isnm:True|UVIN-3665-OUT;n:type:ShaderForge.SFN_Tex2d,id:3622,x:34517,y:33668,ptovrint:False,ptlb:NRM_02,ptin:_NRM_02,varname:node_8516,prsc:2,tex:5e2ba7ad893425f4ca5e546f0a779195,ntxv:3,isnm:True|UVIN-3670-OUT;n:type:ShaderForge.SFN_Tex2d,id:3623,x:34517,y:33976,ptovrint:False,ptlb:NRM_03,ptin:_NRM_03,varname:node_4174,prsc:2,ntxv:3,isnm:True|UVIN-3625-OUT;n:type:ShaderForge.SFN_TexCoord,id:3624,x:34155,y:33868,varname:node_3624,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:3625,x:34321,y:33959,varname:node_3625,prsc:2|A-3624-UVOUT,B-3626-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3626,x:34140,y:34072,ptovrint:False,ptlb:Tile_NRM_03,ptin:_Tile_NRM_03,varname:node_128,prsc:2,glob:False,v1:5;n:type:ShaderForge.SFN_ComponentMask,id:3627,x:34747,y:33983,varname:node_3627,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-3623-RGB;n:type:ShaderForge.SFN_Lerp,id:3628,x:34872,y:33563,varname:node_3628,prsc:2|A-3621-RGB,B-3622-RGB,T-9-R;n:type:ShaderForge.SFN_Add,id:3629,x:35088,y:33716,varname:node_3629,prsc:2|A-3628-OUT,B-3627-OUT;n:type:ShaderForge.SFN_Color,id:3636,x:34691,y:33059,ptovrint:False,ptlb:Color_EM_01,ptin:_Color_EM_01,varname:node_4465,prsc:2,glob:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Color,id:3638,x:34691,y:33343,ptovrint:False,ptlb:Color_EM_02,ptin:_Color_EM_02,varname:node_7318,prsc:2,glob:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:3641,x:34889,y:33210,varname:node_3641,prsc:2|A-3592-OUT,B-3638-RGB;n:type:ShaderForge.SFN_Multiply,id:3642,x:34889,y:32975,varname:node_3642,prsc:2|A-3590-OUT,B-3636-RGB;n:type:ShaderForge.SFN_Color,id:3650,x:34180,y:32412,ptovrint:False,ptlb:Color_SPC_01,ptin:_Color_SPC_01,varname:node_4237,prsc:2,glob:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Color,id:3651,x:34169,y:32725,ptovrint:False,ptlb:Color_SPC_02,ptin:_Color_SPC_02,varname:node_9011,prsc:2,glob:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:3652,x:34401,y:32317,varname:node_3652,prsc:2|A-3502-OUT,B-3650-RGB;n:type:ShaderForge.SFN_Multiply,id:3653,x:34386,y:32645,varname:node_3653,prsc:2|A-3503-OUT,B-3651-RGB;n:type:ShaderForge.SFN_TexCoord,id:3664,x:32930,y:32428,varname:node_3664,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:3665,x:33114,y:32413,varname:node_3665,prsc:2|A-3664-UVOUT,B-3666-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3666,x:32930,y:32592,ptovrint:False,ptlb:Tile_DIF_01,ptin:_Tile_DIF_01,varname:node_1913,prsc:2,glob:False,v1:1;n:type:ShaderForge.SFN_TexCoord,id:3668,x:32912,y:32718,varname:node_3668,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:3670,x:33096,y:32703,varname:node_3670,prsc:2|A-3668-UVOUT,B-3672-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3672,x:32912,y:32882,ptovrint:False,ptlb:Tile_DIF_02,ptin:_Tile_DIF_02,varname:node_6940,prsc:2,glob:False,v1:1;n:type:ShaderForge.SFN_Tex2dAsset,id:3694,x:33460,y:31392,ptovrint:False,ptlb:Detail_Map,ptin:_Detail_Map,varname:node_5852,tex:23e06d0bc06d18149a9ae3d573baa452,ntxv:0,isnm:False;n:type:ShaderForge.SFN_TexCoord,id:3696,x:33289,y:31260,varname:node_3696,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:3698,x:33473,y:31245,varname:node_3698,prsc:2|A-3696-UVOUT,B-3700-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3700,x:33289,y:31424,ptovrint:False,ptlb:Detail_Tile,ptin:_Detail_Tile,varname:node_289,prsc:2,glob:False,v1:1;n:type:ShaderForge.SFN_Tex2d,id:3702,x:33752,y:31221,varname:node_7726,prsc:2,tex:23e06d0bc06d18149a9ae3d573baa452,ntxv:0,isnm:False|UVIN-3698-OUT,TEX-3694-TEX;n:type:ShaderForge.SFN_Power,id:3704,x:34094,y:31186,varname:node_3704,prsc:2|VAL-3726-OUT,EXP-3867-OUT;n:type:ShaderForge.SFN_Multiply,id:3706,x:34275,y:31198,varname:node_3706,prsc:2|A-3704-OUT,B-3704-OUT,C-3782-OUT;n:type:ShaderForge.SFN_Desaturate,id:3726,x:33932,y:31160,varname:node_3726,prsc:2|COL-3702-RGB,DES-3806-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3782,x:34105,y:31366,ptovrint:False,ptlb:Detail_Power,ptin:_Detail_Power,varname:node_3524,prsc:2,glob:False,v1:1;n:type:ShaderForge.SFN_Slider,id:3806,x:33918,y:31045,ptovrint:False,ptlb:Detail_Desaturate,ptin:_Detail_Desaturate,varname:node_3334,prsc:2,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:3808,x:34617,y:31683,varname:node_3808,prsc:2|A-3706-OUT,B-2-OUT;n:type:ShaderForge.SFN_Vector1,id:3810,x:34756,y:31933,varname:node_3810,prsc:2,v1:0.5;n:type:ShaderForge.SFN_If,id:3812,x:34955,y:31916,varname:node_3812,prsc:2|A-3814-OUT,B-3810-OUT,GT-3808-OUT,EQ-2-OUT,LT-2-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3814,x:34871,y:31807,ptovrint:False,ptlb:Detail_On/Off(0=OFF/1=ON),ptin:_Detail_OnOff0OFF1ON,varname:node_4717,prsc:2,glob:False,v1:0;n:type:ShaderForge.SFN_Slider,id:3867,x:33805,y:31444,ptovrint:False,ptlb:Detail_Brightness,ptin:_Detail_Brightness,varname:node_8962,prsc:2,min:0,cur:1,max:3;proporder:9-5-3666-3506-3650-6-3672-3508-3651-3510-3621-3622-3623-3626-3594-3636-3596-3638-3694-3814-3700-3782-3867-3806;pass:END;sub:END;*/

Shader "Crowsfield/Dubble_Texture" {
    Properties {
        _MASK ("MASK", 2D) = "white" {}
        _DIF_01 ("DIF_01", 2D) = "white" {}
        _Tile_DIF_01 ("Tile_DIF_01", Float ) = 1
        _SPC_01 ("SPC_01", Float ) = 1
        _Color_SPC_01 ("Color_SPC_01", Color) = (0.5,0.5,0.5,1)
        _DIF_02 ("DIF_02", 2D) = "white" {}
        _Tile_DIF_02 ("Tile_DIF_02", Float ) = 1
        _SPC_02 ("SPC_02", Float ) = 1
        _Color_SPC_02 ("Color_SPC_02", Color) = (0.5,0.5,0.5,1)
        _Gloss ("Gloss", Range(0, 1)) = 0.5
        _NRM_01 ("NRM_01", 2D) = "bump" {}
        _NRM_02 ("NRM_02", 2D) = "bump" {}
        _NRM_03 ("NRM_03", 2D) = "bump" {}
        _Tile_NRM_03 ("Tile_NRM_03", Float ) = 5
        _EM_1 ("EM_1", Float ) = 0
        _Color_EM_01 ("Color_EM_01", Color) = (0.5,0.5,0.5,1)
        _EM_2 ("EM_2", Float ) = 0
        _Color_EM_02 ("Color_EM_02", Color) = (0.5,0.5,0.5,1)
        _Detail_Map ("Detail_Map", 2D) = "white" {}
        _Detail_OnOff0OFF1ON ("Detail_On/Off(0=OFF/1=ON)", Float ) = 0
        _Detail_Tile ("Detail_Tile", Float ) = 1
        _Detail_Power ("Detail_Power", Float ) = 1
        _Detail_Brightness ("Detail_Brightness", Range(0, 3)) = 1
        _Detail_Desaturate ("Detail_Desaturate", Range(0, 1)) = 1
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
            #define _GLOSSYENV 1
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
            uniform sampler2D _DIF_01; uniform float4 _DIF_01_ST;
            uniform sampler2D _DIF_02; uniform float4 _DIF_02_ST;
            uniform sampler2D _MASK; uniform float4 _MASK_ST;
            uniform float _SPC_01;
            uniform float _SPC_02;
            uniform float _Gloss;
            uniform float _EM_1;
            uniform float _EM_2;
            uniform sampler2D _NRM_01; uniform float4 _NRM_01_ST;
            uniform sampler2D _NRM_02; uniform float4 _NRM_02_ST;
            uniform sampler2D _NRM_03; uniform float4 _NRM_03_ST;
            uniform float _Tile_NRM_03;
            uniform float4 _Color_EM_01;
            uniform float4 _Color_EM_02;
            uniform float4 _Color_SPC_01;
            uniform float4 _Color_SPC_02;
            uniform float _Tile_DIF_01;
            uniform float _Tile_DIF_02;
            uniform sampler2D _Detail_Map; uniform float4 _Detail_Map_ST;
            uniform float _Detail_Tile;
            uniform float _Detail_Power;
            uniform float _Detail_Desaturate;
            uniform float _Detail_OnOff0OFF1ON;
            uniform float _Detail_Brightness;
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
            float2 node_3665 = (i.uv0*_Tile_DIF_01);
            float3 _NRM_01_var = UnpackNormal(tex2D(_NRM_01,TRANSFORM_TEX(node_3665, _NRM_01)));
            float2 node_3670 = (i.uv0*_Tile_DIF_02);
            float3 _NRM_02_var = UnpackNormal(tex2D(_NRM_02,TRANSFORM_TEX(node_3670, _NRM_02)));
            float4 _MASK_var = tex2D(_MASK,TRANSFORM_TEX(i.uv0, _MASK));
            float2 node_3625 = (i.uv0*_Tile_NRM_03);
            float3 _NRM_03_var = UnpackNormal(tex2D(_NRM_03,TRANSFORM_TEX(node_3625, _NRM_03)));
            float3 normalLocal = (lerp(_NRM_01_var.rgb,_NRM_02_var.rgb,_MASK_var.r)+float3(_NRM_03_var.rgb.rg,0.0));
            float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
            float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
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
            float4 _DIF_01_var = tex2D(_DIF_01,TRANSFORM_TEX(node_3665, _DIF_01));
            float node_3497 = dot(_DIF_01_var.rgb,float3(0.3,0.59,0.11));
            float4 _DIF_02_var = tex2D(_DIF_02,TRANSFORM_TEX(node_3670, _DIF_02));
            float node_3501 = dot(_DIF_02_var.rgb,float3(0.3,0.59,0.11));
            float3 specularColor = lerp(((node_3497*_SPC_01)*_Color_SPC_01.rgb),((node_3501*_SPC_02)*_Color_SPC_02.rgb),_MASK_var.r);
            float3 directSpecular = 1 * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
            float3 indirectSpecular = (gi.indirect.specular)*specularColor;
            float3 specular = (directSpecular + indirectSpecular);
/// Diffuse:
            NdotL = max(0.0,dot( normalDirection, lightDirection ));
            float3 directDiffuse = max( 0.0, NdotL) * attenColor;
            float3 indirectDiffuse = float3(0,0,0);
            indirectDiffuse += gi.indirect.diffuse;
            float node_3812_if_leA = step(_Detail_OnOff0OFF1ON,0.5);
            float node_3812_if_leB = step(0.5,_Detail_OnOff0OFF1ON);
            float3 node_2 = lerp(_DIF_01_var.rgb,_DIF_02_var.rgb,_MASK_var.r);
            float2 node_3698 = (i.uv0*_Detail_Tile);
            float4 node_7726 = tex2D(_Detail_Map,TRANSFORM_TEX(node_3698, _Detail_Map));
            float3 node_3704 = pow(lerp(node_7726.rgb,dot(node_7726.rgb,float3(0.3,0.59,0.11)),_Detail_Desaturate),_Detail_Brightness);
            float3 diffuseColor = lerp((node_3812_if_leA*node_2)+(node_3812_if_leB*((node_3704*node_3704*_Detail_Power)*node_2)),node_2,node_3812_if_leA*node_3812_if_leB);
            float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
// Emissive:
            float3 emissive = lerp(((node_3497*_EM_1)*_Color_EM_01.rgb),((node_3501*_EM_2)*_Color_EM_02.rgb),_MASK_var.r);
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
        #define _GLOSSYENV 1
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
        uniform sampler2D _DIF_01; uniform float4 _DIF_01_ST;
        uniform sampler2D _DIF_02; uniform float4 _DIF_02_ST;
        uniform sampler2D _MASK; uniform float4 _MASK_ST;
        uniform float _SPC_01;
        uniform float _SPC_02;
        uniform float _Gloss;
        uniform float _EM_1;
        uniform float _EM_2;
        uniform sampler2D _NRM_01; uniform float4 _NRM_01_ST;
        uniform sampler2D _NRM_02; uniform float4 _NRM_02_ST;
        uniform sampler2D _NRM_03; uniform float4 _NRM_03_ST;
        uniform float _Tile_NRM_03;
        uniform float4 _Color_EM_01;
        uniform float4 _Color_EM_02;
        uniform float4 _Color_SPC_01;
        uniform float4 _Color_SPC_02;
        uniform float _Tile_DIF_01;
        uniform float _Tile_DIF_02;
        uniform sampler2D _Detail_Map; uniform float4 _Detail_Map_ST;
        uniform float _Detail_Tile;
        uniform float _Detail_Power;
        uniform float _Detail_Desaturate;
        uniform float _Detail_OnOff0OFF1ON;
        uniform float _Detail_Brightness;
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
            float2 node_3665 = (i.uv0*_Tile_DIF_01);
            float3 _NRM_01_var = UnpackNormal(tex2D(_NRM_01,TRANSFORM_TEX(node_3665, _NRM_01)));
            float2 node_3670 = (i.uv0*_Tile_DIF_02);
            float3 _NRM_02_var = UnpackNormal(tex2D(_NRM_02,TRANSFORM_TEX(node_3670, _NRM_02)));
            float4 _MASK_var = tex2D(_MASK,TRANSFORM_TEX(i.uv0, _MASK));
            float2 node_3625 = (i.uv0*_Tile_NRM_03);
            float3 _NRM_03_var = UnpackNormal(tex2D(_NRM_03,TRANSFORM_TEX(node_3625, _NRM_03)));
            float3 normalLocal = (lerp(_NRM_01_var.rgb,_NRM_02_var.rgb,_MASK_var.r)+float3(_NRM_03_var.rgb.rg,0.0));
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
            float4 _DIF_01_var = tex2D(_DIF_01,TRANSFORM_TEX(node_3665, _DIF_01));
            float node_3497 = dot(_DIF_01_var.rgb,float3(0.3,0.59,0.11));
            float4 _DIF_02_var = tex2D(_DIF_02,TRANSFORM_TEX(node_3670, _DIF_02));
            float node_3501 = dot(_DIF_02_var.rgb,float3(0.3,0.59,0.11));
            float3 specularColor = lerp(((node_3497*_SPC_01)*_Color_SPC_01.rgb),((node_3501*_SPC_02)*_Color_SPC_02.rgb),_MASK_var.r);
            float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
            float3 specular = directSpecular;
/// Diffuse:
            NdotL = max(0.0,dot( normalDirection, lightDirection ));
            float3 directDiffuse = max( 0.0, NdotL) * attenColor;
            float node_3812_if_leA = step(_Detail_OnOff0OFF1ON,0.5);
            float node_3812_if_leB = step(0.5,_Detail_OnOff0OFF1ON);
            float3 node_2 = lerp(_DIF_01_var.rgb,_DIF_02_var.rgb,_MASK_var.r);
            float2 node_3698 = (i.uv0*_Detail_Tile);
            float4 node_7726 = tex2D(_Detail_Map,TRANSFORM_TEX(node_3698, _Detail_Map));
            float3 node_3704 = pow(lerp(node_7726.rgb,dot(node_7726.rgb,float3(0.3,0.59,0.11)),_Detail_Desaturate),_Detail_Brightness);
            float3 diffuseColor = lerp((node_3812_if_leA*node_2)+(node_3812_if_leB*((node_3704*node_3704*_Detail_Power)*node_2)),node_2,node_3812_if_leA*node_3812_if_leB);
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
        uniform sampler2D _DIF_01; uniform float4 _DIF_01_ST;
        uniform sampler2D _DIF_02; uniform float4 _DIF_02_ST;
        uniform sampler2D _MASK; uniform float4 _MASK_ST;
        uniform float _SPC_01;
        uniform float _SPC_02;
        uniform float _Gloss;
        uniform float _EM_1;
        uniform float _EM_2;
        uniform float4 _Color_EM_01;
        uniform float4 _Color_EM_02;
        uniform float4 _Color_SPC_01;
        uniform float4 _Color_SPC_02;
        uniform float _Tile_DIF_01;
        uniform float _Tile_DIF_02;
        uniform sampler2D _Detail_Map; uniform float4 _Detail_Map_ST;
        uniform float _Detail_Tile;
        uniform float _Detail_Power;
        uniform float _Detail_Desaturate;
        uniform float _Detail_OnOff0OFF1ON;
        uniform float _Detail_Brightness;
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
            
            float2 node_3665 = (i.uv0*_Tile_DIF_01);
            float4 _DIF_01_var = tex2D(_DIF_01,TRANSFORM_TEX(node_3665, _DIF_01));
            float node_3497 = dot(_DIF_01_var.rgb,float3(0.3,0.59,0.11));
            float2 node_3670 = (i.uv0*_Tile_DIF_02);
            float4 _DIF_02_var = tex2D(_DIF_02,TRANSFORM_TEX(node_3670, _DIF_02));
            float node_3501 = dot(_DIF_02_var.rgb,float3(0.3,0.59,0.11));
            float4 _MASK_var = tex2D(_MASK,TRANSFORM_TEX(i.uv0, _MASK));
            o.Emission = lerp(((node_3497*_EM_1)*_Color_EM_01.rgb),((node_3501*_EM_2)*_Color_EM_02.rgb),_MASK_var.r);
            
            float node_3812_if_leA = step(_Detail_OnOff0OFF1ON,0.5);
            float node_3812_if_leB = step(0.5,_Detail_OnOff0OFF1ON);
            float3 node_2 = lerp(_DIF_01_var.rgb,_DIF_02_var.rgb,_MASK_var.r);
            float2 node_3698 = (i.uv0*_Detail_Tile);
            float4 node_7726 = tex2D(_Detail_Map,TRANSFORM_TEX(node_3698, _Detail_Map));
            float3 node_3704 = pow(lerp(node_7726.rgb,dot(node_7726.rgb,float3(0.3,0.59,0.11)),_Detail_Desaturate),_Detail_Brightness);
            float3 diffColor = lerp((node_3812_if_leA*node_2)+(node_3812_if_leB*((node_3704*node_3704*_Detail_Power)*node_2)),node_2,node_3812_if_leA*node_3812_if_leB);
            float3 specColor = lerp(((node_3497*_SPC_01)*_Color_SPC_01.rgb),((node_3501*_SPC_02)*_Color_SPC_02.rgb),_MASK_var.r);
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
