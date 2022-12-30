using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    [SerializeField] private int _id;
    public int ID => _id;

    [SerializeField] private string _name;
    public string Name => _name;

    [SerializeField] private EItemType _itemType;
    public EItemType ItemType => _itemType;

    [SerializeField] private int _weight;
    public int Weight => _weight;

    [SerializeField] private int _maxBuyingAmount;
    public int MaxBuyingAmount => _maxBuyingAmount;

    [SerializeField] private string _description;
    public string Description => _description;

    [SerializeField] private Sprite _iconSprite;
    public Sprite IconSprite => _iconSprite;

    public abstract Item CreateItem();
}