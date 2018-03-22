// Inspired from Camera Path 3
// http://support.jasperstocker.com/camera-path/

using UnityEngine;
using UnityEditor;

namespace UFZ.Interaction
{
	[CustomEditor(typeof(Viewpoint))]
	public class ViewpointEditor : UnityEditor.Editor
	{
		private GameObject _editorPreview;

		private const float Aspect = 1.7777f;
		private const int PreviewResolution = 800;

		private Vector3 _previewCamPos;
		private Quaternion _previewCamRot;

		private void OnEnable()
		{
			//Preview Camera
			if (_editorPreview != null)
				DestroyImmediate(_editorPreview);
			if (true)
			{
				_editorPreview = new GameObject("Animtation Preview Cam") {hideFlags = HideFlags.HideAndDontSave};
				var cam = _editorPreview.AddComponent<Camera>();
				cam.fieldOfView = 60;
				cam.depth = -1;
				//Retreive camera settings from the main camera
				Camera[] cams = Camera.allCameras;
				bool sceneHasCamera = cams.Length > 0;
				Camera sceneCamera = null;
				Skybox sceneCameraSkybox = null;
				if (Camera.main)
				{
					sceneCamera = Camera.main;
				}
				else if (sceneHasCamera)
				{
					sceneCamera = cams[0];
				}

				if (sceneCamera != null)
					sceneCameraSkybox = sceneCamera.GetComponent<Skybox>();
				if (sceneCamera != null)
				{
					cam.backgroundColor = sceneCamera.backgroundColor;
					if (sceneCameraSkybox != null)
						_editorPreview.AddComponent<Skybox>().material = sceneCameraSkybox.material;
					else if (RenderSettings.skybox != null)
						_editorPreview.AddComponent<Skybox>().material = RenderSettings.skybox;

					cam.orthographic = sceneCamera.orthographic;
				}
				cam.enabled = false;
			}

			if (EditorApplication.isPlaying && _editorPreview != null)
				_editorPreview.SetActive(false);
		}

		void OnDisable()
		{
			CleanUp();
		}

		void OnDestroy()
		{
			CleanUp();
		}

		public void OnSceneGUI()
		{
			Handles.Label(_previewCamPos, "Viewpoint " + target.name);

			if (GUI.changed)
				UpdateGui();
		}

		public override void OnInspectorGUI()
		{
			EditorGUILayout.BeginVertical(GUILayout.Width(400));
			RenderPreview();

			//Get animation values and apply them to the preview camera
			var viewpoint = (Viewpoint) target;
			_previewCamPos = viewpoint.transform.position;
			_previewCamRot = viewpoint.transform.rotation;

			if (GUI.changed)
				UpdateGui();
			EditorGUILayout.EndVertical();
		}

		private void RenderPreview()
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Preview");
			EditorGUILayout.EndHorizontal();

			if (!EditorApplication.isPlaying)
			{
				RenderTexture rt = RenderTexture.GetTemporary(PreviewResolution, Mathf.RoundToInt(PreviewResolution / Aspect), 24,
					RenderTextureFormat.Default, RenderTextureReadWrite.Default, 1);

				_editorPreview.SetActive(true);
				_editorPreview.transform.position = _previewCamPos;
				_editorPreview.transform.rotation = _previewCamRot;

				var previewCam = _editorPreview.GetComponent<Camera>();

				previewCam.enabled = true;
				previewCam.targetTexture = rt;
				previewCam.Render();
				previewCam.targetTexture = null;
				previewCam.enabled = false;
				_editorPreview.SetActive(false);

				GUILayout.Label("", GUILayout.Width(400), GUILayout.Height(225));
				var guiRect = GUILayoutUtility.GetLastRect();
				GUI.DrawTexture(guiRect, rt, ScaleMode.ScaleToFit, false);
				RenderTexture.ReleaseTemporary(rt);
			}
		}


		/// <summary>
		/// Called to ensure we're not leaking stuff into the Editor
		/// </summary>
		public void CleanUp()
		{
			if (_editorPreview != null)
				DestroyImmediate(_editorPreview);
		}

		/// <summary>
		/// Handle GUI changes and repaint requests
		/// </summary>
		private void UpdateGui()
		{
			Repaint();
			HandleUtility.Repaint();
			SceneView.RepaintAll();
		}
	}
}