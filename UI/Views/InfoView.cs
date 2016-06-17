using MarkLight;
using MarkLight.Views.UI;
using UnityEngine;
using UFZ.Interaction;

public class InfoView : UIView
{
	public Sprite Image;
	public string ImageWidth;
	public string ImageHeight;
	public string PageInfo;
	public DragableUIView DragableUIView;

	public Image ImageWidget;

	[ChangeHandler("ObjectInfoChanged")]
	public ObjectInfo ObjectInfo;

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

	protected void ObjectInfoChanged()
	{
		if (ObjectInfo == null)
			return;
		SetImage(0);
	}

	private void SetImage(int newIndex)
	{
		if (ObjectInfo.Images == null || newIndex >= ObjectInfo.Images.Length || newIndex < 0)
			return;

		var tex2D = ObjectInfo.Images[newIndex];
		if (Image != null)
			Destroy(Image);

		SetValue(() => Image, Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height),
			new Vector2(0.5f, 0.5f)));
		SetValue(() => ImageWidth, tex2D.width + "px");
		SetValue(() => ImageHeight, tex2D.height + "px");
		SetValue(() => PageInfo, (newIndex + 1) + " / " + (ObjectInfo.Images.Length));

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

	public void Close()
	{
		Deactivate();
	}
}
