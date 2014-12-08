Shader "UFZ/Transparent-VertexColor-Lit-Front" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags {"RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True"}
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert alpha vertex:vert

		fixed4 _Color;

		struct Input {
			float3 vertColor;
		};

		void vert (inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.vertColor = v.color;
		}

		void surf(Input IN, inout SurfaceOutput o) {
			o.Albedo = IN.vertColor;
			o.Alpha = _Color.a;
		}
		ENDCG
	}
	FallBack "UFZ/Opaque-VertexColor-Lit-Front"
}
