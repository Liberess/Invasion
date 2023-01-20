using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField, Range(0.5f, 3.0f)] private float maxPressTime = 1.5f;
    private float pressTime = 0f;
    private bool isPressed = false;
    private bool canDrag = false;
    public bool CanDrag { get => canDrag; }

    private Transform root;

    private void Start()
    {
        root = transform.root;
    }

    private void Update()
    {
        if(isPressed)
        {
            if(pressTime >= maxPressTime)
            {
                var heroSlot = GetComponent<HeroSlot>();

                if (heroSlot != null)
                    heroSlot.SlotDragEvent();

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