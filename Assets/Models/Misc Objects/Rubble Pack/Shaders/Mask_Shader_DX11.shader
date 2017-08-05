// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_LightmapInd', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D
// Upgrade NOTE: replaced tex2D unity_LightmapInd with UNITY_SAMPLE_TEX2D_SAMPLER

// Shader created with Shader Forge Beta 0.25 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.25;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:True,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,hqsc:True,hqlp:False,blpr:0,bsrc:0,bdst:1,culm:0,dpts:2,wrdp:True,ufog:True,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0;n:type:ShaderForge.SFN_Final,id:1,x:32719,y:32712|diff-3863-OUT,spec-63-OUT,gloss-69-OUT,normal-48-OUT,emission-66-OUT,amspl-3919-OUT,clip-4-R,disp-88-OUT,tess-92-OUT;n:type:ShaderForge.SFN_Tex2d,id:2,x:34482,y:32506,ptlb:DIF,tex:e40d535b0adf50f4ca15d5ad7ef6bb8e,ntxv:0,isnm:False|UVIN-35-OUT;n:type:ShaderForge.SFN_Tex2d,id:3,x:33642,y:33682,ptlb:NRM,tex:c2b17506b5081f24b82e4d9f4ede6502,ntxv:3,isnm:True|UVIN-35-OUT;n:type:ShaderForge.SFN_Tex2d,id:4,x:32747,y:33156,ptlb:MASK,tex:52a252d371220834b9d8321353afe9b0,ntxv:0,isnm:False;n:type:ShaderForge.SFN_TexCoord,id:34,x:35128,y:32454,uv:0;n:type:ShaderForge.SFN_Multiply,id:35,x:34930,y:32502|A-34-UVOUT,B-36-OUT;n:type:ShaderForge.SFN_ValueProperty,id:36,x:35128,y:32633,ptlb:Tile_DIF/NRM,v1:1;n:type:ShaderForge.SFN_Tex2d,id:40,x:33642,y:33874,ptlb:NRM_DETAIL,tex:fd96950021780624e9746d1c5b068e90,ntxv:3,isnm:True|UVIN-44-OUT;n:type:ShaderForge.SFN_TexCoord,id:42,x:34083,y:33753,uv:0;n:type:ShaderForge.SFN_Multiply,id:44,x:33887,y:33874|A-42-UVOUT,B-46-OUT;n:type:ShaderForge.SFN_ValueProperty,id:46,x:34085,y:33985,ptlb:Tile_NRM_DETAIL,v1:5;n:type:ShaderForge.SFN_ComponentMask,id:47,x:33438,y:33887,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-40-RGB;n:type:ShaderForge.SFN_Add,id:48,x:33438,y:33715|A-3-RGB,B-47-OUT;n:type:ShaderForge.SFN_Desaturate,id:57,x:34127,y:32675|COL-2-RGB;n:type:ShaderForge.SFN_Multiply,id:58,x:33883,y:32701|A-57-OUT,B-60-OUT;n:type:ShaderForge.SFN_ValueProperty,id:60,x:34127,y:32836,ptlb:SPC,v1:1;n:type:ShaderForge.SFN_Multiply,id:61,x:33412,y:33020|A-57-OUT,B-62-OUT;n:type:ShaderForge.SFN_ValueProperty,id:62,x:33620,y:33067,ptlb:EM,v1:0;n:type:ShaderForge.SFN_Multiply,id:63,x:33683,y:32775|A-58-OUT,B-64-RGB;n:type:ShaderForge.SFN_Color,id:64,x:33883,y:32852,ptlb:Color_SPC,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Color,id:65,x:33412,y:33183,ptlb:Color_EM,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:66,x:33203,y:33073|A-61-OUT,B-65-RGB;n:type:ShaderForge.SFN_Slider,id:69,x:32693,y:32578,ptlb:Gloss,min:0,cur:0.9826303,max:1;n:type:ShaderForge.SFN_Multiply,id:86,x:34805,y:33096|A-284-OUT,B-87-OUT;n:type:ShaderForge.SFN_Vector1,id:87,x:34988,y:33158,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:88,x:34420,y:33516|A-258-OUT,B-89-OUT;n:type:ShaderForge.SFN_NormalVector,id:89,x:34420,y:33651,pt:False;n:type:ShaderForge.SFN_ValueProperty,id:92,x:33012,y:33333,ptlb:TESS_Amound,v1:10;n:type:ShaderForge.SFN_Power,id:114,x:34954,y:33271|VAL-86-OUT,EXP-115-OUT;n:type:ShaderForge.SFN_Vector1,id:115,x:35115,y:33321,v1:1;n:type:ShaderForge.SFN_Power,id:201,x:34826,y:33381|VAL-114-OUT,EXP-202-OUT;n:type:ShaderForge.SFN_Vector1,id:202,x:35014,y:33434,v1:1;n:type:ShaderForge.SFN_Add,id:205,x:34601,y:33253|A-86-OUT,B-201-OUT,C-114-OUT;n:type:ShaderForge.SFN_Vector1,id:206,x:34806,y:33516,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:258,x:34420,y:33281|A-205-OUT,B-269-OUT;n:type:ShaderForge.SFN_ValueProperty,id:269,x:34601,y:33401,ptlb:TES_Hight,v1:0.1;n:type:ShaderForge.SFN_Desaturate,id:284,x:35095,y:32965|COL-285-RGB;n:type:ShaderForge.SFN_Tex2d,id:285,x:35280,y:32965,ptlb:TES,ntxv:0,isnm:False|UVIN-35-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3792,x:33426,y:32407,ptlb:Detail_On/Off(0=OFF/1=ON),v1:0;n:type:ShaderForge.SFN_Tex2dAsset,id:3794,x:34837,y:31992,ptlb:DetailMap,tex:23e06d0bc06d18149a9ae3d573baa452;n:type:ShaderForge.SFN_TexCoord,id:3796,x:35008,y:31860,uv:0;n:type:ShaderForge.SFN_Multiply,id:3798,x:34824,y:31845|A-3796-UVOUT,B-3800-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3800,x:35008,y:32024,ptlb:Detail_Tile,v1:1;n:type:ShaderForge.SFN_Tex2d,id:3802,x:34545,y:31821,tex:23e06d0bc06d18149a9ae3d573baa452,ntxv:0,isnm:False|UVIN-3798-OUT,TEX-3794-TEX;n:type:ShaderForge.SFN_Power,id:3804,x:34203,y:31786|VAL-3810-OUT,EXP-3859-OUT;n:type:ShaderForge.SFN_Multiply,id:3806,x:34022,y:31798|A-3804-OUT,B-3804-OUT,C-3812-OUT;n:type:ShaderForge.SFN_Multiply,id:3808,x:33680,y:32283|A-3806-OUT,B-2-RGB;n:type:ShaderForge.SFN_Desaturate,id:3810,x:34365,y:31760|COL-3802-RGB,DES-3814-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3812,x:34192,y:31966,ptlb:Detail_Power,v1:1;n:type:ShaderForge.SFN_Slider,id:3814,x:34222,y:31645,ptlb:Detail_Desaturate,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Slider,id:3859,x:34391,y:31975,ptlb:Detail_Brightness,min:0,cur:1,max:3;n:type:ShaderForge.SFN_Vector1,id:3861,x:33541,y:32533,v1:0.5;n:type:ShaderForge.SFN_If,id:3863,x:33341,y:32533|A-3792-OUT,B-3861-OUT,GT-3808-OUT,EQ-2-RGB,LT-2-RGB;n:type:ShaderForge.SFN_ValueProperty,id:3915,x:33257,y:32709,ptlb:SAL_On/Off(0=OFF/1=ON),v1:0;n:type:ShaderForge.SFN_Vector1,id:3917,x:33235,y:32764,v1:0.5;n:type:ShaderForge.SFN_If,id:3919,x:33172,y:32835|A-3915-OUT,B-3917-OUT,GT-3921-RGB,EQ-3921-RGB,LT-3923-OUT;n:type:ShaderForge.SFN_Cubemap,id:3921,x:33423,y:32786,ptlb:SAL_CubeMap;n:type:ShaderForge.SFN_Vector1,id:3923,x:33392,y:32938,v1:0;proporder:4-2-3-36-40-46-69-60-64-3915-3921-62-65-285-92-269-3794-3792-3800-3812-3859-3814;pass:END;sub:END;*/

Shader "Crowsfield/Mask_Shader_DX11" {
    Properties {
        _MASK ("MASK", 2D) = "white" {}
        _DIF ("DIF", 2D) = "white" {}
        _NRM ("NRM", 2D) = "bump" {}
        _TileDIFNRM ("Tile_DIF/NRM", Float ) = 1
        _NRMDETAIL ("NRM_DETAIL", 2D) = "bump" {}
        _TileNRMDETAIL ("Tile_NRM_DETAIL", Float ) = 5
        _Gloss ("Gloss", Range(0, 1)) = 0.9826303
        _SPC ("SPC", Float ) = 1
        _ColorSPC ("Color_SPC", Color) = (0.5,0.5,0.5,1)
        _SALOnOff0OFF1ON ("SAL_On/Off(0=OFF/1=ON)", Float ) = 0
        _SALCubeMap ("SAL_CubeMap", Cube) = "_Skybox" {}
        _EM ("EM", Float ) = 0
        _ColorEM ("Color_EM", Color) = (0.5,0.5,0.5,1)
        _TES ("TES", 2D) = "white" {}
        _TESSAmound ("TESS_Amound", Float ) = 10
        _TESHight ("TES_Hight", Float ) = 0.1
        _DetailMap ("DetailMap", 2D) = "white" {}
        _DetailOnOff0OFF1ON ("Detail_On/Off(0=OFF/1=ON)", Float ) = 0
        _DetailTile ("Detail_Tile", Float ) = 1
        _DetailPower ("Detail_Power", Float ) = 1
        _DetailBrightness ("Detail_Brightness", Range(0, 3)) = 1
        _DetailDesaturate ("Detail_Desaturate", Range(0, 1)) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 5.0
            #ifndef LIGHTMAP_OFF
                // sampler2D unity_Lightmap;
                // float4 unity_LightmapST;
                #ifndef DIRLIGHTMAP_OFF
                    // sampler2D unity_LightmapInd;
                #endif
            #endif
            uniform sampler2D _DIF; uniform float4 _DIF_ST;
            uniform sampler2D _NRM; uniform float4 _NRM_ST;
            uniform sampler2D _MASK; uniform float4 _MASK_ST;
            uniform float _TileDIFNRM;
            uniform sampler2D _NRMDETAIL; uniform float4 _NRMDETAIL_ST;
            uniform float _TileNRMDETAIL;
            uniform float _SPC;
            uniform float _EM;
            uniform float4 _ColorSPC;
            uniform float4 _ColorEM;
            uniform float _Gloss;
            uniform float _TESSAmound;
            uniform float _TESHight;
            uniform sampler2D _TES; uniform float4 _TES_ST;
            uniform float _DetailOnOff0OFF1ON;
            uniform sampler2D _DetailMap; uniform float4 _DetailMap_ST;
            uniform float _DetailTile;
            uniform float _DetailPower;
            uniform float _DetailDesaturate;
            uniform float _DetailBrightness;
            uniform float _SALOnOff0OFF1ON;
            uniform samplerCUBE _SALCubeMap;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
                #ifndef LIGHTMAP_OFF
                    float2 uvLM : TEXCOORD7;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                #ifndef LIGHTMAP_OFF
                    o.uvLM = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                #endif
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float4 uv0 : TEXCOORD0;
                    float4 uv1 : TEXCOORD1;
                };
                struct OutputPatchConstant {
                    float edge[3]         : SV_TessFactor;
                    float inside          : SV_InsideTessFactor;
                    float3 vTangent[4]    : TANGENT;
                    float2 vUV[4]         : TEXCOORD;
                    float3 vTanUCorner[4] : TANUCORNER;
                    float3 vTanVCorner[4] : TANVCORNER;
                    float4 vCWts          : TANWEIGHTS;
                };
                TessVertex tessvert (VertexInput v) {
                    TessVertex o;
                    o.vertex = v.vertex;
                    o.normal = v.normal;
                    o.tangent = v.tangent;
                    o.uv0 = v.uv0;
                    o.uv1 = v.uv1;
                    return o;
                }
                void displacement (inout VertexInput v){
                    float2 node_35 = (v.uv0.rg*_TileDIFNRM);
                    float node_86 = (dot(tex2Dlod(_TES,float4(TRANSFORM_TEX(node_35, _TES),0.0,0)).rgb,float3(0.3,0.59,0.11))*0.5);
                    float node_114 = pow(node_86,1.0);
                    v.vertex.xyz +=  (((node_86+pow(node_114,1.0)+node_114)*_TESHight)*v.normal);
                }
                float Tessellation(TessVertex v){
                    return _TESSAmound;
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o;
                    float ts = Tessellation( v[0] );
                    o.edge[0] = ts;
                    o.edge[1] = ts;
                    o.edge[2] = ts;
                    o.inside = ts;
                    return o;
                }
                [domain("tri")]
                [partitioning("fractional_odd")]
                [outputtopology("triangle_cw")]
                [patchconstantfunc("hullconst")]
                [outputcontrolpoints(3)]
                TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
                    return v[id];
                }
                [domain("tri")]
                VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
                    VertexInput v;
                    v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
                    v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
                    v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
                    v.uv0 = vi[0].uv0*bary.x + vi[1].uv0*bary.y + vi[2].uv0*bary.z;
                    v.uv1 = vi[0].uv1*bary.x + vi[1].uv1*bary.y + vi[2].uv1*bary.z;
                    displacement(v);
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            fixed4 frag(VertexOutput i) : COLOR {
                float2 node_3954 = i.uv0;
                clip(tex2D(_MASK,TRANSFORM_TEX(node_3954.rg, _MASK)).r - 0.5);
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_35 = (i.uv0.rg*_TileDIFNRM);
                float2 node_44 = (i.uv0.rg*_TileNRMDETAIL);
                float3 normalLocal = (UnpackNormal(tex2D(_NRM,TRANSFORM_TEX(node_35, _NRM))).rgb+float3(UnpackNormal(tex2D(_NRMDETAIL,TRANSFORM_TEX(node_44, _NRMDETAIL))).rgb.rg,0.0));
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                #ifndef LIGHTMAP_OFF
                    float4 lmtex = UNITY_SAMPLE_TEX2D(unity_Lightmap,i.uvLM);
                    #ifndef DIRLIGHTMAP_OFF
                        float3 lightmap = DecodeLightmap(lmtex);
                        float3 scalePerBasisVector = DecodeLightmap(UNITY_SAMPLE_TEX2D_SAMPLER(unity_LightmapInd,unity_Lightmap,i.uvLM));
                        UNITY_DIRBASIS
                        half3 normalInRnmBasis = saturate (mul (unity_DirBasis, normalLocal));
                        lightmap *= dot (normalInRnmBasis, scalePerBasisVector);
                    #else
                        float3 lightmap = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap,i.uvLM));
                    #endif
                #endif
                #ifndef LIGHTMAP_OFF
                    #ifdef DIRLIGHTMAP_OFF
                        float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                    #else
                        float3 lightDirection = normalize (scalePerBasisVector.x * unity_DirBasis[0] + scalePerBasisVector.y * unity_DirBasis[1] + scalePerBasisVector.z * unity_DirBasis[2]);
                        lightDirection = mul(lightDirection,tangentTransform); // Tangent to world
                    #endif
                #else
                    float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                #endif
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                #ifndef LIGHTMAP_OFF
                    float3 diffuse = lightmap;
                #else
                    float3 diffuse = max( 0.0, NdotL) * attenColor + UNITY_LIGHTMODEL_AMBIENT.xyz;
                #endif
////// Emissive:
                float4 node_2 = tex2D(_DIF,TRANSFORM_TEX(node_35, _DIF));
                float node_57 = dot(node_2.rgb,float3(0.3,0.59,0.11));
                float3 emissive = ((node_57*_EM)*_ColorEM.rgb);
///////// Gloss:
                float gloss = exp2(_Gloss*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float node_3919_if_leA = step(_SALOnOff0OFF1ON,0.5);
                float node_3919_if_leB = step(0.5,_SALOnOff0OFF1ON);
                float4 node_3921 = texCUBE(_SALCubeMap,viewReflectDirection);
                float3 specularColor = ((node_57*_SPC)*_ColorSPC.rgb);
                float3 specularAmb = lerp((node_3919_if_leA*0.0)+(node_3919_if_leB*node_3921.rgb),node_3921.rgb,node_3919_if_leA*node_3919_if_leB) * specularColor;
                float3 specular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor + specularAmb;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float node_3863_if_leA = step(_DetailOnOff0OFF1ON,0.5);
                float node_3863_if_leB = step(0.5,_DetailOnOff0OFF1ON);
                float2 node_3798 = (i.uv0.rg*_DetailTile);
                float3 node_3804 = pow(lerp(tex2D(_DetailMap,TRANSFORM_TEX(node_3798, _DetailMap)).rgb,dot(tex2D(_DetailMap,TRANSFORM_TEX(node_3798, _DetailMap)).rgb,float3(0.3,0.59,0.11)),_DetailDesaturate),_DetailBrightness);
                finalColor += diffuseLight * lerp((node_3863_if_leA*node_2.rgb)+(node_3863_if_leB*((node_3804*node_3804*_DetailPower)*node_2.rgb)),node_2.rgb,node_3863_if_leA*node_3863_if_leB);
                finalColor += specular;
                finalColor += emissive;
/// Final Color:
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 5.0
            #ifndef LIGHTMAP_OFF
                // sampler2D unity_Lightmap;
                // float4 unity_LightmapST;
                #ifndef DIRLIGHTMAP_OFF
                    // sampler2D unity_LightmapInd;
                #endif
            #endif
            uniform sampler2D _DIF; uniform float4 _DIF_ST;
            uniform sampler2D _NRM; uniform float4 _NRM_ST;
            uniform sampler2D _MASK; uniform float4 _MASK_ST;
            uniform float _TileDIFNRM;
            uniform sampler2D _NRMDETAIL; uniform float4 _NRMDETAIL_ST;
            uniform float _TileNRMDETAIL;
            uniform float _SPC;
            uniform float _EM;
            uniform float4 _ColorSPC;
            uniform float4 _ColorEM;
            uniform float _Gloss;
            uniform float _TESSAmound;
            uniform float _TESHight;
            uniform sampler2D _TES; uniform float4 _TES_ST;
            uniform float _DetailOnOff0OFF1ON;
            uniform sampler2D _DetailMap; uniform float4 _DetailMap_ST;
            uniform float _DetailTile;
            uniform float _DetailPower;
            uniform float _DetailDesaturate;
            uniform float _DetailBrightness;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float4 uv0 : TEXCOORD0;
                    float4 uv1 : TEXCOORD1;
                };
                struct OutputPatchConstant {
                    float edge[3]         : SV_TessFactor;
                    float inside          : SV_InsideTessFactor;
                    float3 vTangent[4]    : TANGENT;
                    float2 vUV[4]         : TEXCOORD;
                    float3 vTanUCorner[4] : TANUCORNER;
                    float3 vTanVCorner[4] : TANVCORNER;
                    float4 vCWts          : TANWEIGHTS;
                };
                TessVertex tessvert (VertexInput v) {
                    TessVertex o;
                    o.vertex = v.vertex;
                    o.normal = v.normal;
                    o.tangent = v.tangent;
                    o.uv0 = v.uv0;
                    o.uv1 = v.uv1;
                    return o;
                }
                void displacement (inout VertexInput v){
                    float2 node_35 = (v.uv0.rg*_TileDIFNRM);
                    float node_86 = (dot(tex2Dlod(_TES,float4(TRANSFORM_TEX(node_35, _TES),0.0,0)).rgb,float3(0.3,0.59,0.11))*0.5);
                    float node_114 = pow(node_86,1.0);
                    v.vertex.xyz +=  (((node_86+pow(node_114,1.0)+node_114)*_TESHight)*v.normal);
                }
                float Tessellation(TessVertex v){
                    return _TESSAmound;
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o;
                    float ts = Tessellation( v[0] );
                    o.edge[0] = ts;
                    o.edge[1] = ts;
                    o.edge[2] = ts;
                    o.inside = ts;
                    return o;
                }
                [domain("tri")]
                [partitioning("fractional_odd")]
                [outputtopology("triangle_cw")]
                [patchconstantfunc("hullconst")]
                [outputcontrolpoints(3)]
                TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
                    return v[id];
                }
                [domain("tri")]
                VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
                    VertexInput v;
                    v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
                    v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
                    v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
                    v.uv0 = vi[0].uv0*bary.x + vi[1].uv0*bary.y + vi[2].uv0*bary.z;
                    v.uv1 = vi[0].uv1*bary.x + vi[1].uv1*bary.y + vi[2].uv1*bary.z;
                    displacement(v);
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            fixed4 frag(VertexOutput i) : COLOR {
                float2 node_3955 = i.uv0;
                clip(tex2D(_MASK,TRANSFORM_TEX(node_3955.rg, _MASK)).r - 0.5);
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_35 = (i.uv0.rg*_TileDIFNRM);
                float2 node_44 = (i.uv0.rg*_TileNRMDETAIL);
                float3 normalLocal = (UnpackNormal(tex2D(_NRM,TRANSFORM_TEX(node_35, _NRM))).rgb+float3(UnpackNormal(tex2D(_NRMDETAIL,TRANSFORM_TEX(node_44, _NRMDETAIL))).rgb.rg,0.0));
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor;
///////// Gloss:
                float gloss = exp2(_Gloss*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float4 node_2 = tex2D(_DIF,TRANSFORM_TEX(node_35, _DIF));
                float node_57 = dot(node_2.rgb,float3(0.3,0.59,0.11));
                float3 specularColor = ((node_57*_SPC)*_ColorSPC.rgb);
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float node_3863_if_leA = step(_DetailOnOff0OFF1ON,0.5);
                float node_3863_if_leB = step(0.5,_DetailOnOff0OFF1ON);
                float2 node_3798 = (i.uv0.rg*_DetailTile);
                float3 node_3804 = pow(lerp(tex2D(_DetailMap,TRANSFORM_TEX(node_3798, _DetailMap)).rgb,dot(tex2D(_DetailMap,TRANSFORM_TEX(node_3798, _DetailMap)).rgb,float3(0.3,0.59,0.11)),_DetailDesaturate),_DetailBrightness);
                finalColor += diffuseLight * lerp((node_3863_if_leA*node_2.rgb)+(node_3863_if_leB*((node_3804*node_3804*_DetailPower)*node_2.rgb)),node_2.rgb,node_3863_if_leA*node_3863_if_leB);
                finalColor += specular;
/// Final Color:
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCollector"
            Tags {
                "LightMode"="ShadowCollector"
            }
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCOLLECTOR
            #define SHADOW_COLLECTOR_PASS
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcollector
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 5.0
            #ifndef LIGHTMAP_OFF
                // sampler2D unity_Lightmap;
                // float4 unity_LightmapST;
                #ifndef DIRLIGHTMAP_OFF
                    // sampler2D unity_LightmapInd;
                #endif
            #endif
            uniform sampler2D _MASK; uniform float4 _MASK_ST;
            uniform float _TileDIFNRM;
            uniform float _TESSAmound;
            uniform float _TESHight;
            uniform sampler2D _TES; uniform float4 _TES_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_COLLECTOR;
                float4 uv0 : TEXCOORD5;
                float3 normalDir : TEXCOORD6;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_SHADOW_COLLECTOR(o)
                return o;
            }
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float4 uv0 : TEXCOORD0;
                };
                struct OutputPatchConstant {
                    float edge[3]         : SV_TessFactor;
                    float inside          : SV_InsideTessFactor;
                    float3 vTangent[4]    : TANGENT;
                    float2 vUV[4]         : TEXCOORD;
                    float3 vTanUCorner[4] : TANUCORNER;
                    float3 vTanVCorner[4] : TANVCORNER;
                    float4 vCWts          : TANWEIGHTS;
                };
                TessVertex tessvert (VertexInput v) {
                    TessVertex o;
                    o.vertex = v.vertex;
                    o.normal = v.normal;
                    o.tangent = v.tangent;
                    o.uv0 = v.uv0;
                    return o;
                }
                void displacement (inout VertexInput v){
                    float2 node_35 = (v.uv0.rg*_TileDIFNRM);
                    float node_86 = (dot(tex2Dlod(_TES,float4(TRANSFORM_TEX(node_35, _TES),0.0,0)).rgb,float3(0.3,0.59,0.11))*0.5);
                    float node_114 = pow(node_86,1.0);
                    v.vertex.xyz +=  (((node_86+pow(node_114,1.0)+node_114)*_TESHight)*v.normal);
                }
                float Tessellation(TessVertex v){
                    return _TESSAmound;
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o;
                    float ts = Tessellation( v[0] );
                    o.edge[0] = ts;
                    o.edge[1] = ts;
                    o.edge[2] = ts;
                    o.inside = ts;
                    return o;
                }
                [domain("tri")]
                [partitioning("fractional_odd")]
                [outputtopology("triangle_cw")]
                [patchconstantfunc("hullconst")]
                [outputcontrolpoints(3)]
                TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
                    return v[id];
                }
                [domain("tri")]
                VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
                    VertexInput v;
                    v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
                    v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
                    v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
                    v.uv0 = vi[0].uv0*bary.x + vi[1].uv0*bary.y + vi[2].uv0*bary.z;
                    displacement(v);
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            fixed4 frag(VertexOutput i) : COLOR {
                float2 node_3956 = i.uv0;
                clip(tex2D(_MASK,TRANSFORM_TEX(node_3956.rg, _MASK)).r - 0.5);
                i.normalDir = normalize(i.normalDir);
                SHADOW_COLLECTOR_FRAGMENT(i)
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Cull Off
            Offset 1, 1
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 5.0
            #ifndef LIGHTMAP_OFF
                // sampler2D unity_Lightmap;
                // float4 unity_LightmapST;
                #ifndef DIRLIGHTMAP_OFF
                    // sampler2D unity_LightmapInd;
                #endif
            #endif
            uniform sampler2D _MASK; uniform float4 _MASK_ST;
            uniform float _TileDIFNRM;
            uniform float _TESSAmound;
            uniform float _TESHight;
            uniform sampler2D _TES; uniform float4 _TES_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float4 uv0 : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float4 uv0 : TEXCOORD0;
                };
                struct OutputPatchConstant {
                    float edge[3]         : SV_TessFactor;
                    float inside          : SV_InsideTessFactor;
                    float3 vTangent[4]    : TANGENT;
                    float2 vUV[4]         : TEXCOORD;
                    float3 vTanUCorner[4] : TANUCORNER;
                    float3 vTanVCorner[4] : TANVCORNER;
                    float4 vCWts          : TANWEIGHTS;
                };
                TessVertex tessvert (VertexInput v) {
                    TessVertex o;
                    o.vertex = v.vertex;
                    o.normal = v.normal;
                    o.tangent = v.tangent;
                    o.uv0 = v.uv0;
                    return o;
                }
                void displacement (inout VertexInput v){
                    float2 node_35 = (v.uv0.rg*_TileDIFNRM);
                    float node_86 = (dot(tex2Dlod(_TES,float4(TRANSFORM_TEX(node_35, _TES),0.0,0)).rgb,float3(0.3,0.59,0.11))*0.5);
                    float node_114 = pow(node_86,1.0);
                    v.vertex.xyz +=  (((node_86+pow(node_114,1.0)+node_114)*_TESHight)*v.normal);
                }
                float Tessellation(TessVertex v){
                    return _TESSAmound;
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o;
                    float ts = Tessellation( v[0] );
                    o.edge[0] = ts;
                    o.edge[1] = ts;
                    o.edge[2] = ts;
                    o.inside = ts;
                    return o;
                }
                [domain("tri")]
                [partitioning("fractional_odd")]
                [outputtopology("triangle_cw")]
                [patchconstantfunc("hullconst")]
                [outputcontrolpoints(3)]
                TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
                    return v[id];
                }
                [domain("tri")]
                VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
                    VertexInput v;
                    v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
                    v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
                    v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
                    v.uv0 = vi[0].uv0*bary.x + vi[1].uv0*bary.y + vi[2].uv0*bary.z;
                    displacement(v);
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            fixed4 frag(VertexOutput i) : COLOR {
                float2 node_3957 = i.uv0;
                clip(tex2D(_MASK,TRANSFORM_TEX(node_3957.rg, _MASK)).r - 0.5);
                i.normalDir = normalize(i.normalDir);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
