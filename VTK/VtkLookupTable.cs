#if UNITY_STANDALONE_WIN
using Kitware.VTK;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UFZ.VTK
{
	public class VtkLookupTable : MonoBehaviour
	{
		[HideInInspector]
		public vtkLookupTable Lut;
		
		[ShowInInspector, MinMaxSlider(0f, 1f, true)]
		public Vector2 HueRange
		{
			get { return _hueRange; }
			set
			{
				_hueRange = value;
				Lut.SetHueRange(value.x, value.y);
			}
		}
		[SerializeField, HideInInspector]
		private Vector2 _hueRange;
		
		[ShowInInspector, MinMaxSlider(0f, 1f, true)]
		public Vector2 SaturationRange
		{
			get { return _saturationRange; }
			set
			{
				_saturationRange = value;
				if (_flipHueRange)
					Lut.SetSaturationRange(value.y, value.x);
				else
					Lut.SetSaturationRange(value.x, value.y);
			}
		}
		[SerializeField, HideInInspector]
		private Vector2 _saturationRange;
		
		/*
		[ShowInInspector, MinMaxSlider(0f, 1f, true)]
		public Vector2 ValueRange
		{
			get { return _valueRange; }
			set
			{
				_valueRange = value;
				Lut.SetValueRange(value.x, value.y);
			}
		}
		[SerializeField, HideInInspector]
		private Vector2 _valueRange;
		*/

		[ShowInInspector]
		public bool FlipHueRange
		{
			get { return _flipHueRange; }
			set
			{
				_flipHueRange = value;
				SaturationRange = _saturationRange;
			}
		}

		[SerializeField, HideInInspector]
		private bool _flipHueRange;

		private void Reset()
		{
			Initialize();
		}

		protected void Start()
		{
			Initialize();
		}

		protected void OnValidate()
		{
			Initialize();
			
			Lut.SetNumberOfColors(256);
			Lut.Build();
			if(OnChange != null)
				OnChange();

		}

		private void Initialize()
		{
			SetDllPath.Init();
			if (Lut == null)
				Lut = vtkLookupTable.New();
			Lut.SetRampToLinear();
			Lut.SetValueRange(1f,1f);
			
			// Local inits
			HueRange = _hueRange;
			FlipHueRange = _flipHueRange;
			SaturationRange = _saturationRange;
		}
		
		public delegate void ChangeAction();
		public event ChangeAction OnChange;
	}
}
#endif