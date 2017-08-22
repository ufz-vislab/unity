#if UNITY_STANDALONE_WIN
using Kitware.VTK;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UFZ.VTK
{
	public class VtkLookupTable : MonoBehaviour
	{
		public Gradient Gradient;

		[HideInInspector]
		public vtkLookupTable Lut;
		
		[ShowInInspector]
		public Vector2 Range
		{
			get { return _range; }
			set
			{
				_range = value;
				Lut.SetRange(value.x, value.y);
			}
		}
		[SerializeField, HideInInspector]
		private Vector2 _range;

		private void Reset()
		{
			Initialize();
		}

		protected void OnValidate()
		{
			Initialize();
			
			Lut.SetNumberOfTableValues(Gradient.colorKeys.Length);
			Lut.Build();
			var index = 0;
			foreach (var colorKey in Gradient.colorKeys)
			{
				Lut.SetTableValue(index, colorKey.color.r, colorKey.color.g, colorKey.color.b, 1.0);
				index++;
			}
			Lut.Build();
			if(OnChange != null)
				OnChange();

		}

		private void Initialize()
		{
			SetDllPath.Init();
			if (Lut == null)
				Lut = vtkLookupTable.New();
		}
		
		public delegate void ChangeAction();
		public event ChangeAction OnChange;
	}
}
#endif