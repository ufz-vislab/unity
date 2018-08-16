using System.Collections;
using UnityEngine;
using System.Linq;
using UFZ.UI.Views;

namespace UFZ.Interaction
{
    public class ObjectSwitch : IPlayable
    {
        public enum Ordering
        {
            Alphanumeric,
            Transform
        }

        protected GameObject ActiveChildGo;
        private VisibilityView _visibilityView;
        public delegate void Callback(int index);
        public Callback ActiveChildCallback;
        public Ordering Order = Ordering.Alphanumeric;

        private Transform[] _transforms;

        public bool Active
        {
            get { return _active; }
            set
            {
                foreach (var childRenderer in ActiveChildGo.GetComponentsInChildren<Renderer>(true))
                    childRenderer.enabled = value;
                _active = value;
            }
        }

        private bool _active = true;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        private vrCommand _activeChildCommand;

        protected void Start()
        {
            _activeChildCommand = new vrCommand("", ActiveChildCommandHandler);
            _visibilityView = Resources.FindObjectsOfTypeAll(typeof (VisibilityView))[0] as VisibilityView;
        }

        protected void OnEnable()
        {

        }

        private void OnDestroy()
        {
            MiddleVR.DisposeObject(ref _activeChildCommand);
        }

        private vrValue ActiveChildCommandHandler(vrValue index)
        {
            SetStep(index.GetInt());
            return true;
        }
#endif

        protected virtual void OnValidate()
        {
            _transforms = Order == Ordering.Alphanumeric
                ? transform.Cast<Transform>().ToArray().OrderBy(t => t.name, new AlphanumComparatorFast()).ToArray()
                : transform.Cast<Transform>().ToArray();

            NumSteps = transform.childCount;
            SetStep(GetStep());
        }

        public void SetActiveChild(float percentage)
        {
            SetActiveChild((int) (percentage * (transform.childCount - 1)));
        }

        public void SetActiveChild(int index)
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            if (_activeChildCommand != null)
                _activeChildCommand.Do(index);
            else
                SetStep(index);
#else
            SetStep(index);
#endif
        }

        public override void SetStep(int step)
        {
            if (_transforms == null)
                return;
            var previousStep = base._previousStep;
            base.SetStep(step);
            step = GetStep();

            if (_transforms.Length < step + 1)
                return;

            ActiveChildGo = _transforms[step].gameObject;
            if (!_active)
                return;

            SetVisible(_transforms[previousStep], false);
            SetVisible(_transforms[step], true);

            if (ActiveChildCallback != null)
                ActiveChildCallback(step);
        }

        protected void Reset()
        {
            ResetRenderers();
        }

        [Sirenix.OdinInspector.Button]
        public void ResetRenderers()
        {
            var i = 0;
            var step = base.GetStep();
            foreach (var child in _transforms)
            {
                var enable = i == step || step < 0;
                SetVisible(child, enable);
                ++i;
            }
        }

        private void SetVisible(Transform currentTransform, bool enable)
        {
            var objectSwitch = currentTransform.GetComponent<ObjectSwitch>();

            // Don't process objects which are switched off in Visibility View
            if (enable && _visibilityView != null && _visibilityView.Objects != null)
            {
                if (_visibilityView.Objects.Any(obj => obj.Name == gameObject.name &&
                                                           obj.Enabled == false))
                    return;
            }

            if (objectSwitch != null && enable)
            {
                objectSwitch.SetActiveChild(objectSwitch.GetStep());
                return;
            }
            var matProps = GetComponent<UFZ.Rendering.MaterialProperties>();
            foreach (var ren in currentTransform.GetComponents<Renderer>())
            {
                ren.enabled = enable;
                if (matProps)
                    ren.SetPropertyBlock(matProps.PropertyBlock);
            }

            for (var i = 0; i < currentTransform.childCount; i++)
                SetVisible(currentTransform.GetChild(i), enable);
        }

        protected void NoActiveChild()
        {
            foreach (var ren in transform.GetComponentsInChildren<Renderer>(true))
                ren.enabled = false;
        }

        public override void Stop()
        {
            base.Stop();
            NoActiveChild();
        }
    }
}
