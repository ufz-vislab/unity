#if UNITY_STANDALONE_WIN
using UnityEngine;
using System.Collections;
using Kitware.VTK;
using UFZ.VTK;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class VtkExternalRenderer : MonoBehaviour
{
	protected vtkExternalOpenGLRenderer externalRenderer;
	public VtkAlgorithm Algorithm;
	protected vtkActor actor;
	protected vtkPolyDataMapper mapper;
	protected vtkExternalOpenGLRenderWindow window;

	protected bool contextInit = false;

#if UNITY_EDITOR
	private void Reset()
	{
		Initialize();
	}
#endif

	private void Initialize()
	{
		if (Algorithm == null)
			return;

		mapper = vtkPolyDataMapper.New();
		mapper.SetInputConnection(Algorithm.OutputPort());

		actor = vtkActor.New();
		actor.SetMapper(mapper);

		externalRenderer = vtkExternalOpenGLRenderer.New();
		externalRenderer.AddActor(actor);

		window = vtkExternalOpenGLRenderWindow.New();
		window.AddRenderer(externalRenderer);

		//ExternalVTKWidget widget = ExternalVTKWidget.New();
		//widget.
	}

	private void Awake()
	{
		if (Application.isPlaying)
			Initialize();
	}

	private void OnEnable()
	{
#if UNITY_EDITOR
		EditorApplication.playmodeStateChanged += delegate
		{
			if (!EditorApplication.isPlaying)
				Initialize();
		};
#endif
	}

	private void OnDisable()
	{
#if UNITY_EDITOR
		EditorApplication.playmodeStateChanged = null;
#endif
	}

	public void Start()
	{
		//externalRenderer.
	}

	public void Update()
	{
		//window.Render();
		//externalRenderer.Render();
	}

	public void OnPostRender()
	{
		if (!contextInit)
		{
			GL.Begin(GL.TRIANGLES);
			contextInit = window.InitializeFromCurrentContext();
			//window.OpenGLInitContext();
			//contextInit = true;
			GL.End();
		}
		else
			externalRenderer.Render();
		//window.Render();
	}
}
#endif
