// Inspired from Camera Path 3
// http://support.jasperstocker.com/camera-path/

using UnityEngine;
using UnityEditor;

namespace UFZ.Interaction
{
	[CustomEditor(typeof (Viewpoint))]
	public class ViewpointEditor : Editor
	{
		private GameObject _editorPreview;

		private const float Aspect = 1.7777f;
		private const int PreviewResolution = 800;

		private Vector3 _previewCamPos;
		private Quaternion _previewCamRot;

		private static bool PreviewSupported
		{
			get
			{
#if UNITY_EDITOR
				if (!SystemInfo.supportsRenderTextures) return false;
				// if (SystemInfo.graphicsShaderLevel >= 50 && PlayerSettings.useDirect3D11) return false;
				return Application.HasProLicense();
#endif
			}
		}

		private void OnEnable()
		{
			//Preview Camera
			if (_editorPreview != null)
				DestroyImmediate(_editorPreview);
			if (PreviewSupported)
			{
				_editorPreview = new GameObject("Animtation Preview Cam");
				_editorPreview.hideFlags = HideFlags.HideAndDontSave;
				_editorPreview.AddComponent<Camera>();
				_editorPreview.GetComponent<Camera>().fieldOfView = 60;
				_editorPreview.GetComponent<Camera>().depth = -1;
				//Retreive camera settings from the main camera
				var cams = Camera.allCameras;
				var sceneHasCamera = cams.Length > 0;
				Camera sceneCamera = null;
				Skybox sceneCameraSkybox = null;
				if (Camera.main)
					sceneCamera = Camera.main;
				else if (sceneHasCamera)
					sceneCamera = cams[0];

				if (sceneCamera != null)
					sceneCameraSkybox = sceneCamera.GetComponent<Skybox>();
				if (sceneCamera != null)
				{
					_editorPreview.GetComponent<Camera>().backgroundColor = sceneCamera.backgroundColor;
					if (sceneCameraSkybox != null)
						_editorPreview.AddComponent<Skybox>().material = sceneCameraSkybox.material;
					else if (RenderSettings.skybox != null)
						_editorPreview.AddComponent<Skybox>().material = RenderSettings.skybox;

					_editorPreview.GetComponent<Camera>().orthographic = sceneCamera.orthographic;
				}
				_editorPreview.GetComponent<Camera>().enabled = false;
			}

			if (EditorApplication.isPlaying && _editorPreview != null)
				_editorPreview.SetActive(false);
		}

		private void OnDisable()
		{
			CleanUp();
		}

		private void OnDestroy()
		{
			CleanUp();
		}

		public void OnSceneGUI()
		{

			//Axis Gizmo Marker
			Handles.color = Color.green;
			Handles.DrawLine(_previewCamPos, _previewCamPos + _previewCamRot*Vector3.up*0.5f);
			Handles.DrawLine(_previewCamPos, _previewCamPos + _previewCamRot*Vector3.down*0.5f);
			Handles.color = Color.red;
			Handles.DrawLine(_previewCamPos, _previewCamPos + _previewCamRot*Vector3.left*0.5f);
			Handles.DrawLine(_previewCamPos, _previewCamPos + _previewCamRot*Vector3.right*0.5f);
			Handles.color = Color.blue;
			Handles.DrawLine(_previewCamPos, _previewCamPos + _previewCamRot*Vector3.forward*0.5f);
			Handles.DrawLine(_previewCamPos, _previewCamPos + _previewCamRot*Vector3.back*0.5f);

			//Preview Direction Arrow
			var handleSize = HandleUtility.GetHandleSize(_previewCamPos);
			Handles.ArrowCap(0, _previewCamPos, _previewCamRot, handleSize);
			Handles.Label(_previewCamPos, "Viewpoint\nPosition");

			if (GUI.changed)
				UpdateGui();
		}

		public override void OnInspectorGUI()
		{
			EditorGUILayout.BeginVertical(GUILayout.Width(400));
			((Viewpoint) target).StartHere = EditorGUILayout.Toggle("Start Application Here", ((Viewpoint) target).StartHere);
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

			if (!PreviewSupported || EditorApplication.isPlaying)
				return;

			var rt = RenderTexture.GetTemporary(PreviewResolution, Mathf.RoundToInt(PreviewResolution/Aspect), 24,
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
