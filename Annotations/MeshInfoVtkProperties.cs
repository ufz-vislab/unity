using UnityEngine;

namespace UFZ.Annotations
{
	/// <summary>
	/// Specific VTK properties exported by ParaView / DataExplorer.
	/// </summary>
	public class MeshInfoVtkProperties : MeshInfoLoaderBase
	{
		protected MeshInfo Info;

		private void Start()
		{
			Info = ScriptableObject;
		}

		/// <summary>
		/// Are there scalars stored in the vertex colors.
		/// </summary>
		/// <param name="subMeshIndex">Index of the sub mesh.</param>
		/// <returns>True if scalars were exported.</returns>
		public bool GetUseVertexColors(int subMeshIndex)
		{
			return Info.GetBool("UseVertexColors", subMeshIndex);
		}

		/// <summary>
		/// Gets the scalar range.
		/// </summary>
		/// <param name="subMeshIndex">Index of the sub mesh.</param>
		public float[] GetRange(int subMeshIndex)
		{
			var range = new float[2];
			range[0] = Info.GetFloat("ScalarRangeMin", subMeshIndex);
			range[1] = Info.GetFloat("ScalarRangeMax", subMeshIndex);
			return range;
		}

		/// <summary>
		/// Gets the scalar color range.
		/// </summary>
		/// <param name="subMeshIndex">Index of the sub mesh.</param>
		public Color[] GetColorRange(int subMeshIndex)
		{
			var color = new Color[2];
			color[0] = Info.GetColor("ScalarRangeMinColor", subMeshIndex);
			color[1] = Info.GetColor("ScalarRangeMaxColor", subMeshIndex);
			return color;
		}
	}
}
