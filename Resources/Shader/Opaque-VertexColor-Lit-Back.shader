Shader "UFZ/Opaque-VertexColor-Lit-Back" {
	SubShader {
		Tags { "RenderType"="Opaque" }
		Cull Front
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert

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
