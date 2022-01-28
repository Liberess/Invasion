using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Central : MonoBehaviour
{
    [SerializeField] private Transform invisibleSlot;
    [SerializeField] private List<Arranger> arrangerList = new List<Arranger>();

    private void Start()
    {
        var arrs = transform.GetComponentsInChildren<Arranger>();

        for (int i = 0; i < arrs.Length; i++)
            arrangerList.Add(arrs[i]);
    }

    private void SwapSlotsInHierarchy(Transform src, Transform dest)
    {
        Transform srcParent = src.parent;
        Transform destParent = dest.parent;

        int srcIndex = src.GetSiblingIndex();
        int destIndex = dest.GetSiblingIndex();

        src.SetParent(destParent);
        src.SetSiblingIndex(destIndex);

        dest.SetParent(srcParent);
        dest.SetSiblingIndex(srcIndex);

        arrangerList.ForEach(t => t.UpdateSlot());
    }

    /// <summary>
    /// 타겟이 RectTransform안에 있는지 확인한다.
    /// </summary>
    /// <param name="rt"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    private bool ContainPosition(RectTransform rt, Vector2 pos)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rt, pos);
    }

    private void BeginDrag(Transform card)
    {
        SwapSlotsInHierarchy(invisibleSlot, card);
    }

    private void Drag(Transform card)
    {
        var whichArrangerSlot = arrangerList.Find(
            t => ContainPosition(t.transform as RectTransform, card.position));
    }

    private void EndDrag(Transform card)
    {
        SwapSlotsInHierarchy(invisibleSlot, card);
    }
}