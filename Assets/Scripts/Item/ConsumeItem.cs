using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumeItem : CountableItem, IUsableItem
{
    private enum EConsumeItemType { Recovery, PowerUp }

    [SerializeField] private EConsumeItemType _csItemType;
    
    public ConsumeItem(ConsumeItemData data, int amount = 1)
        : base(data, amount) { }

    public void Use()
    {
        --Amount;

        switch (_csItemType)
        {
            case EConsumeItemType.Recovery:
                break;

            case EConsumeItemType.PowerUp:
                break;
        }

        Debug.Log("use " + _csItemType.ToString() + " item");
    }

    public override int GetTodayBuyingAmount()
    {
        return TodayBuyingAmount;
    }
}
