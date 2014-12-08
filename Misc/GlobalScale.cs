using UnityEngine;
using System.Collections;

public class GlobalScale
{
	static Vector3 GetParentScale (GameObject gameObject)
	{
		Vector3 sf = new Vector3 (1f, 1f, 1f);
		Transform parentTransform = null;
		if (gameObject.transform.parent)
			parentTransform = gameObject.transform.parent;
		
		while (parentTransform) {
			sf.x *= 1f / parentTransform.localScale.x;
			sf.y *= 1f / parentTransform.localScale.y;
			sf.z *= 1f / parentTransform.localScale.z;
			if (parentTransform.parent)
				parentTransform = parentTransform.parent;
			else
				parentTransform = null;
		}
		return sf;
	}
	
	static void SetGlobalScale (GameObject gameObject, Vector3 globalScale)
	{
		Vector3 parentGlobalScale = GetParentScale (gameObject);
		gameObject.transform.localScale = Vector3.Scale(globalScale, parentGlobalScale);
	}
}
