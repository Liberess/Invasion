using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CountableItem : Item
{
    public CountableItemData CountableData { get; private set; }

    public int Amount { get; protected set; } = 0;
    public int MaxAmount => CountableData.MaxAmount;
    public bool IsMax => Amount >= CountableData.MaxAmount;

    public int TodayBuyingAmount { get; protected set; } = 0;
    public int MaxBuyingAmount => CountableData.MaxBuyingAmount;

    public CountableItem(CountableItemData data, int amount = 1)
        : base(data)
    {
        CountableData = data;
        SetAmount(amount);
    }

    public void SetAmount(int amount)
    {
        Amount = Mathf.Clamp(amount, 0, MaxAmount);
    }

    public void AddAmount(int amount)
    {
        int newAmount = Amount + amount;
        SetAmount(newAmount);
    }

    public void SetTodayBuyingAmount(int amount)
    {
        TodayBuyingAmount = Mathf.Clamp(amount, 0, MaxBuyingAmount);
    }

    public void AddTodayBuyingAmount(int amount)
    {
        int newAmount = TodayBuyingAmount + amount;
        SetTodayBuyingAmount(newAmount);
    }
}
