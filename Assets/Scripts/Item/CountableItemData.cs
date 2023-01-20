using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CountableItemData : ItemData
{
    [SerializeField] private int _maxAmount = 999;
    public int MaxAmount => _maxAmount;
}
