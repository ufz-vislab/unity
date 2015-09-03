using System.Linq;
using UnityEngine;
using System.Reflection;

namespace UFZ.Tools.Extensions
{
	public static class GameObjectExtensions
	{
		public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
		{
			return go.AddComponent<T>().GetCopyOf(toAdd);
		}

		public static T GetCopyOf<T>(this Component comp, T other) where T : Component
		{
			var type = comp.GetType();
			if (type != other.GetType()) return null; // type mis-match
			const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
				BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
			var pinfos = type.GetProperties(flags);
			foreach (var pinfo in pinfos.Where(pinfo => pinfo.CanWrite))
			{
				try
				{
					pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
				}
				catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
			}
			var finfos = type.GetFields(flags);
			foreach (var finfo in finfos)
				finfo.SetValue(comp, finfo.GetValue(other));

			return comp as T;
		}
	}
}
