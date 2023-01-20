using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Consume Item Data",
    menuName = "Item Data/Consume Item", order = int.MaxValue)]
public class ConsumeItemData : CountableItemData
{
    [SerializeField] private float _value;
    public float Value => _value;

    public override Item CreateItem()
    {
        return new ConsumeItem(this);
    }
}