using Sirenix.OdinInspector;
using UnityEngine;

namespace UFZ.Rendering
{
	public class CityEngineMaterialProperties : SerializedMonoBehaviour, IHideable
	{

		[ShowInInspector]
		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				_enabled = value;
				foreach (var meshRenderer in gameObject.GetComponentsInChildren<MeshRenderer>())
				{
					meshRenderer.enabled = value;
				}
			}
		}
		[SerializeField, HideInInspector]
		private bool _enabled = true;
	}
}