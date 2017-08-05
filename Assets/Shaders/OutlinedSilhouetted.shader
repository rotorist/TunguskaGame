// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Outlined/Silhouetted Diffuse" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,0.5)
        _SpecColor ("Spec Color", Color) = (1,1,1,1)
        _Emission ("Emmisive Color", Color) = (0,0,0,0)
        _Shininess ("Shininess", Range (0.01, 1)) = 0.7
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
    }
CGINCLUDE
#include "UnityCG.cginc"
struct appdata {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
};
struct v2f {
    float4 pos : POSITION;
    float4 color : COLOR;
};
//uniform float _Outline;
uniform float4 _OutlineColor;
v2f vert(appdata v) {
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.color = _OutlineColor;
    return o;
}
ENDCG
    SubShader {
        Tags { "Queue" = "Transparent" }
        // note that a vertex shader is specified here but its using the one above
        Pass {
            Name "OUTLINE"
            Tags { "LightMode" = "Always" }
            Cull Off
            ZWrite Off
            ZTest Greater
            ColorMask RGB // alpha not used
            // you can choose what kind of blending mode you want for the outline
            //Blend SrcAlpha OneMinusSrcAlpha // Normal
            Blend One One // Additive
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
half4 frag(v2f i) :COLOR {
    return i.color;
}
ENDCG
        }
        Pass {
       
            ZWrite On
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha
            Offset -50,-50
           
            Material {
                Diffuse [_Color]
                Ambient [_Color]
                Shininess [_Shininess]
                Specular [_SpecColor]
                Emission [_Emission]
            }
            Lighting On
            SeparateSpecular On
            SetTexture [_MainTex] {
                constantColor [_Color]
                Combine texture * primary DOUBLE, texture * constant
            }
        }
    }
    Fallback "Diffuse"
}