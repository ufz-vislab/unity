using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using FullInspector;

public class MeshInfo : BaseScriptableObject
{
	[JsonObject(MemberSerialization.OptIn)]
	public struct PropertyDictionaries
	{
		[JsonProperty]
		public int SubMeshIndex;

		[JsonProperty]
		public Dictionary<string, bool> Bools;

		[JsonProperty]
		public Dictionary<string, float> Floats;

		[JsonProperty]
		public Dictionary<string, Color> Colors;
	}

	public List<PropertyDictionaries> Properties = new List<PropertyDictionaries>();

	/// <returns>false if not found.</returns>
	public bool GetBool(string name, int subMeshIndex)
	{
		if(HasBool(name, subMeshIndex))
			return Properties[subMeshIndex].Bools[name];
		else
			return false;
	}

	public bool HasBool(string name, int subMeshIndex)
	{
		if(Properties == null)
			return false;
		if(subMeshIndex >= Properties.Count)
			return false;
		if(Properties[subMeshIndex].Bools.ContainsKey(name))
			return true;
		else
			return false;
	}

	/// <returns>-9999 if not found.</returns>
	public float GetFloat(string name, int subMeshIndex)
	{
		if(HasFloat(name, subMeshIndex))
			return Properties[subMeshIndex].Floats[name];
		else
			return -9999f;
	}

	public bool HasFloat(string name, int subMeshIndex)
	{
		if(Properties == null)
			return false;
		if(subMeshIndex >= Properties.Count)
			return false;
		if(Properties[subMeshIndex].Floats.ContainsKey(name))
			return true;
		else
			return false;
	}

	/// <returns>Pink if not found.</returns>
	public Color GetColor(string name, int subMeshIndex)
	{
		if(HasColor(name, subMeshIndex))
			return Properties[subMeshIndex].Colors[name];
		else
			return Color.magenta;
	}

	public bool HasColor(string name, int subMeshIndex)
	{
		if(Properties == null)
			return false;
		if(subMeshIndex >= Properties.Count)
			return false;
		if(Properties[subMeshIndex].Colors.ContainsKey(name))
			return true;
		else
			return false;
	}
}
