#if UNITY_STANDALONE_WIN
using Sirenix.OdinInspector;
using UFZ.Interaction;
using UnityEngine;

namespace UFZ.VTK
{
	public class ActiveScalarPlayer : IPlayable
	{
		public VtkRenderer Renderer;
		public float Fps;
		public bool Loop = true;
		protected float ElapsedTime;

		public override float Percentage
		{
			get { return _percentage; }
			set
			{
				if (value < 0.0 || value > 1.0)
					return;
				_percentage = value;
				if (Renderer == null)
					return;
				CurrentIndex = (uint)Mathf.RoundToInt(value * (Renderer.NumPointDataArrays-1));
			}
		}
		private float _percentage;

		[ShowInInspector]
		public uint CurrentIndex
		{
			get { return _currentIndex; }
			set
			{
				if (Renderer == null)
					return;
#if MVR
				if (_activeArrayCommand != null)
					_activeArrayCommand.Do(value);
				else
					SetActiveArrayInternal(value);
#else
			SetActiveArrayInternal(value);
#endif
			}
		}

		[SerializeField, HideInInspector] private uint _currentIndex;

#if MVR
		private vrCommand _activeArrayCommand;

		protected void Start()
		{
			_activeArrayCommand = new vrCommand("", ActiveArrayCommandHandler);
		}

		private void OnDestroy()
		{
			MiddleVR.DisposeObject(ref _activeArrayCommand);
		}

		private vrValue ActiveArrayCommandHandler(vrValue index)
		{
			SetActiveArrayInternal(index.GetUInt());
			return true;
		}
#endif
		
		private void SetActiveArrayInternal(uint index)
		{
			if (index > Renderer.NumPointDataArrays && Loop)
				Renderer.ActiveColorArrayIndex = 0;
			else
				Renderer.ActiveColorArrayIndex = index;
			_currentIndex = Renderer.ActiveColorArrayIndex;
			_percentage = (float) _currentIndex /(Renderer.NumPointDataArrays-1);
			TimeInfo = CurrentIndex + 1 + " / " + Renderer.NumPointDataArrays;
		}

		public override void Begin()
		{
			if (Renderer == null)
				return;
			CurrentIndex = 0;
			IsPlaying = false;
		}

		public override void End()
		{
			if (Renderer == null)
				return;
			CurrentIndex = (uint)Renderer.NumPointDataArrays-1;
			IsPlaying = false;
		}

		public override void Forward()
		{
			if (Renderer == null)
				return;
			CurrentIndex += 1;
			IsPlaying = false;
		}

		public override void Back()
		{
			if (Renderer == null)
				return;
			CurrentIndex -= 1;
			IsPlaying = false;
		}

		public override void Play()
		{
			if (Renderer == null)
				return;
			ElapsedTime = 0;
			IsPlaying = true;
		}

		public override void Stop()
		{
			IsPlaying = false;
		}

		public override void TogglePlay()
		{
			if (Renderer == null)
				return;
			IsPlaying = !IsPlaying;
			ElapsedTime = 0;
		}

		private void Reset()
		{
			CurrentIndex = 0;
			Fps = 5;
		}

		private void Update()
		{
			if (!IsPlaying || Renderer == null) return;
			ElapsedTime += IOC.Core.Instance.Time.DeltaTime();
			if (ElapsedTime > (1f / Fps))
				CurrentIndex += 1;
		}
		
	}
}
#endif
