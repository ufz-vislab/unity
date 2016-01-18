Shader "UFZ/Opaque-VertexColor-Unlit-Back" {
	SubShader {
		Tags { "RenderType"="Opaque" }
		Cull Front
		LOD 200

		CGPROGRAM
		#pragma surface surf Unlit vertex:vert
		half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten) {
			half4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}

		struct Input {
			float3 vertColor;
		};

		void vert (inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			v.normal = -v.normal;
			o.vertColor = v.color;
		}

		void surf(Input IN, inout SurfaceOutput o) {
			o.Albedo = IN.vertColor;
			o.Alpha = 1;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
