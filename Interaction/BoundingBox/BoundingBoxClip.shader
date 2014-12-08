Shader "UFZ/Bounding Box Clip"
{
	Properties
	{
		minima ("Min clipping planes", Vector) = (0,0,0)
		maxima ("Max clipping planes", Vector) = (1,1,1)
		cutout_pos ("Octant cutout position", Vector) = (0.5, 0.5, 0.5)
		cutout_octant ("Octant to cut out", Vector) = (1.0, 1.0, 1.0)

		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}

	}
	SubShader
	{
		Pass
		{
			CULL BACK

CGPROGRAM
#pragma exclude_renderers gles d3d11 xbox360
#pragma vertex vertex_shader
#pragma fragment fragment_shader

#include "UnityCG.cginc"

uniform float3 minima;
uniform float3 maxima;
uniform float3 cutout_pos;
uniform float3 cutout_octant;
uniform sampler2D _MainTex;

float4x4 rotate(float3 r)
{
	float3 c, s;
	sincos(r.x, s.x, c.x);
	sincos(r.y, s.y, c.y);
	sincos(r.z, s.z, c.z);
	return float4x4( c.y*c.z,    -s.z,     s.y, 0,
					     s.z, c.x*c.z,    -s.x, 0,
					    -s.y,     s.x, c.x*c.y, 0,
					       0,       0,       0, 1 );
}

struct a2v
{
	float4 vertex   : POSITION;
	float4 color    : COLOR;
	float2 texcoord : TEXCOORD;
	float3 normal   : NORMAL;
};

struct v2f
{
	float4 position : POSITION;
	float4 color    : COLOR;
	float2 texcoord : TEXCOORD0;
	float3 normal   : TEXCOORD1;
	float4 vertex   : TEXCOORD2;
};

v2f vertex_shader( a2v IN )
{
	v2f OUT;

	OUT.position = mul(UNITY_MATRIX_MVP, IN.vertex);
	OUT.texcoord = IN.texcoord;
	OUT.normal   = IN.normal;
	OUT.vertex   = IN.vertex;
	OUT.color    = IN.color;

	return OUT;
}

void fragment_shader( v2f IN,
					  out float4 finalcolor : COLOR)
{
	// Bounding box
	if(! all(IN.vertex > minima))
		discard;
	if(! all(IN.vertex < maxima))
		discard;

	// Octant
	bool3 octant = bool3(cutout_octant);
	bool3 blub = IN.vertex >= cutout_pos;
	if(all(blub == octant))
		discard;

	float3 N = IN.normal;

	N = mul(UNITY_MATRIX_IT_MV, float4(N, 1));
	float diffuse = saturate(dot(glstate.light[0].position, N));

	finalcolor = IN.color;
	finalcolor.xyz = tex2D(_MainTex, IN.texcoord).xyz *(diffuse *0.6 +0.4) * finalcolor.xyz;
}
ENDCG

		}
	}  Fallback "Diffuse"
}