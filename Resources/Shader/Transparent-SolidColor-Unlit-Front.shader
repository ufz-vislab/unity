Shader "UFZ/Transparent-SolidColor-Unlit-Front" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
	}

	SubShader {
		Tags {"RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True"}
		LOD 200
		Color [_Color]
		Pass {
			Blend SrcAlpha OneMinusSrcAlpha
		}
	}
}
