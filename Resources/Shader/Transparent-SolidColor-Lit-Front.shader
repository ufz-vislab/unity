Shader "UFZ/Transparent-SolidColor-Lit-Front" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags {"RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True"}
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert alpha

		float4 _Color;

		struct Input {
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			half4 c = _Color;
			o.Albedo = c.rgb * _Color.a;
			o.Alpha = _Color.a;
		}
		ENDCG
	}
	FallBack "UFZ/Opaque-SolidColor-Lit-Front"
}
