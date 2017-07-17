using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace UFZ.Interaction
{
	public class GroupObjectVisibility : SerializedMonoBehaviour
	{
		public Dictionary<string, ObjectVisibility[] > ObjectVisibilities;

		public void Execute(string groupName)
		{
			if (!ObjectVisibilities.ContainsKey(groupName))
			{
				Debug.LogWarning("There is no ObjectVisibility with the name " +
					groupName + " on GameObject " + name + "!");
				return;
			}

			var objectVisibilities = ObjectVisibilities[groupName];
			foreach (var objectVisibility in objectVisibilities)
				objectVisibility.Do();
		}
	}
}
