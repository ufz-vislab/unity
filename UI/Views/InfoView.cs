using UnityEngine;
using MarkUX;
using MarkUX.Views;
using UFZ.Interaction;

public class InfoView : View
{
	public Sprite Image;
	public string ImageWidth;
	public string ImageHeight;
	public string ObjectName;
	public string PageInfo;

	public Image ImageWidget;

	private ObjectInfo _objectInfo;
	private int _index = 0;

	public override void Initialize()
	{
		base.Initialize();

		// BUG workaround(?): Images are rotated 180 degrees around Y-axis
		ImageWidget.transform.localRotation = Quaternion.identity;

		// Empty init
		ImageWidth = "0";
		ImageHeight = "0";
	}

	public void SetObjectInfo(ObjectInfo objectInfo)
	{
		_objectInfo = objectInfo;
		ObjectName = _objectInfo.name;
		SetChanged(() => ObjectName);
		SetImage(0);
	}

	private void SetImage(int newIndex)
	{
		if (newIndex >= _objectInfo.Images.Length || newIndex < 0)
			return;

		var tex2D = _objectInfo.Images[newIndex];
		if (Image != null)
			Destroy(Image);

		SetValue(() => Image, Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height),
			new Vector2(0.5f, 0.5f)));
		SetValue(() => ImageWidth, tex2D.width + "px");
		SetValue(() => ImageHeight, tex2D.height + "px");
		SetValue(() => PageInfo, (newIndex + 1) + " / " + (_objectInfo.Images.Length));

		_index = newIndex;
	}

	public void Forward()
	{
		SetImage(_index + 1);
	}

	public void Backward()
	{
		SetImage(_index - 1);
	}

	public void Show()
	{
		this.GetLayoutRoot().transform.parent.gameObject.SetActive(true);
	}

	public void Close()
	{
		this.GetLayoutRoot().transform.parent.gameObject.SetActive(false);
	}
}
