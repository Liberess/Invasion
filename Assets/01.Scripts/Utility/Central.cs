using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Central : MonoBehaviour
{
    [SerializeField] private Transform invisibleSlot;
    [SerializeField] private List<Arranger> arrangerList = new List<Arranger>();

    private int originIndex;
    [SerializeField] private Arranger workingArranger;

    private void Start()
    {
        var arrs = transform.GetComponentsInChildren<Arranger>();

        for (int i = 0; i < arrs.Length; i++)
            arrangerList.Add(arrs[i]);
    }

    public static void SwapSlots(Transform src, Transform dest)
    {
        Transform srcParent = src.parent;
        Transform destParent = dest.parent;

        int srcIndex = src.GetSiblingIndex();
        int destIndex = dest.GetSiblingIndex();

        src.SetParent(destParent);
        src.SetSiblingIndex(destIndex);

        dest.SetParent(srcParent);
        dest.SetSiblingIndex(srcIndex);
    }

    private void SwapSlotsInHierarchy(Transform src, Transform dest)
    {
        SwapSlots(src, dest);

        arrangerList.ForEach(t => t.UpdateSlot());
    }

    /// <summary>
    /// 타겟이 RectTransform안에 있는지 확인한다.
    /// </summary>
    private bool ContainPosition(RectTransform rt, Vector2 pos)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rt, pos);
    }

    private void BeginDrag(Transform slot)
    {
        workingArranger = arrangerList.Find(
            t =>ContainPosition(t.transform as RectTransform, slot.position));

        originIndex = slot.GetSiblingIndex();

        SwapSlotsInHierarchy(invisibleSlot, slot);
    }

    private void Drag(Transform slot)
    {
        var whichArrangerSlot = arrangerList.Find(
            t => ContainPosition(t.transform as RectTransform, slot.position));

        if(whichArrangerSlot == null)
        {
            invisibleSlot.SetParent(transform);
            bool updateChildren = transform != invisibleSlot.parent;

            if(updateChildren)
                arrangerList.ForEach(t => t.UpdateSlot());
        }
        else
        {
            bool insert = invisibleSlot.parent == transform;

            if(insert)
            {
                int index = whichArrangerSlot.GetIndexByPosition(slot);

                invisibleSlot.SetParent(whichArrangerSlot.transform);
                whichArrangerSlot.InsertSlot(invisibleSlot, index);
            }
            else
            {
                int invisibleSlotIndex = invisibleSlot.GetSiblingIndex();
                int targetIndex = whichArrangerSlot.GetIndexByPosition(slot, invisibleSlotIndex);

                if (invisibleSlotIndex != targetIndex)
                    whichArrangerSlot.SwapSlot(invisibleSlotIndex, targetIndex);
            }
        }
    }

    private void EndDrag(Transform slot)
    {
        if(invisibleSlot.parent == transform)
        {
            workingArranger.InsertSlot(slot, originIndex);
            workingArranger = null;
            originIndex = -1;
        }
        else
        {
            SwapSlotsInHierarchy(invisibleSlot, slot);
        }
    }
}