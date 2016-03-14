using UnityEngine;
using UnityEditor;
using System.Collections;

namespace UFZ
{
	[InitializeOnLoad]
	public static class IconStyles
	{
		public static Texture2D zoneIcon;

		static IconStyles ()
		{
			zoneIcon = (Texture2D)Resources.Load ("RecordIcon");
		}
	}
}