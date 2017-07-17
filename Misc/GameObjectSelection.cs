using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UFZ.Helper;

namespace UFZ.Misc
{
	/// <summary>
	/// A selection of GameObjectse either explicitly defined or implicitly by
	/// given a Base GameObject and a search string - children matching that
	/// string a selected.
	/// 
	/// Can be used on the Viewpoints object to select objects which can be
	/// switched on / off via the VisibilityView (menu).
	/// </summary>
	public class GameObjectSelection : SerializedMonoBehaviour
	{
		public struct SelectionInfo
		{
			public GameObject Base;
			public bool SearchChildren;
			[ShowIf("SearchChildren")]
			public string SearchString;
			[ShowIf("SearchChildren")]
			public bool IncludeBase;

			[HideInInspector]
			public List<GameObject> Selected;

			[ShowIf("SearchChildren"), ReadOnly]
			public int SelectedObjects;
		}

		[OdinSerialize]
		public SelectionInfo[] Selections = new SelectionInfo[0];

		protected void OnValidate()
		{
			for (var i = 0; i < Selections.Length; ++i)
			{
				var info = Selections[i];
				info.Selected = new List<GameObject>();
				if(info.Base == null)
					continue;
				if (!info.SearchChildren)
				{
					info.Selected.Add(info.Base);
					Selections[i] = info;
					continue;
				}
				if (info.IncludeBase)
					info.Selected.Add(info.Base);

				IterateChildren.Iterate(info.Base, delegate(GameObject go)
				{
					var localInfo = info; // Has to be copied
					if(go.name.Contains(localInfo.SearchString))
						localInfo.Selected.Add(go);
				}, true);
				info.SelectedObjects = info.Selected.Count;
				Selections[i] = info;    // Has to be copied back
			}
		}
	}
}