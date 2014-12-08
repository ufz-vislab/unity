using UnityEngine;
using System.Collections;

public class MeshInfoVtkProperties : MeshInfoLoaderBase
{

	protected MeshInfo info;

	void Start ()
	{
		info = (MeshInfo)ScriptableObject;
	}

	public bool getUseVertexColors(int subMeshIndex)
	{
		return info.GetBool("UseVertexColors", subMeshIndex);
	}

	public float[] getRange(int subMeshIndex)
	{
		float[] range = new float[2];
		range[0] = info.GetFloat("ScalarRangeMin", subMeshIndex);
		range[1] = info.GetFloat("ScalarRangeMax", subMeshIndex);
		return range;
	}

	public Color[] getColorRange(int subMeshIndex)
	{
		Color[] color = new Color[2];
		color[0] = info.GetColor("ScalarRangeMinColor", subMeshIndex);
		color[1] = info.GetColor("ScalarRangeMaxColor", subMeshIndex);
		return color;
	}
}
