using UFZ.Interaction;
using UFZ.Misc;
using UnityEngine;
using UnityEditor;

namespace UFZ.Initialization
{
	public class CreateSceneSetup : MonoBehaviour
	{
		[MenuItem("UFZ/Create scene setup")]
		static void CreateSetup()
		{
			var globalTransformGo = GameObject.Find("Global Transform");
			var sceneSetupGo = GameObject.Find("Scene Setup") ?? new GameObject("Scene Setup");
			AddComponent<UFZ.Setup.SceneSetup>(sceneSetupGo);

			GetChildGameObject(sceneSetupGo, "CameraPaths");
			if (globalTransformGo)
				GetChildGameObject(globalTransformGo, "Viewpoints");
			GetChildGameObject(sceneSetupGo, "Viewpoints");
			var visibilitiesGo = GetChildGameObject(sceneSetupGo, "Visibilities");
			AddComponent<GameObjectList>(visibilitiesGo);
			var animationsGo = GetChildGameObject(sceneSetupGo, "Animations");
			AddComponent<GameObjectList>(animationsGo);

			Selection.activeGameObject = sceneSetupGo;
		}

		[MenuItem("UFZ/Add viewpoint")]
		static void AddViewpoint()
		{
			var viewpointsGo = GameObject.Find("Viewpoints");
			if (viewpointsGo == null)
			{
				Debug.LogError("Viewpoints object not found. Run menu item " +
					"UFZ/Create scene setup!");
				return;
			}

			var viewpointGo = new GameObject("New Viewpoint");
			Undo.RegisterCreatedObjectUndo(viewpointGo, "Created new viewpoint");
			viewpointGo.transform.SetParent(viewpointsGo.transform, false);
			// TODO sets the pivot point and not the transform?
			//var cam = Camera.current;
			//if (cam)
			//{
			//	viewpointsGo.transform.position = cam.transform.position;
			//	viewpointsGo.transform.rotation = cam.transform.rotation;
			//	viewpointsGo.transform.hasChanged = true;
			//}
			viewpointGo.AddComponent<Viewpoint>();

			Selection.activeGameObject = viewpointGo;
		}

		private static GameObject GetChildGameObject(GameObject parentGo, string childName)
		{
			GameObject childGo;
			var childTr = parentGo.transform.Find(childName);
			if (childTr == null)
			{
				childGo = new GameObject(childName);
				childGo.transform.SetParent(parentGo.transform, false);
			}
			else
			{
				childGo = childTr.gameObject;
			}
			return childGo;
		}

		private static T AddComponent<T>(GameObject go) where T : MonoBehaviour
		{
			return go.GetComponent<T>() ?? go.AddComponent<T>();
		}
	}
}
