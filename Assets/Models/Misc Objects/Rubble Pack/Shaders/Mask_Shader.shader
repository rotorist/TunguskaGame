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
/*SF_DATA;ver:0.25;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:False,lmpd:True,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,hqsc:True,hqlp:False,blpr:0,bsrc:0,bdst:1,culm:0,dpts:2,wrdp:True,ufog:True,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0;n:type:ShaderForge.SFN_Final,id:1,x:32719,y:32713|diff-3760-OUT,spec-63-OUT,gloss-69-OUT,normal-48-OUT,emission-66-OUT,amspl-3909-OUT,clip-4-R;n:type:ShaderForge.SFN_Tex2d,id:2,x:34269,y:32582,ptlb:DIF,tex:e40d535b0adf50f4ca15d5ad7ef6bb8e,ntxv:0,isnm:False|UVIN-35-OUT;n:type:ShaderForge.SFN_Tex2d,id:3,x:33377,y:33400,ptlb:NRM,tex:c2b17506b5081f24b82e4d9f4ede6502,ntxv:3,isnm:True|UVIN-35-OUT;n:type:ShaderForge.SFN_Tex2d,id:4,x:32747,y:33156,ptlb:MASK,tex:52a252d371220834b9d8321353afe9b0,ntxv:0,isnm:False;n:type:ShaderForge.SFN_TexCoord,id:34,x:34362,y:32976,uv:0;n:type:ShaderForge.SFN_Multiply,id:35,x:34164,y:33024|A-34-UVOUT,B-36-OUT;n:type:ShaderForge.SFN_ValueProperty,id:36,x:34362,y:33155,ptlb:Tile_DIF/NRM,v1:1;n:type:ShaderForge.SFN_Tex2d,id:40,x:33642,y:33874,ptlb:NRM_DETAIL,tex:5e2ba7ad893425f4ca5e546f0a779195,ntxv:3,isnm:True|UVIN-44-OUT;n:type:ShaderForge.SFN_TexCoord,id:42,x:34083,y:33753,uv:0;n:type:ShaderForge.SFN_Multiply,id:44,x:33887,y:33874|A-42-UVOUT,B-46-OUT;n:type:ShaderForge.SFN_ValueProperty,id:46,x:34085,y:33985,ptlb:Tile_NRM_DETAIL,v1:5;n:type:ShaderForge.SFN_ComponentMask,id:47,x:33438,y:33887,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-40-RGB;n:type:ShaderForge.SFN_Add,id:48,x:33126,y:33417|A-3-RGB,B-47-OUT;n:type:ShaderForge.SFN_Desaturate,id:57,x:34008,y:32646|COL-2-R;n:type:ShaderForge.SFN_Multiply,id:58,x:33764,y:32672|A-57-OUT,B-60-OUT;n:type:ShaderForge.SFN_ValueProperty,id:60,x:34008,y:32807,ptlb:SPC,v1:1;n:type:ShaderForge.SFN_Multiply,id:61,x:33412,y:33020|A-57-OUT,B-62-OUT;n:type:ShaderForge.SFN_ValueProperty,id:62,x:33620,y:33067,ptlb:EM,v1:0;n:type:ShaderForge.SFN_Multiply,id:63,x:33566,y:32751|A-58-OUT,B-64-RGB;n:type:ShaderForge.SFN_Color,id:64,x:33766,y:32823,ptlb:Color_SPC,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:65,x:33412,y:33183,ptlb:Color_EM,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:66,x:33150,y:32999|A-61-OUT,B-65-RGB;n:type:ShaderForge.SFN_Slider,id:69,x:32693,y:32578,ptlb:Gloss,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Tex2dAsset,id:87,x:34773,y:31928,ptlb:Detail_Map,tex:23e06d0bc06d18149a9ae3d573baa452;n:type:ShaderForge.SFN_TexCoord,id:3669,x:34944,y:31796,uv:0;n:type:ShaderForge.SFN_Multiply,id:3671,x:34760,y:31781|A-3669-UVOUT,B-3673-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3673,x:34944,y:31960,ptlb:Detail_Tile,v1:1;n:type:ShaderForge.SFN_Tex2d,id:3675,x:34481,y:31757,tex:23e06d0bc06d18149a9ae3d573baa452,ntxv:0,isnm:False|UVIN-3671-OUT,TEX-87-TEX;n:type:ShaderForge.SFN_Power,id:3677,x:34139,y:31722|VAL-3725-OUT,EXP-3858-OUT;n:type:ShaderForge.SFN_Multiply,id:3678,x:33958,y:31734|A-3677-OUT,B-3677-OUT,C-3781-OUT;n:type:ShaderForge.SFN_Multiply,id:3684,x:33616,y:32219|A-3678-OUT,B-2-RGB;n:type:ShaderForge.SFN_Desaturate,id:3725,x:34301,y:31696|COL-3675-RGB,DES-3805-OUT;n:type:ShaderForge.SFN_If,id:3760,x:33277,y:32469|A-3791-OUT,B-3762-OUT,GT-3684-OUT,EQ-2-RGB,LT-2-RGB;n:type:ShaderForge.SFN_Vector1,id:3762,x:33477,y:32469,v1:0.5;n:type:ShaderForge.SFN_ValueProperty,id:3781,x:34128,y:31902,ptlb:Detail_Power,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:3791,x:33362,y:32343,ptlb:Detail_On/Off(0=OFF/1=ON),v1:0;n:type:ShaderForge.SFN_Slider,id:3805,x:34158,y:31581,ptlb:Detail_Desaturate,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Slider,id:3858,x:34327,y:31911,ptlb:Detail_Brightness,min:0,cur:1,max:3;n:type:ShaderForge.SFN_Cubemap,id:3877,x:33370,y:32688,ptlb:SAL_CubeMap;n:type:ShaderForge.SFN_Vector1,id:3900,x:33339,y:32840,v1:0;n:type:ShaderForge.SFN_If,id:3909,x:33119,y:32737|A-3913-OUT,B-3911-OUT,GT-3877-RGB,EQ-3877-RGB,LT-3900-OUT;n:type:ShaderForge.SFN_Vector1,id:3911,x:33182,y:32666,v1:0.5;n:type:ShaderForge.SFN_ValueProperty,id:3913,x:33204,y:32611,ptlb:SAL_On/Off(0=OFF/1=ON),v1:0;proporder:4-2-3-36-40-46-69-60-64-3913-3877-62-65-87-3791-3673-3781-3858-3805;pass:END;sub:END;*/

Shader "Crowsfield/Mask_Shader" {
    Properties {
        _MASK ("MASK", 2D) = "white" {}
        _DIF ("DIF", 2D) = "white" {}
        _NRM ("NRM", 2D) = "bump" {}
        _TileDIFNRM ("Tile_DIF/NRM", Float ) = 1
        _NRMDETAIL ("NRM_DETAIL", 2D) = "bump" {}
        _TileNRMDETAIL ("Tile_NRM_DETAIL", Float ) = 5
        _Gloss ("Gloss", Range(0, 1)) = 0.5
        _SPC ("SPC", Float ) = 1
        _ColorSPC ("Color_SPC", Color) = (1,1,1,1)
        _SALOnOff0OFF1ON ("SAL_On/Off(0=OFF/1=ON)", Float ) = 0
        _SALCubeMap ("SAL_CubeMap", Cube) = "_Skybox" {}
        _EM ("EM", Float ) = 0
        _ColorEM ("Color_EM", Color) = (1,1,1,1)
        _DetailMap ("Detail_Map", 2D) = "white" {}
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
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
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
            uniform sampler2D _DetailMap; uniform float4 _DetailMap_ST;
            uniform float _DetailTile;
            uniform float _DetailPower;
            uniform float _DetailOnOff0OFF1ON;
            uniform float _DetailDesaturate;
            uniform float _DetailBrightness;
            uniform samplerCUBE _SALCubeMap;
            uniform float _SALOnOff0OFF1ON;
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
            fixed4 frag(VertexOutput i) : COLOR {
                float2 node_3929 = i.uv0;
                clip(tex2D(_MASK,TRANSFORM_TEX(node_3929.rg, _MASK)).r - 0.5);
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
                float node_57 = dot(node_2.r,float3(0.3,0.59,0.11));
                float3 emissive = ((node_57*_EM)*_ColorEM.rgb);
///////// Gloss:
                float gloss = exp2(_Gloss*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float node_3909_if_leA = step(_SALOnOff0OFF1ON,0.5);
                float node_3909_if_leB = step(0.5,_SALOnOff0OFF1ON);
                float4 node_3877 = texCUBE(_SALCubeMap,viewReflectDirection);
                float3 specularColor = ((node_57*_SPC)*_ColorSPC.rgb);
                float3 specularAmb = lerp((node_3909_if_leA*0.0)+(node_3909_if_leB*node_3877.rgb),node_3877.rgb,node_3909_if_leA*node_3909_if_leB) * specularColor;
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor + specularAmb;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float node_3760_if_leA = step(_DetailOnOff0OFF1ON,0.5);
                float node_3760_if_leB = step(0.5,_DetailOnOff0OFF1ON);
                float2 node_3671 = (i.uv0.rg*_DetailTile);
                float3 node_3677 = pow(lerp(tex2D(_DetailMap,TRANSFORM_TEX(node_3671, _DetailMap)).rgb,dot(tex2D(_DetailMap,TRANSFORM_TEX(node_3671, _DetailMap)).rgb,float3(0.3,0.59,0.11)),_DetailDesaturate),_DetailBrightness);
                finalColor += diffuseLight * lerp((node_3760_if_leA*node_2.rgb)+(node_3760_if_leB*((node_3677*node_3677*_DetailPower)*node_2.rgb)),node_2.rgb,node_3760_if_leA*node_3760_if_leB);
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
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
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
            uniform sampler2D _DetailMap; uniform float4 _DetailMap_ST;
            uniform float _DetailTile;
            uniform float _DetailPower;
            uniform float _DetailOnOff0OFF1ON;
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
            fixed4 frag(VertexOutput i) : COLOR {
                float2 node_3930 = i.uv0;
                clip(tex2D(_MASK,TRANSFORM_TEX(node_3930.rg, _MASK)).r - 0.5);
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
                float node_57 = dot(node_2.r,float3(0.3,0.59,0.11));
                float3 specularColor = ((node_57*_SPC)*_ColorSPC.rgb);
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float node_3760_if_leA = step(_DetailOnOff0OFF1ON,0.5);
                float node_3760_if_leB = step(0.5,_DetailOnOff0OFF1ON);
                float2 node_3671 = (i.uv0.rg*_DetailTile);
                float3 node_3677 = pow(lerp(tex2D(_DetailMap,TRANSFORM_TEX(node_3671, _DetailMap)).rgb,dot(tex2D(_DetailMap,TRANSFORM_TEX(node_3671, _DetailMap)).rgb,float3(0.3,0.59,0.11)),_DetailDesaturate),_DetailBrightness);
                finalColor += diffuseLight * lerp((node_3760_if_leA*node_2.rgb)+(node_3760_if_leB*((node_3677*node_3677*_DetailPower)*node_2.rgb)),node_2.rgb,node_3760_if_leA*node_3760_if_leB);
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
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCOLLECTOR
            #define SHADOW_COLLECTOR_PASS
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcollector
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            #ifndef LIGHTMAP_OFF
                // sampler2D unity_Lightmap;
                // float4 unity_LightmapST;
                #ifndef DIRLIGHTMAP_OFF
                    // sampler2D unity_LightmapInd;
                #endif
            #endif
            uniform sampler2D _MASK; uniform float4 _MASK_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_COLLECTOR;
                float4 uv0 : TEXCOORD5;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_SHADOW_COLLECTOR(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float2 node_3931 = i.uv0;
                clip(tex2D(_MASK,TRANSFORM_TEX(node_3931.rg, _MASK)).r - 0.5);
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
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            #ifndef LIGHTMAP_OFF
                // sampler2D unity_Lightmap;
                // float4 unity_LightmapST;
                #ifndef DIRLIGHTMAP_OFF
                    // sampler2D unity_LightmapInd;
                #endif
            #endif
            uniform sampler2D _MASK; uniform float4 _MASK_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float4 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float2 node_3932 = i.uv0;
                clip(tex2D(_MASK,TRANSFORM_TEX(node_3932.rg, _MASK)).r - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
