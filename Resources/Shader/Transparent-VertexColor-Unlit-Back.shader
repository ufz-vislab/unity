Shader "UFZ/Transparent-VertexColor-Unlit-Back" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags {"RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True"}
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Front
		LOD 200

		CGPROGRAM
		#pragma surface surf Unlit alpha vertex:vert
		half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten) {
			half4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}

		fixed4 _Color;

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
			o.Alpha = _Color.a;
		}
		ENDCG
	}
	FallBack "UFZ/Opaque-VertexColor-Unlit-Back"
}
