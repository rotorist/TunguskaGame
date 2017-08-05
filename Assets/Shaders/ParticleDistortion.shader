// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Particles/Distortion"
{
    Properties{
        _TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
        _GlowPower("GlowPower", Range(1, 10)) = 1
        _EmisCol("Emisison Color", Color) = (0.0, 0.0, 0.0, 0.0)
        _MainTex("Base (RGB)", 2D) = "white" {}
    _Distortion("Distortion", Range(0,128)) = 10 //The range can be whatever you want, not sure why I put 128, but yea.
        _BumpMap("Normalmap", 2D) = "bump" {}
    _InvFade("Soft Particles Factor", Range(0.01,3.0)) = 1.0
    }
 
        Category{
        Tags{ "Queue" = "Overlay" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGBA
        Cull Off Lighting Off ZWrite Off Fog{ Color(0,0,0,0) }
        BindChannels{
        Bind "Color", color
        Bind "Vertex", vertex
        Bind "TexCoord", texcoord
    }
 
        SubShader{
        // This pass grabs the screen behind the object into a texture.
        // We can access the result in the next pass as _GrabTexture
        GrabPass{}
 
        // Main pass: Take the texture grabbed above and use the bumpmap to perturb it
        // on to the screen
        Pass{
 
        CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#pragma multi_compile_particles
 
#include "UnityCG.cginc"
 
        sampler2D _MainTex;
    fixed4 _TintColor;
    fixed4 _EmisCol;
    float _Distortion;
    struct appdata_t {
        float4 vertex : POSITION;
        float4 color : COLOR;
        float2 texcoord : TEXCOORD0;
 
    };
 
    struct v2f {
        float4 vertex : POSITION;
        float4 color : COLOR;
        float4 uvgrab : TEXCOORD0; //UV for the GrabPass{} texture
        float2 uvbump : TEXCOORD1;
        float2 uvmain : TEXCOORD2;
#ifdef SOFTPARTICLES_ON
        float4 projPos : TEXCOORD3;
#endif
    };
 
    float4 _MainTex_ST;
    float4 _BumpMap_ST;
    float _GlowPower;
    v2f vert(appdata_t v)
    {
        v2f o;
#if UNITY_UV_STARTS_AT_TOP
        float scale = -1.0;
#else
        float scale = 1.0;
#endif
        o.vertex = UnityObjectToClipPos(v.vertex);
#ifdef SOFTPARTICLES_ON
        o.projPos = ComputeScreenPos(o.vertex);
        COMPUTE_EYEDEPTH(o.projPos.z);
#endif
        o.color = v.color;
        o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
        o.uvgrab.zw = o.vertex.zw;
        o.uvbump = TRANSFORM_TEX(v.texcoord, _BumpMap);
        o.uvmain = TRANSFORM_TEX(v.texcoord, _MainTex);
        return o;
    }
 
    //Only necesary if you plan on having soft particles and intersection fading
    sampler2D _CameraDepthTexture;
    float _InvFade;
 
    //This is a reference to the GrabPass{} texture used earlier
    sampler2D _GrabTexture;
    float4 _GrabTexture_TexelSize;
 
    sampler2D _BumpMap;
    fixed4 frag(v2f i) : COLOR
    {
 
#ifdef SOFTPARTICLES_ON
        float sceneZ = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
    float partZ = i.projPos.z;
    float fade = saturate(_InvFade * (sceneZ - partZ));
    i.color.a *= fade;
#endif
 
    half4 tint = tex2D(_MainTex, i.uvmain);
    half2 bump = UnpackNormal(tex2D(_BumpMap, i.uvbump)).rg; // we could optimize this by just reading the x & y without reconstructing the Z
    float h2 = _Distortion * 100;
    float2 offset = (bump * h2 * _GrabTexture_TexelSize.xy) * (i.color.a * tint.a);
    i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;
 
    half4 col = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvgrab)); //col now represents the GrabPass{} texture
   
    col.rgb *= _GlowPower;
    col.rgb *= i.color.rgb;
    col.rgb += _EmisCol.rgb;
 
    col.rgb = abs(col.rgb); //Using absolute value in some of these calculations to avoid artifacts when glow power reaches higher levels
    fixed4 cRet = col;
    cRet.rgb *= (tint.rgb * i.color.rgb);
    cRet.a = col.a * i.color.a * tint.a;
    cRet.a *= _TintColor.a;
    cRet.a = abs(cRet.a);
    return  cRet;
    }
        ENDCG
    }
    }
    }
}
 