using System.Collections.Generic;
using UnityEngine;
using FullInspector;

namespace UFZ.Annotations
{
	/// <summary>
	/// Generic annotation class storing property dictionaries.
	/// </summary>
	public class MeshInfo : BaseScriptableObject
	{
		/// <summary>
		/// Data storage for the property dictionaries.
		/// </summary>
		public struct PropertyDictionaries
		{
			/// <summary>
			/// The sub mesh index.
			/// </summary>
			public int SubMeshIndex;

			/// <summary>
			/// Boolean properties.
			/// </summary>
			public Dictionary<string, bool> Bools;

			/// <summary>
			/// Float properties.
			/// </summary>
			public Dictionary<string, float> Floats;

			/// <summary>
			/// Color properties.
			/// </summary>
			public Dictionary<string, Color> Colors;
		}

		/// <summary>
		/// The properties.
		/// </summary>
		public List<PropertyDictionaries> Properties = new List<PropertyDictionaries>();

		/// <summary>
		/// Gets a boolean property
		/// </summary>
		/// <param name="boolName">Name of the bool property.</param>
		/// <param name="subMeshIndex">Index of the sub mesh.</param>
		/// <returns>
		/// false if not found.
		/// </returns>
		public bool GetBool(string boolName, int subMeshIndex)
		{
			return HasBool(boolName, subMeshIndex) && Properties[subMeshIndex].Bools[boolName];
		}

		/// <summary>
		/// Determines whether a bool property with the specified name exists.
		/// </summary>
		/// <param name="boolName">Name of the bool.</param>
		/// <param name="subMeshIndex">Index of the sub mesh.</param>
		/// <returns></returns>
		public bool HasBool(string boolName, int subMeshIndex)
		{
			if (Properties == null)
				return false;
			if (subMeshIndex >= Properties.Count)
				return false;
			return Properties[subMeshIndex].Bools.ContainsKey(boolName);
		}

		/// <summary>
		/// Gets a float property.
		/// </summary>
		/// <param name="floatName">Name of the float property.</param>
		/// <param name="subMeshIndex">Index of the sub mesh.</param>
		/// <returns>
		/// -9999 if not found.
		/// </returns>
		public float GetFloat(string floatName, int subMeshIndex)
		{
			if (HasFloat(floatName, subMeshIndex))
				return Properties[subMeshIndex].Floats[floatName];
			return -9999f;
		}

		/// <summary>
		/// Determines whether a float property with the specified name exists.
		/// </summary>
		/// <param name="floatName">Name of the float.</param>
		/// <param name="subMeshIndex">Index of the sub mesh.</param>
		/// <returns></returns>
		public bool HasFloat(string floatName, int subMeshIndex)
		{
			if (Properties == null)
				return false;
			if (subMeshIndex >= Properties.Count)
				return false;
			return Properties[subMeshIndex].Floats.ContainsKey(floatName);
		}

		/// <summary>
		/// Gets a color property.
		/// </summary>
		/// <param name="colorName">Name of the color property.</param>
		/// <param name="subMeshIndex">Index of the sub mesh.</param>
		/// <returns>
		/// Pink if not found.
		/// </returns>
		public Color GetColor(string colorName, int subMeshIndex)
		{
			return HasColor(colorName, subMeshIndex) ?
				Properties[subMeshIndex].Colors[colorName] : Color.magenta;
		}

		/// <summary>
		/// Determines whether a color property with the specified name exists.
		/// </summary>
		/// <param name="colorName">Name of the color.</param>
		/// <param name="subMeshIndex">Index of the sub mesh.</param>
		/// <returns></returns>
		public bool HasColor(string colorName, int subMeshIndex)
		{
			if (Properties == null)
				return false;
			if (subMeshIndex >= Properties.Count)
				return false;
			return Properties[subMeshIndex].Colors.ContainsKey(colorName);
		}
	}
}
