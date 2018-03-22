using MarkLight;
using UFZ.Initialization;
using UnityEngine;

[ExcludeComponent("UnityEventSystem")]
public class UfzEventSystem : MarkLight.Views.EventSystem
{
	public WandInputModule WandInputModule;

	public void Awake()
	{
		#if MVR
		return;
		UFZ.Core.Info("UfzEventSystem: Creating cursor");
		var cursorPrefab = Resources.Load("WandCursor");
		var cursor = (GameObject)Instantiate(cursorPrefab);//SRResources.WandCursor.Instantiate();
		cursor.transform.SetParent(transform, false);
		WandInputModule.cursor = cursor.GetComponent<RectTransform>();

		var globalInits = FindObjectOfType<GlobalInits>();
		if (globalInits.GuiInputType == GlobalInits.InputType.Mouse)
		{
			UFZ.Core.Info("UfzEventSystem: Mouse input, disabling cursor");
			WandInputModule.enabled = false;
			WandInputModule.cursor.gameObject.SetActive(false);
		}
		#endif
	}
}
