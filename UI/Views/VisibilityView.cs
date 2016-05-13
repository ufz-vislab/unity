using System;
using UnityEngine;
using System.Collections.Generic;
using MarkLight;
using MarkLight.Views.UI;
using UFZ.Interaction;
using UFZ.Misc;
using UFZ.Rendering;

//[InternalView]
public class VisibilityView : UIView
{
	public DataGrid DataGrid;
	public ObservableList<VisibilityStruct> Objects;
	//public int AdditionalInfo;

	protected GameObjectList GoList;

	public override void Initialize()
	{
		base.Initialize();


		Objects = new ObservableList<VisibilityStruct>();

		var objectVisibilities = FindObjectsOfType<UFZ.Interaction.ObjectVisibility>();
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
		Objects.ItemsModified();
		//SetChanged(() => Objects);
	}

	public void EnabledHandler()
	{
		if (!IsActive)
			return;

		UpdateVisibilities();
	}

	public void EnabledClick(CheckBox source)
	{
		var visibilityStruct = Objects.SelectedItem;
		if (visibilityStruct == null)
			return;
		foreach (var matProp in visibilityStruct.MatProps)
			matProp.SetEnabled(visibilityStruct.Enabled);
	}

	public void OpacityChanged(Slider source)
	{
		var visibilityStruct = Objects.SelectedItem;
		if (visibilityStruct == null)
			return;
		foreach (var matProp in visibilityStruct.MatProps)
			matProp.SetOpacity(visibilityStruct.Opacity);
	}

	public void TextClick(HyperLink source)
	{
		// TODO: does not sync checkbox
		var visibilityStruct = Objects.SelectedItem;
		if (visibilityStruct == null)
			return;
		visibilityStruct.Enabled = !visibilityStruct.Enabled;
		//SetChanged(() => Objects);
		Objects.ItemModified(visibilityStruct);

		foreach (var matProp in visibilityStruct.MatProps)
			matProp.SetEnabled(visibilityStruct.Enabled);
	}
}

public class VisibilityStruct
{
	public string Name;
	public float Opacity;
	public bool Enabled;
	public MaterialProperties[] MatProps;
}
