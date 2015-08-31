using UFZ.Interaction;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraLineRenderer : MonoBehaviour
{
	static Material lineMaterial = null;
	public List<BoundingBoxClip> clips = new List<BoundingBoxClip>();

	static void CreateLineMaterial()
	{
		if (!lineMaterial)
		{
			lineMaterial = new Material("Shader \"Lines/Colored Blended\" {" +
				"SubShader { Pass { " +
				//"    Blend SrcAlpha OneMinusSrcAlpha " +
				"    ZWrite Off Cull Off Fog { Mode Off } " +
				"    BindChannels {" +
				"      Bind \"vertex\", vertex Bind \"color\", color }" +
				"} } }");
			lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
		}
	}

	void OnPostRender()
	{
		CreateLineMaterial();
		lineMaterial.SetPass(0);

		foreach (BoundingBoxClip clip in clips)
		{
			GL.Begin(GL.LINES);
			GL.Color(new Color(1f, 1f, 1f, 1f));

			/*
			GL.Vertex (clip.Boxes [0].transform.position);
			GL.Vertex (clip.Boxes [1].transform.position);
			GL.Vertex (clip.Boxes [1].transform.position);
			GL.Vertex (clip.Boxes [2].transform.position);
			GL.Vertex (clip.Boxes [2].transform.position);
			GL.Vertex (clip.Boxes [3].transform.position);
			GL.Vertex (clip.Boxes [3].transform.position);
			GL.Vertex (clip.Boxes [0].transform.position);
			
			GL.Vertex (clip.Boxes [4].transform.position);
			GL.Vertex (clip.Boxes [5].transform.position);
			GL.Vertex (clip.Boxes [5].transform.position);
			GL.Vertex (clip.Boxes [6].transform.position);
			GL.Vertex (clip.Boxes [6].transform.position);
			GL.Vertex (clip.Boxes [7].transform.position);
			GL.Vertex (clip.Boxes [7].transform.position);
			GL.Vertex (clip.Boxes [4].transform.position);
			
			GL.Vertex (clip.Boxes [0].transform.position);
			GL.Vertex (clip.Boxes [4].transform.position);
			GL.Vertex (clip.Boxes [1].transform.position);
			GL.Vertex (clip.Boxes [5].transform.position);
			GL.Vertex (clip.Boxes [2].transform.position);
			GL.Vertex (clip.Boxes [6].transform.position);
			GL.Vertex (clip.Boxes [3].transform.position);
			GL.Vertex (clip.Boxes [7].transform.position);
			*/

			GL.End();
		}
	}
}
