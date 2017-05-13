Shader "Bumped Colour Specular" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_SpecMap ("Specular Map", 2D) = "grey" {}
	}
	SubShader { 
		Tags { "RenderType"="Opaque" }
		LOD 400
		
		CGPROGRAM
			#pragma surface surf BlinnPhongColoured

			sampler2D _MainTex;
			sampler2D _BumpMap;
			sampler2D _SpecMap;
			fixed4 _Color;
			half _Shininess;

			struct SurfaceOutputColoured {
				fixed3 Albedo;
				fixed3 Normal;
				fixed3 Emission;
				fixed4 SpecColor;
				half Specular;
				fixed Gloss;
				fixed Alpha;
			};

			struct Input {
				float2 uv_MainTex;
				float2 uv_BumpMap;
				float2 uv_SpecMap;
			};

			void surf (Input IN, inout SurfaceOutputColoured o) {
				fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
				o.Albedo = tex.rgb * _Color.rgb;
				o.Gloss = tex.a;
				o.Alpha = tex.a * _Color.a;
				o.Specular = _Shininess;
				o.SpecColor = tex2D(_SpecMap, IN.uv_SpecMap);
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			}

			inline fixed4 LightingBlinnPhongColoured (SurfaceOutputColoured s, fixed3 lightDir, half3 viewDir, fixed atten)
			{
				half3 h = normalize (lightDir + viewDir);
				
				fixed diff = max (0, dot (s.Normal, lightDir));
				
				float nh = max (0, dot (s.Normal, h));
				float spec = pow (nh, s.Specular*128.0) * s.Gloss;
				
				fixed4 c;
				c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * _SpecColor.rgb * s.SpecColor.rgb * spec) * (atten * 2);
				c.a = s.Alpha + _LightColor0.a * _SpecColor.a * s.SpecColor.a * spec * atten;
				return c;
			}

			inline fixed4 LightingBlinnPhongColoured_PrePass (SurfaceOutputColoured s, half4 light)
			{
				fixed spec = light.a * s.Gloss;
				
				fixed4 c;
				c.rgb = (s.Albedo * light.rgb + light.rgb * _SpecColor.rgb * s.SpecColor.rgb * spec);
				c.a = s.Alpha + spec * _SpecColor.a * s.SpecColor.a;
				return c;
			}

			inline half4 LightingBlinnPhongColoured_DirLightmap (SurfaceOutputColoured s, fixed4 color, fixed4 scale, half3 viewDir, bool surfFuncWritesNormal, out half3 specColor)
			{
				UNITY_DIRBASIS
				half3 scalePerBasisVector;
				
				half3 lm = DirLightmapDiffuse (unity_DirBasis, color, scale, s.Normal, surfFuncWritesNormal, scalePerBasisVector);
				
				half3 lightDir = normalize (scalePerBasisVector.x * unity_DirBasis[0] + scalePerBasisVector.y * unity_DirBasis[1] + scalePerBasisVector.z * unity_DirBasis[2]);
				half3 h = normalize (lightDir + viewDir);

				float nh = max (0, dot (s.Normal, h));
				float spec = pow (nh, s.Specular * 128.0);
				
				// specColor used outside in the forward path, compiled out in prepass
				specColor = lm * _SpecColor.rgb * s.SpecColor.rgb * s.Gloss * spec;
				
				// spec from the alpha component is used to calculate specular
				// in the Lighting*_Prepass function, it's not used in forward
				return half4(lm, spec);
			}

		ENDCG
	}

	FallBack "Bumped Specular"
}