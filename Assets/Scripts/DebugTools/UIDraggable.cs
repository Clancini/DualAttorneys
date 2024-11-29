using UnityEngine;
using UnityEngine.EventSystems;

public class UIDraggable : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField] GameObject containerToDrag;

    Vector3 distanceFromMouse;

    public void OnDrag(PointerEventData eventData)
    {
        if (!eventData.lastPress != gameObject)
            return;

        containerToDrag.transform.position = Input.mousePosition + distanceFromMouse;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        distanceFromMouse = containerToDrag.transform.position - Input.mousePosition;
    }
}
