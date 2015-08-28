using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// From https://forums.oculus.com/viewtopic.php?t=16710
public class WandInputModule : BaseInputModule
{
	// singleton makes it easy to access the instanced fields from other code without needing a pointer
	// e.g.  if (WandInputModule.singleton != null && WandInputModule.singleton.controlAxisUsed) ...
	private static WandInputModule _singleton;
	public static WandInputModule singleton
	{
		get
		{
			return _singleton;
		}
	}

	// smooth axis - default UI move handlers do things in steps, meaning you can smooth scroll a slider or scrollbar
	// with axis control. This option allows setting value of scrollbar/slider directly as opposed to using move handler
	// to avoid this
	public bool useSmoothAxis = true;
	// multiplier controls how fast slider/scrollbar moves with respect to input axis value
	public float smoothAxisMultiplier = 0.01f;
	// if useSmoothAxis is off, this next field controls how many steps per second are done when axis is on
	public float steppedAxisStepsPerSecond = 10f;

	// guiRaycastHit is helpful if you have other places you want to use look input outside of UI system
	// you can use this to tell if the UI raycaster hit a UI element
	private bool _guiRaycastHit;
	public bool guiRaycastHit
	{
		get
		{
			return _guiRaycastHit;
		}
	}

	// controlAxisUsed is helpful if you use same axis elsewhere
	// you can use this boolean to see if the UI used the axis control or not
	// if something is selected and takes move event, then this will be set
	private bool _controlAxisUsed;
	public bool controlAxisUsed
	{
		get
		{
			return _controlAxisUsed;
		}
	}

	// buttonUsed is helpful if you use same button elsewhere
	// you can use this boolean to see if the UI used the button press or not
	private bool _buttonUsed;
	public bool buttonUsed
	{
		get
		{
			return _buttonUsed;
		}
	}

	public enum Mode { Pointer, Submit };
	// WandInputModule supports 2 modes:
	// 1 - Pointer
	//     Module acts a lot like a mouse with pointer locked where you look. Where you look is where
	//     pointerDown/pointerUp/pointerClick events are used
	//     useCursor is recommended for correct precision
	//     axis control of sliders/scrollbars/etc. is optional
	// 2 - Submit
	//     controls are selected and manipulated with axis control only
	//     submit/select events are used
	//     in this mode you can't click along a slider/scrollbar to set the slider/scroll value
	//     useLookDrag option is ignored
	public Mode mode = Mode.Pointer;

	// useLookDrag allows you to use look-based drag and drop (see example)
	// and also drag sliders/scrollbars based on where you are looking
	// only works if usePointerMethod is true
	public bool useLookDrag = true;
	public bool useLookDragSlider = true;
	public bool useLookDragScrollbar = false;

	// useCursor only applies when usePointerMethod is true
	// the cursor works like a mouse pointer so you can see exactly where you are clicking
	// not recommended to turn off
	public bool useCursor = true;
	public float normalCursorScale = 0.0005f;
	public bool scaleCursorWithDistance = true;

	// the UI element to use for the cursor
	// the cursor will appear on the plane of the current UI element being looked at - so it adjusts to depth correctly
	// recommended to use a simple Image component (typical mouse cursor works pretty well) and you MUST add the
	// Unity created IgnoreRaycast component (script included in example) so that the cursor will not be see by the UI
	// event system
	public RectTransform cursor;

	// when UI element is selected this is the color it gets
	// useful for when want to use axis input to control sliders/scrollbars so you can see what is being
	// manipulated
	public bool useSelectColor = true;
	public bool useSelectColorOnButton = false;
	public bool useSelectColorOnToggle = false;
	public Color selectColor = Color.blue;

	// ignore input when looking away from all UI elements
	// useful if you want to use buttons/axis for other controls
	public bool ignoreInputsWhenLookAway = true;

	// deselect when looking away from all UI elements
	// useful if you want to use axis for other controls
	public bool deselectWhenLookAway = true;

	// interal vars
	private PointerEventData lookData;
	private Color currentSelectedNormalColor;
	private bool currentSelectedNormalColorValid;
	private Color currentSelectedHighlightedColor;
	private GameObject currentLook;
	private GameObject currentPressed;
	private GameObject currentDragging;
	private float nextAxisActionTime;

	// use screen midpoint as locked pointer location, enabling look location to be the "mouse"
	private PointerEventData GetLookPointerEventData()
	{
		Vector2 lookPosition;
		lookPosition.x = Screen.width / 2f;
		lookPosition.y = Screen.height / 2f;
		if (lookData == null)
			lookData = new PointerEventData(eventSystem);

		lookData.Reset();
		lookData.delta = Vector2.zero;
		lookData.position = lookPosition;
		lookData.scrollDelta = Vector2.zero;
		eventSystem.RaycastAll(lookData, m_RaycastResultCache);
		lookData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
		_guiRaycastHit = lookData.pointerCurrentRaycast.gameObject != null;
		m_RaycastResultCache.Clear();
		return lookData;
	}

	// update the cursor location and whether it is enabled
	// this code is based on Unity's DragMe.cs code provided in the UI drag and drop example
	private void UpdateCursor(PointerEventData lookDataLocal)
	{
		if (cursor == null)
			return;

		if (useCursor)
		{
			if (lookDataLocal.pointerEnter != null)
			{
				var draggingPlane = lookDataLocal.pointerEnter.GetComponent<RectTransform>();
				Vector3 globalLookPos;
				if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, lookDataLocal.position, lookDataLocal.enterEventCamera, out globalLookPos))
				{
					cursor.gameObject.SetActive(true);
					cursor.position = globalLookPos;
					cursor.rotation = draggingPlane.rotation;
					if (!scaleCursorWithDistance)
						return;
					// scale cursor with distance
					var lookPointDistance = (globalLookPos - lookDataLocal.enterEventCamera.transform.position).magnitude;
					var cursorScale = lookPointDistance * normalCursorScale;
					if (cursorScale < normalCursorScale)
					{
						cursorScale = normalCursorScale;
					}
					Vector3 cursorScaleVector;
					cursorScaleVector.x = cursorScale;
					cursorScaleVector.y = cursorScale;
					cursorScaleVector.z = cursorScale;
					cursor.localScale = cursorScaleVector;
				}
				else
				{
					cursor.gameObject.SetActive(false);
				}
			}
			else
			{
				cursor.gameObject.SetActive(false);
			}
		}
		else
		{
			cursor.gameObject.SetActive(false);
		}
	}

	// sets color of selected UI element and saves current color so it can be restored on deselect
	private void SetSelectedColor(GameObject go)
	{
		if (!useSelectColor)
			return;

		if (!useSelectColorOnButton && go.GetComponent<Button>())
		{
			currentSelectedNormalColorValid = false;
			return;
		}
		if (!useSelectColorOnToggle && go.GetComponent<Toggle>())
		{
			currentSelectedNormalColorValid = false;
			return;
		}
		var s = go.GetComponent<Selectable>();
		if (s == null)
			return;

		var cb = s.colors;
		currentSelectedNormalColor = cb.normalColor;
		currentSelectedNormalColorValid = true;
		currentSelectedHighlightedColor = cb.highlightedColor;
		cb.normalColor = selectColor;
		cb.highlightedColor = selectColor;
		s.colors = cb;
	}

	// restore color of previously selected UI element
	private void RestoreColor(GameObject go)
	{
		if (!useSelectColor || !currentSelectedNormalColorValid)
			return;

		var s = go.GetComponent<Selectable>();
		if (s == null)
			return;

		var cb = s.colors;
		cb.normalColor = currentSelectedNormalColor;
		cb.highlightedColor = currentSelectedHighlightedColor;
		s.colors = cb;
	}

	// clear the current selection
	public void ClearSelection()
	{
		if (!eventSystem.currentSelectedGameObject)
			return;

		RestoreColor(eventSystem.currentSelectedGameObject);
		eventSystem.SetSelectedGameObject(null);
	}

	// select a game object
	private void Select(GameObject go)
	{
		ClearSelection();
		if (!ExecuteEvents.GetEventHandler<ISelectHandler>(go)) return;

		SetSelectedColor(go);
		eventSystem.SetSelectedGameObject(go);
	}

	// send update event to selected object
	// needed for InputField to receive keyboard input
	private bool SendUpdateEventToSelectedObject()
	{
		if (eventSystem.currentSelectedGameObject == null)
			return false;
		var data = GetBaseEventData();
		ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
		return data.used;
	}

	// Process is called by UI system to process events
	public override void Process()
	{
		_singleton = this;

		// send update events if there is a selected object - this is important for InputField to receive keyboard events
		SendUpdateEventToSelectedObject();

		// see if there is a UI element that is currently being looked at
		var lookDataLocal = GetLookPointerEventData();
		currentLook = lookDataLocal.pointerCurrentRaycast.gameObject;

		// deselect when look away
		if (deselectWhenLookAway && currentLook == null)
			ClearSelection();

		// handle enter and exit events (highlight)
		// using the function that is already defined in BaseInputModule
		HandlePointerExitAndEnter(lookDataLocal, currentLook);

		// update cursor
		UpdateCursor(lookDataLocal);

		if (!ignoreInputsWhenLookAway || ignoreInputsWhenLookAway && currentLook != null)
		{
			// button down handling
			_buttonUsed = false;
			if (UFZ.IOC.Core.Instance.Input.IsOkButtonPressed())
			{
				ClearSelection();
				lookDataLocal.pressPosition = lookDataLocal.position;
				lookDataLocal.pointerPressRaycast = lookDataLocal.pointerCurrentRaycast;
				lookDataLocal.pointerPress = null;
				if (currentLook != null)
				{
					currentPressed = currentLook;
					GameObject newPressed = null;
					switch (mode)
					{
						case Mode.Pointer:
							newPressed = ExecuteEvents.ExecuteHierarchy(currentPressed, lookDataLocal, ExecuteEvents.pointerDownHandler);
							if (newPressed == null)
							{
								// some UI elements might only have click handler and not pointer down handler
								newPressed = ExecuteEvents.ExecuteHierarchy(currentPressed, lookDataLocal, ExecuteEvents.pointerClickHandler);
								if (newPressed != null)
								{
									currentPressed = newPressed;
								}
							}
							else
							{
								currentPressed = newPressed;
								// we want to do click on button down at same time, unlike regular mouse processing
								// which does click when mouse goes up over same object it went down on
								// reason to do this is head tracking might be jittery and this makes it easier to click buttons
								ExecuteEvents.Execute(newPressed, lookDataLocal, ExecuteEvents.pointerClickHandler);
							}
							break;
						case Mode.Submit:
							newPressed = ExecuteEvents.ExecuteHierarchy(currentPressed, lookDataLocal, ExecuteEvents.submitHandler);
							if (newPressed == null)
							{
								// try select handler instead
								newPressed = ExecuteEvents.ExecuteHierarchy(currentPressed, lookDataLocal, ExecuteEvents.selectHandler);
							}
							break;
					}
					if (newPressed != null)
					{
						lookDataLocal.pointerPress = newPressed;
						currentPressed = newPressed;
						Select(currentPressed);
						_buttonUsed = true;
					}
					if (mode == Mode.Pointer)
					{
						if (useLookDrag)
						{
							var useLookTest = true;
							if (!useLookDragSlider && currentPressed.GetComponent<Slider>())
							{
								useLookTest = false;
							}
							else if (!useLookDragScrollbar && currentPressed.GetComponent<Scrollbar>())
							{
								useLookTest = false;
								// the following is for scrollbars to work right
								// apparently they go into an odd drag mode when pointerDownHandler is called
								// a begin/end drag fixes that
								if (ExecuteEvents.Execute(currentPressed, lookDataLocal, ExecuteEvents.beginDragHandler))
								{
									ExecuteEvents.Execute(currentPressed, lookDataLocal, ExecuteEvents.endDragHandler);
								}
							}
							if (useLookTest)
							{
								ExecuteEvents.Execute(currentPressed, lookDataLocal, ExecuteEvents.beginDragHandler);
								lookDataLocal.pointerDrag = currentPressed;
								currentDragging = currentPressed;
							}
						}
						else if (currentPressed.GetComponent<Scrollbar>())
						{
							// the following is for scrollbars to work right
							// apparently they go into an odd drag mode when pointerDownHandler is called
							// a begin/end drag fixes that
							if (ExecuteEvents.Execute(currentPressed, lookDataLocal, ExecuteEvents.beginDragHandler))
							{
								ExecuteEvents.Execute(currentPressed, lookDataLocal, ExecuteEvents.endDragHandler);
							}
						}
					}
				}
			}
		}

		// have to handle button up even if looking away
		if (UFZ.IOC.Core.Instance.Input.WasOkButtonPressed())
		{
			if (currentDragging)
			{
				ExecuteEvents.Execute(currentDragging, lookDataLocal, ExecuteEvents.endDragHandler);
				if (currentLook != null)
				{
					ExecuteEvents.ExecuteHierarchy(currentLook, lookDataLocal, ExecuteEvents.dropHandler);
				}
				lookDataLocal.pointerDrag = null;
				currentDragging = null;
			}
			if (currentPressed)
			{
				ExecuteEvents.Execute(currentPressed, lookDataLocal, ExecuteEvents.pointerUpHandler);
				lookDataLocal.rawPointerPress = null;
				lookDataLocal.pointerPress = null;
				currentPressed = null;
			}
		}

		// drag handling
		if (currentDragging != null)
		{
			ExecuteEvents.Execute(currentDragging, lookDataLocal, ExecuteEvents.dragHandler);
		}

		if (ignoreInputsWhenLookAway && (!ignoreInputsWhenLookAway || currentLook == null))
			return;

		// control axis handling
		_controlAxisUsed = false;
		if (!eventSystem.currentSelectedGameObject)
			return;

		var newVal = UFZ.IOC.Core.Instance.Input.GetHorizontalAxis();
		if (!(newVal > 0.01f) && !(newVal < -0.01f))
			return;

		if (useSmoothAxis)
		{
			var sl = eventSystem.currentSelectedGameObject.GetComponent<Slider>();
			if (sl != null)
			{
				var mult = sl.maxValue - sl.minValue;
				sl.value += newVal * smoothAxisMultiplier * mult;
				_controlAxisUsed = true;
			}
			else
			{
				var sb = eventSystem.currentSelectedGameObject.GetComponent<Scrollbar>();
				if (sb == null)
					return;

				sb.value += newVal * smoothAxisMultiplier;
				_controlAxisUsed = true;
			}
		}
		else
		{
			_controlAxisUsed = true;
			var time = UFZ.IOC.Core.Instance.Time.Time(); // Time.unscaledTime
			if (!(time > nextAxisActionTime))
				return;

			nextAxisActionTime = time + 1f / steppedAxisStepsPerSecond;
			var axisData = GetAxisEventData(newVal, 0.0f, 0.0f);
			if (!ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisData, ExecuteEvents.moveHandler))
				_controlAxisUsed = false;
		}
	}
}
