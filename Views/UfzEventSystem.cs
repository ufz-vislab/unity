using MarkLight;
using UFZ.Initialization;
using UnityEngine;

[ExcludeComponent("UnityEventSystem")]
public class UfzEventSystem : MarkLight.Views.EventSystem
{
	public WandInputModule WandInputModule;

	public void Awake()
	{
		var cursor = SRResources.WandCursor.Instantiate();
		cursor.transform.SetParent(transform, false);
		WandInputModule.cursor = cursor.GetComponent<RectTransform>();

		var globalInits = FindObjectOfType<GlobalInits>();
		if (globalInits.GuiInputType == GlobalInits.InputType.Mouse)
		{
			WandInputModule.enabled = false;
			WandInputModule.cursor.gameObject.SetActive(false);
		}
	}
}
