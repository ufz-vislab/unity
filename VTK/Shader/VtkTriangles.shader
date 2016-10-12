// TODO: only one light
Shader "DX11/VtkTriangles" {
	SubShader{
		Tags { "RenderType" = "Opaque" }

		CGPROGRAM
		#pragma target 5.0
		#pragma only_renderers d3d11
		// #pragma surface surf Lambert vertex:vert addshadow nolightmap
		// Quickfix by using no lighting for wrong normals
		#pragma surface surf NoLighting vertex:vert addshadow nolightmap
		
		#ifdef SHADER_API_D3D11
		StructuredBuffer<float3> buf_Points;
		StructuredBuffer<int> buf_Indices;
		StructuredBuffer<float3> buf_Normals;
		StructuredBuffer<float3> buf_Colors;
		#endif
		float4x4 _MATRIX_M; // model-matrix
		
		struct appdata {
			float3 normal    : NORMAL;
			float4 tangent   : TANGENT;
			float4 vertex    : POSITION;
			float4 color     : COLOR;
			uint id          : SV_VertexID;
		};
		
		struct Input {
			float4 color : Color;
		};
		
		void vert(inout appdata v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
		
			uint index = buf_Indices[v.id];
			v.vertex = mul(_MATRIX_M,float4(buf_Points[index], 1.0));
			v.normal = buf_Normals[index]; // float3(0, 0, 1);
			v.color = float4(buf_Colors[index], 1.0);
		}
		
		void surf(Input IN, inout SurfaceOutput o)
		{
			o.Albedo = IN.color.rgb;
			o.Specular = 0.2;
			o.Gloss = 1.0;
			o.Alpha = IN.color.a;
		}

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			fixed4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}
		
		ENDCG
	}
	Fallback Off
}
