using UnityEngine;
using UnityEngine.EventSystems;

namespace UFZ.UI
{
    // Works but can be improved, maybe
    // https://docs.unity3d.com/ScriptReference/EventSystems.IDragHandler.html
    public class PanelDragBehavior : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        public Transform DragObject;
        private Vector3 lastPointerPos;

        public void Start()
        {
            if (DragObject == null)
                DragObject = transform;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            // Init pointer position
            lastPointerPos = Core.HasDevice("Flystick")
                ? transform.InverseTransformPoint(FindObjectOfType<WandInputModule>().cursor.position)
                : UFZ.Core.Position();
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 currentPointerPos;
            currentPointerPos = Core.HasDevice("Flystick")
                ? transform.InverseTransformPoint(FindObjectOfType<WandInputModule>().cursor.position)
                : Core.Position();
            var deltaPointerPos = currentPointerPos - lastPointerPos;
            DragObject.localPosition += deltaPointerPos;
            lastPointerPos = currentPointerPos;
            // Take movement of UI into account for last pointer position
            if (Core.HasDevice("Flystick"))
                lastPointerPos -= deltaPointerPos;
        }
    }
}
