using UnityEngine;
using System.Linq;

namespace UFZ.Tools.Extensions
{
	/// <summary>
	/// Add methods to Unitys Transform-component.
	/// </summary>
	/// <seealso cref="http://unitypatterns.com/extension-methods/"/>
	public static class TransformExtensions
	{
		/// <summary>
		/// Gets the components in children.
		/// </summary>
		/// <param name="includeInactive">Iterate over inactive components?</param>
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
}
