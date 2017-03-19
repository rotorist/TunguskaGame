Shader "FX/TransparentDiffuseWithShadows" {
Properties{
  _TintColor("Main Color", Color) = (1,1,1,1)
  _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
}
SubShader{
Tags{ "Queue" = "AlphaTest+50" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
LOD 200
CGPROGRAM
#pragma surface surf Lambert alpha:fade
sampler2D _MainTex;
 
fixed4 _TintColor;
struct Input {
   float2 uv_MainTex;
};
void surf(Input IN, inout SurfaceOutput o) {
  fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _TintColor;
  o.Albedo = c.rgb;
  o.Alpha = c.a;
}
ENDCG
}
  Fallback "Legacy Shaders/Transparent/VertexLit"
}