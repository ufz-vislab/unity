#if UNITY_STANDALONE_WIN

namespace UFZ.VTK
{
	public class ActiveScalarPlayer : Interaction.IPlayable
	{
		public VtkRenderer Renderer;

#if MVR
		private vrCommand _activeArrayCommand;

		protected void Awake()
		{
			_activeArrayCommand = new vrCommand("", ActiveArrayCommandHandler);

		}

		private void OnDestroy()
		{
			MiddleVR.DisposeObject(ref _activeArrayCommand);
		}

		private vrValue ActiveArrayCommandHandler(vrValue index)
		{
			SetStep(index.GetInt());
			return true;
		}
#endif

		protected void Start()
		{
			Invoke("SetTimesteps", 2);
		}

		private void SetTimesteps()
		{
			NumSteps = Renderer.NumPointDataArrays;
		}

		public override void SetStep(int index)
		{
			base.SetStep(index);
			Renderer.Enabled = true; // Show object
			Renderer.ActiveColorArrayIndex = (uint)GetStep();

		}

		public override void Stop()
		{
			base.Stop();
			Renderer.Enabled = false; // Hide object
		}
	}
}
#endif
