using MarkLight;
using MarkLight.Views.UI;
using UFZ.Interaction;
using UnityEngine;

namespace UFZ.UI.Views
{
	public class InfoView : UIView
	{
		public Sprite Image;
		public string ImageWidth;
		public string ImageHeight;
		public string PageInfo;
		// ReSharper disable once InconsistentNaming
		public DragableUIView DragableUIView;

		public Image ImageWidget;

		[ChangeHandler("ObjectInfoChanged")]
		public ObjectInfo ObjectInfo;

		private int _index;

		public override void Initialize()
		{
			base.Initialize();

			// BUG workaround(?): Images are rotated 180 degrees around Y-axis
			ImageWidget.transform.localRotation = Quaternion.identity;

			// Empty init
			ImageWidth = "800";
			ImageHeight = "494"; // 600 -106
		    ObjectInfoChanged();
		}

		public void ObjectInfoChanged()
		{
			if (ObjectInfo == null)
				return;
			SetImage(0);
		}
	    
	    private static bool IsPowerOfTwo(int x)
	    {
	        return (x & (x - 1)) == 0;
	    }

	    private void SetImage()
	    {
	        SetImage(_index);
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

            var parentTransform = GetComponent<RectTransform>();
            var p_w = parentTransform.rect.width;
            var p_h = parentTransform.rect.height - 106;
            var p_ratio = p_w / p_h;

            var w = tex2D.width;
            var h = tex2D.height;
		    if (IsPowerOfTwo(w) && IsPowerOfTwo(h))
		        Debug.LogWarning("On image "+ tex2D.name + " under Import Settings set 'Advanced / Non Power of 2' to" +
		                         " 'None' for proper scaling in InfoViews!");
            var ratio = w / h;
            var i_w = 0;
            var i_h = 0;

            if (ratio > p_ratio)
            {
                i_w = (int)p_w;
                i_h = (int)(h / (w / p_w));
            }
            else
            {
                i_w = (int)(w / (h / p_h));
                i_h = (int)p_h;
            }
            SetValue(() => ImageWidth, i_w + "px");
            SetValue(() => ImageHeight, i_h + "px");

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

        public void DragStart()
	    {
            DragableUIView.DragStart();
	    }

	    public void DragEnd()
	    {
            DragableUIView.DragEnd();
	    }
	    
	    public override void Activate()
	    {
	        base.Activate();
	        // Delay because widget is not sized at this time
	        Invoke("SetImage", 0.25f);
	    }
	}
}
