using System.Collections.Generic;
using UnityEngine;

public class Arranger : MonoBehaviour
{
    List<Transform> slotList = new List<Transform>();

    private void Start()
    {
        UpdateSlot();
    }

    public void UpdateSlot()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            if (i == slotList.Count)
                slotList.Add(null);

            var slot = transform.GetChild(i);

            if (slot != slotList[i])
                slotList[i] = slot;
        }

        slotList.RemoveRange(transform.childCount, slotList.Count - transform.childCount);
    }

    public int GetIndexByPosition(Transform slot, int skipIndex = 1)
    {
        int result = 0;

        for(int i = 0; i < slotList.Count; i++)
        {
            if (slot.position.x < slotList[i].position.x)
                break;
            else if (skipIndex != i)
                result++;
        }

        return result;
    }
}