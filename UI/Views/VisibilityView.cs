using System;
using UnityEngine;
using System.Collections.Generic;
using MarkLight;
using MarkLight.Views.UI;
using UFZ.Interaction;
using UFZ.Misc;
using UFZ.Rendering;

[HideInPresenter]
public class VisibilityView : UIView
{
	public DataGrid DataGrid;
	public ObservableList<VisibilityStruct> Objects;

	protected GameObjectList GoList;

	public override void Initialize()
	{
		base.Initialize();

		Objects = new ObservableList<VisibilityStruct>();

		var objectVisibilities = FindObjectsOfType<ObjectVisibility>();
		foreach (var objectVisibility in objectVisibilities)
		{
			objectVisibility.RestoreState();
			if (objectVisibility.Entries == null || objectVisibility.Entries.Length == 0)
				continue;

			var index = 0;
			foreach (var objectVisibilityInfo in objectVisibility.Entries)
			{
				var materialProperties = objectVisibilityInfo.GameObject.GetComponentsInChildren<MaterialProperties>();
				if (materialProperties == null || materialProperties.Length == 0)
					continue;

				Objects.Add(new VisibilityStruct
				{
					Name = objectVisibilityInfo.GameObject.name,
					Opacity = materialProperties[0].Opacity,
					Enabled = materialProperties[0].Enabled,
					MatProps = materialProperties
				});
				index++;
			}
		}

		var go = GameObject.Find("Visibilities");
		if (go == null)
			return;
		GoList = go.GetComponentInChildren<GameObjectList>();

		UpdateVisibilities();
	}

	private void UpdateVisibilities()
	{
		if (GoList == null)
			return;
		if (GoList.Objects == null)
			return;

		Objects.Clear();
		foreach (var o in GoList.Objects)
		{
			if (o == null)
				continue;

			var materialProperties = o.GetComponentsInChildren<MaterialProperties>();
			if (materialProperties == null || materialProperties.Length == 0)
				continue;

			foreach (var materialPropertiese in materialProperties)
			{
				materialPropertiese.RestoreState();
			}
			Objects.Add(new VisibilityStruct
			{
				Name = o.name,
				Opacity = materialProperties[0].Opacity,
				Enabled = materialProperties[0].Enabled,
				MatProps = materialProperties
			});
		}
		var go = GameObject.Find("Visibilities");
		var selection = go.GetComponentInChildren<GameObjectSelection>();
		if (selection != null)
		{
			foreach (var selectionInfo in selection.Selections)
			{
				var name = selectionInfo.Base.name;
				if (selectionInfo.SearchChildren)
					name = name + "-" + selectionInfo.SearchString;

				Objects.Add(new VisibilityStruct
				{
					Name = name,
					Enabled = true,
					GameObjects = selectionInfo.Selected == null ? null : selectionInfo.Selected.ToArray()
				});
			}
		}

		Objects.ItemsModified();
	}

	public void EnabledHandler()
	{
		if (!IsActive)
			return;

		UpdateVisibilities();
	}

	public void EnabledClick(CheckBox source)
	{
		var visibilityStruct = source.Item.Value as VisibilityStruct;
		SetEnabled(visibilityStruct);
	}

	public void OpacityChanged(Slider source)
	{
		var visibilityStruct = source.Item.Value as VisibilityStruct;
		if(visibilityStruct.MatProps != null)
			foreach (var matProp in visibilityStruct.MatProps)
				matProp.SetOpacity(visibilityStruct.Opacity);
		
	}

	public void TextClick(HyperLink source)
	{
		var visibilityStruct = source.Item.Value as VisibilityStruct;
		visibilityStruct.Enabled = !visibilityStruct.Enabled;
		Objects.ItemModified(visibilityStruct);

		SetEnabled(visibilityStruct);
	}

	private static void SetEnabled(VisibilityStruct visibilityStruct)
	{
		if (visibilityStruct.MatProps != null)
			foreach (var matProp in visibilityStruct.MatProps)
				matProp.SetEnabled(visibilityStruct.Enabled);
		if (visibilityStruct.GameObjects != null)
			foreach (var go in visibilityStruct.GameObjects)
			{
				var objectSwitches = go.GetComponentsInChildren<ObjectSwitchBase>();
				if (objectSwitches != null && objectSwitches.Length > 0)
				{
					foreach (var switchBase in objectSwitches)
						switchBase.Active = visibilityStruct.Enabled;
				}
				else
					go.SetActive(visibilityStruct.Enabled);
			}
	}
}

public class VisibilityStruct
{
	public string Name;
	public float Opacity;
	public bool Enabled;
	public MaterialProperties[] MatProps;
	public GameObject[] GameObjects;
}
