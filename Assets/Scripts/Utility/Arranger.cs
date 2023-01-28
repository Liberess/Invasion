using System.Collections.Generic;
using UnityEngine;

public class Arranger : MonoBehaviour
{
    private DataManager dataMgr;

    [SerializeField] private EArrangerType myType;
    [SerializeField] private List<Transform> slotList = new List<Transform>();

    private void Start()
    {
        dataMgr = DataManager.Instance;

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
                if(!dataMgr.IsContainsInParty(heroSlot.HumalData.ID))
                {
                    UIManager.Instance.SwapSlotToParty(heroSlot.HumalData.ID);

                    if (i == 0)
                        dataMgr.HumalData.partyList.Insert(0, heroSlot.HumalData);
                    else
                        dataMgr.HumalData.partyList.Add(heroSlot.HumalData);
                }

                if(i == 0)
                    heroSlot.HumalData.SetLeader(true);
                else
                    heroSlot.HumalData.SetLeader(false);

                heroSlot.UpdateSlotImage();
            }

            dataMgr.UpdatePartyLeader();
        }
        else if(myType == EArrangerType.Hero)
        {
            for (int i = 0; i < slotList.Count; i++)
            {
                var heroSlot = slotList[i].GetComponent<HeroSlot>();

                if (heroSlot == null)
                    continue;

                // 만약 파티에 같은 정보의 영웅 슬롯이 존재한다면
                if (dataMgr.IsContainsInParty(heroSlot.HumalData.ID))
                {
                    heroSlot.HumalData.SetLeader(false);
                    int partyIndex = dataMgr.GetIndexOfHumalInParty(heroSlot.HumalData);

                    UIManager.Instance.SwapPartyToSlot(heroSlot.HumalData.ID);
                    dataMgr.HumalData.partyList.RemoveAt(partyIndex);
                }

                heroSlot.UpdateSlotImage();
            }

            dataMgr.UpdatePartyLeader();
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
            dataMgr.SwapPartyData(index1, index2);

        Central.SwapSlots(slotList[index1], slotList[index2]);

        UpdateSlot();
    }
}