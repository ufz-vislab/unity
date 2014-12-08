Shader "UFZ/Opaque-SolidColor-Unlit-Front" {
	Properties {
		_Color ("Color", Color) = (1,1,1)
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Color [_Color]
		Pass {}
	}
}
