Shader "DX11/VtkPoints" {
	SubShader {
		Pass {

			CGPROGRAM
			#pragma target 5.0

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			StructuredBuffer<float3> buf_Points;
			float4x4 _MATRIX_M; // model-matrix

			struct ps_input {
				float4 pos : SV_POSITION;
			};

			ps_input vert(uint id : SV_VertexID)
			{
				ps_input o;
				float3 worldPos = buf_Points[id];

				o.pos = mul(mul(UNITY_MATRIX_VP, _MATRIX_M), float4(worldPos, 1.0f));
				return o;
			}

			float4 frag(ps_input i) : COLOR
			{
				return float4(1.0f, 0.5f, 0.0f, 1.0f);
			}

			ENDCG

		}
	}

	Fallback Off
}
