using Sirenix.OdinInspector;
using UnityEngine;

namespace UFZ.Interaction
{
    /// <summary>
    /// An interface for something which has playing controls with discrete timesteps.
    /// </summary>
    public abstract class IPlayable : SerializedMonoBehaviour
    {
        public bool IsPlaying;
        public string Name = "";
        public virtual float Percentage { get; set; }
        [HideInInspector] public string TimeInfo;
        public float Fps = 1f;

        [ReadOnly]
        public int NumSteps;

        public virtual void Begin()
        {
            IsPlaying = false;
            _elapsedTime = 0;
            SetStep(0);
        }

        public virtual void End()
        {
            IsPlaying = false;
            _elapsedTime = 0;
            SetStep(NumSteps - 1);
        }

        public virtual void Forward()
        {
            IsPlaying = false;
            _elapsedTime = 0;
            SetStep(_step + 1);
        }

        public virtual void Back()
        {
            IsPlaying = false;
            _elapsedTime = 0;
            SetStep(_step - 1);
        }

        public virtual void Play()
        {
            IsPlaying = true;
        }

        public virtual void Stop()
        {
            IsPlaying = false;
            _elapsedTime = 0;
        }

        public virtual void TogglePlay()
        {
            IsPlaying = !IsPlaying;
        }

        public virtual void SetStep(int step)
        {
            if (step < 0)
                _step = NumSteps - 1;
            else if (step >= NumSteps)
                _step = 0;
            else
                _step = step;
            Percentage = (float)_step / (NumSteps-1);
            TimeInfo = string.Format("{0:00}", _step);
        }

        public int GetStep()
        {
            return _step;
        }

        protected virtual void Update()
        {
            if (!IsPlaying) return;
            _elapsedTime += Core.DeltaTime();
            if (!(_elapsedTime > (1f / Fps)))
                return;

            _elapsedTime = 0;
            SetStep(_step + 1);
        }

        [ShowInInspector]
        public int Step
        {
            get { return _step; }
            set
            {
                _previousStep = _step;
                if (value < 0)
                    _step = NumSteps - 1;
                else if (value >= NumSteps)
                    _step = 0;
                else
                    _step = value;
                Percentage = (float) _step / (NumSteps - 1);
                TimeInfo = string.Format("{0:00}", _step);
            }
        }
        [SerializeField, HideInInspector] private int _step;
        [SerializeField, HideInInspector] protected int _previousStep;

        private float _elapsedTime;
    }
}
