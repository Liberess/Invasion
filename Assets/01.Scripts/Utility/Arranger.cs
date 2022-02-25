using System.Collections.Generic;
using UnityEngine;

public class Arranger : MonoBehaviour
{
    private enum ArrangerType
    {
        Party = 0,
        Hero,
        Item
    }

    [SerializeField] private ArrangerType myType;
    [SerializeField] private List<Transform> slotList = new List<Transform>();

    private void Start()
    {
        UpdateSlot();
    }

    public void UpdateSlot()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i == slotList.Count)
                slotList.Add(null);

            var slot = transform.GetChild(i);

            if (slot != slotList[i])
                slotList[i] = slot;
        }

        slotList.RemoveRange(transform.childCount, slotList.Count - transform.childCount);

        if (myType == ArrangerType.Party)
        {
            for (int i = 0; i < slotList.Count; i++)
            {
                var heroSlot = slotList[i].GetComponent<HeroSlot>();

                if (heroSlot == null)
                    continue;
                
                if(!DataManager.Instance.IsContainsInParty(heroSlot.MyStatus.ID))
                {
                    UIManager.Instance.SwapSlotToParty(heroSlot.MyStatus.ID);
                    DataManager.Instance.heroData.partyList.Add(heroSlot.MyStatus);
                }
            }
        }
        else if(myType == ArrangerType.Hero)
        {
            for (int i = 0; i < slotList.Count; i++)
            {
                var heroSlot = slotList[i].GetComponent<HeroSlot>();

                if (heroSlot == null)
                    continue;

                if (DataManager.Instance.IsContainsInParty(heroSlot.MyStatus.ID))
                {
                    UIManager.Instance.SwapPartyToSlot(heroSlot.MyStatus.ID);
                    int index = DataManager.Instance.GetIndexOfHeroInParty(heroSlot.MyStatus.ID);
                    DataManager.Instance.heroData.partyList.RemoveAt(index);
                }
            }
        }
    }

    public void InsertSlot(Transform slot, int index)
    {
        slot.SetParent(transform);
        slotList.Add(slot);
        slot.SetSiblingIndex(index);
        UpdateSlot();
    }

    public int GetIndexByPosition(Transform slot, int skipIndex = 1)
    {
        int result = 0;

        for (int i = 0; i < slotList.Count; i++)
        {
            if (slot.position.x < slotList[i].position.x)
                break;
            else if (skipIndex != i)
                result++;
        }

        return result;
    }

    public void SwapSlot(int index1, int index2)
    {
        Central.SwapSlots(slotList[index1], slotList[index2]);

        UpdateSlot();
    }
}