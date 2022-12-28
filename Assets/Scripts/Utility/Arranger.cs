using System.Collections.Generic;
using UnityEngine;

public class Arranger : MonoBehaviour
{
    [SerializeField] private EArrangerType myType;
    [SerializeField] private List<Transform> slotList = new List<Transform>();

    private void Start()
    {
        slotList.Clear();
        Invoke(nameof(UpdateSlot), 1.5f);
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

        if (myType == EArrangerType.Party)
        {
            for (int i = 0; i < slotList.Count; i++)
            {
                var heroSlot = slotList[i].GetComponent<HeroSlot>();

                if (heroSlot == null)
                    continue;
                
                // 슬롯에서 파티로 추가했을 경우
                if(!DataManager.Instance.IsContainsInParty(heroSlot.MyStatus.ID))
                {
                    UIManager.Instance.SwapSlotToParty(heroSlot.MyStatus.ID);

                    if (i == 0)
                        DataManager.Instance.HeroData.partyList.Insert(0, heroSlot.MyStatus);
                    else
                        DataManager.Instance.HeroData.partyList.Add(heroSlot.MyStatus);
                }

                if(i == 0)
                    heroSlot.MyStatus.SetLeader(true);
                else
                    heroSlot.MyStatus.SetLeader(false);

                heroSlot.UpdateSlotImage();
            }

            DataManager.Instance.UpdatePartyLeader();
        }
        else if(myType == EArrangerType.Hero)
        {
            for (int i = 0; i < slotList.Count; i++)
            {
                var heroSlot = slotList[i].GetComponent<HeroSlot>();

                if (heroSlot == null)
                    continue;

                // 만약 파티에 같은 정보의 영웅 슬롯이 존재한다면
                if (DataManager.Instance.IsContainsInParty(heroSlot.MyStatus.ID))
                {
                    heroSlot.MyStatus.SetLeader(false);
                    int partyIndex = DataManager.Instance.GetIndexOfHeroInParty(heroSlot.MyStatus);

                    UIManager.Instance.SwapPartyToSlot(heroSlot.MyStatus.ID);
                    DataManager.Instance.HeroData.partyList.RemoveAt(partyIndex);
                }

                heroSlot.UpdateSlotImage();
            }

            DataManager.Instance.UpdatePartyLeader();
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
        if(slotList[index1].name != "InvisibleSlot" && slotList[index2].name != "InvisibleSlot")
            DataManager.Instance.SwapPartyData(index1, index2);

        Central.SwapSlots(slotList[index1], slotList[index2]);

        UpdateSlot();
    }
}