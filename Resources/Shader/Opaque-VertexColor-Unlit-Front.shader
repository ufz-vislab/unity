Shader "UFZ/Opaque-VertexColor-Unlit-Front" {
	Category {
		BindChannels {
			Bind "Color", color
			Bind "Vertex", vertex
		}
		SubShader { Pass { } }
	}
}
