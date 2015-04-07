using UnityEngine;
using UnityEditor;

namespace UFZ.Initialization
{
	/// <summary>
	/// A wizard dialog for creating an appropriate global transformation.
	/// </summary>
	/// The selected GameObjects bounding box is calculated and its center is
	/// moved to world coordinates root. An additional scaling factor is applied.
	/// Once the global transform is created it is required to drag and drop
	/// additional GameObjects under it to preserve the right transformation.
	public class WizardGlobalTransform : ScriptableWizard
	{
		public float Scale = 0.01f;
		private float _oldScale = 0.01f;
		public string TransformName = "Global Transform";

		private GameObject[] _gos;
		private Bounds _bounds;
		private bool _transformSelected;

		[MenuItem("UFZ/Global Transformation")]
		private static void CreateWizard()
		{
			DisplayWizard<WizardGlobalTransform>
				("Apply global transformation", "Apply Transform!").Calculate();
		}

		private void Calculate()
		{
			_gos = Selection.gameObjects;
			if (_gos.Length == 0)
			{
				isValid = false;
				errorString = "No GameObjects selected in the Hierarchy Window!";
			}
			else if (_gos.Length == 1 && _gos[0].name == TransformName)
			{
				_transformSelected = true;
				Scale = _gos[0].transform.localScale.x;
				_oldScale = Scale;
				OnWizardUpdate();
				return;
			}

			var boundsInited = false;
			foreach (GameObject go in _gos)
			{
				var goRenderers = go.GetComponentsInChildren<Renderer>();
				foreach (var renderer in goRenderers)
				{
					if (!boundsInited)
					{
						_bounds = new Bounds(renderer.bounds.center, renderer.bounds.size);
						boundsInited = true;
					}
					_bounds.Encapsulate(renderer.bounds.min);
					_bounds.Encapsulate(renderer.bounds.max);
				}
			}
		}

		private void OnWizardCreate()
		{
			if (_transformSelected)
			{
				if (Mathf.Approximately(Scale, _oldScale))
					return;
				Debug.Log("Rescaled global transform.");
				var factor = _oldScale/Scale;
				_gos[0].transform.position = _gos[0].transform.position/factor;
				_gos[0].transform.localScale = new Vector3(Scale, Scale, Scale);
			}
			else
			{
				Debug.Log("Bounds center: " + _bounds.center);
				var transformGo = new GameObject(TransformName);
				foreach (var go in _gos)
					go.transform.parent = transformGo.transform;

				transformGo.transform.position = -_bounds.center*Scale;
				transformGo.transform.localScale = new Vector3(Scale, Scale, Scale);
			}
		}

		private void OnWizardUpdate()
		{
			if (_transformSelected)
				helpString = "Global Transform will be rescaled.";
			else
				helpString = "All selected objects will be appended to a transformation\n GameObject named "
				             + TransformName + " with a scaling of " + Scale + ".";
		}
	}
}
