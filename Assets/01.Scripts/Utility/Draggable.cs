using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    private Transform root;

    [SerializeField] private bool canDrag = false;
    [SerializeField] private bool isPressed = false;
    [SerializeField] private float pressTime = 0f;
    private float maxPressTime = 1.5f;

    private void Start()
    {
        root = transform.root;
        maxPressTime = 1.5f;
    }

    private void Update()
    {
        if(isPressed)
        {
            if(pressTime >= maxPressTime)
            {
                canDrag = true;
                isPressed = false;
            }
            else
            {
                pressTime += Time.deltaTime;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        canDrag = false;
        isPressed = true;
        pressTime = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        pressTime = 0f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPressed = false;
        pressTime = 0f;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        root.BroadcastMessage("BeginDrag", transform, SendMessageOptions.DontRequireReceiver);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag)
            return;

        transform.position = eventData.position;
        root.BroadcastMessage("Drag", transform, SendMessageOptions.DontRequireReceiver);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isPressed = false;
        root.BroadcastMessage("EndDrag", transform, SendMessageOptions.DontRequireReceiver);
    }
}