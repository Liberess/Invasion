using System;
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
        try
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
                    if (!dataMgr.IsContainsInParty(heroSlot.HumalData.ID))
                    {
                        if (dataMgr.HumalData.humalDic.TryGetValue(heroSlot.HumalData.ID, out UnitData data))
                        {
                            data.SetParty(true);
       
                            dataMgr.HumalData.humalList.Remove(data);

                            if (i == 0)
                            {
                                data.SetLeader(true);
                                dataMgr.HumalData.partyList.Insert(0, heroSlot.HumalData);
                            }
                            else
                            {
                                data.SetLeader(false);
                                dataMgr.HumalData.partyList.Add(heroSlot.HumalData);
                            }
                        }
                    }

                    heroSlot.UpdateSlotImage();
                }

                dataMgr.UpdatePartyLeader();
            }
        }
        catch(Exception ex)
        {
            Debug.LogError(ex.Message);
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
        if (index1 < 0 || index1 >= slotList.Count || index2 < 0 || index2 >= slotList.Count)
        {
            Debug.LogWarning("Arranger::SwapSlot - index is not vaild " + index1 + ", " + index2);
            return;
        }
        
        if(slotList[index1].name != "InvisibleSlot" && slotList[index2].name != "InvisibleSlot")
            dataMgr.SwapPartyData(index1, index2);

        Central.SwapSlots(slotList[index1], slotList[index2]);

        UpdateSlot();
    }
}