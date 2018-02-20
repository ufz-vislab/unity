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

#if MVR
		private vrCommand _activeChildCommand;

		protected void Start()
		{
			_activeChildCommand = new vrCommand("", ActiveChildCommandHandler);
			_visibilityView = Resources.FindObjectsOfTypeAll(typeof (VisibilityView))[0] as VisibilityView;
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
			NumSteps = transform.childCount;
			SetStep(GetStep());
		}

		public void SetActiveChild(float percentage)
		{
			SetActiveChild((int) (percentage * (transform.childCount - 1)));
		}

		public void SetActiveChild(int index)
		{
#if MVR
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
			base.SetStep(step);
			step = GetStep();

			var i = 0;
			var transforms = Order == Ordering.Alphanumeric
				? transform.Cast<Transform>().ToArray().OrderBy(t => t.name, new AlphanumComparatorFast()).ToArray()
				: transform.Cast<Transform>().ToArray();

			ActiveChildGo = transforms[step].gameObject;
			if (!_active)
				return;

			foreach (var child in transforms)
			{
				var enable = i == step || step < 0;
				SetVisible(child, enable);
				++i;
			}

			if (ActiveChildCallback != null)
				ActiveChildCallback(step);
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
			foreach (var ren in currentTransform.GetComponents<Renderer>())
				ren.enabled = enable;

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
