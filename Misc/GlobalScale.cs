using UnityEngine;

namespace UFZ.Initialization
{
	/// <summary>
	/// Helper class used by the WizardGlobalTransform.
	/// </summary>
	public class GlobalScale
	{
		private static Vector3 GetParentScale(GameObject gameObject)
		{
			var sf = new Vector3(1f, 1f, 1f);
			Transform parentTransform = null;
			if (gameObject.transform.parent)
				parentTransform = gameObject.transform.parent;

			while (parentTransform)
			{
				sf.x *= 1f/parentTransform.localScale.x;
				sf.y *= 1f/parentTransform.localScale.y;
				sf.z *= 1f/parentTransform.localScale.z;
				if (parentTransform.parent)
					parentTransform = parentTransform.parent;
				else
					parentTransform = null;
			}
			return sf;
		}

		private static void SetGlobalScale(GameObject gameObject, Vector3 globalScale)
		{
			var parentGlobalScale = GetParentScale(gameObject);
			gameObject.transform.localScale = Vector3.Scale(globalScale, parentGlobalScale);
		}
	}
}
