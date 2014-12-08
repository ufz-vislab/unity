using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class TransformExtensions
{
	/// <summary/>
	/// <param name="skipSelf">Does not return the components in the object itself</param>
	public static T[] GetComponentsInChildren<T>
		(this GameObject go, bool includeInactive, bool skipSelf = false)
		where T : Component
	{
		var comps = go.GetComponentsInChildren<T>(includeInactive);
		if (!skipSelf)
			return comps;

		return comps.Where(comp => comp.gameObject.GetInstanceID() != go.GetInstanceID()).ToArray();
	}
}