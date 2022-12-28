using UnityEngine;

[System.Serializable]
public class PriceComponent : MonoBehaviour
{
    [SerializeField] private EBuyingType buyingType;
    public EBuyingType BuyingType { get => buyingType; }

    [SerializeField] private EGoodsType payGoodsType;
    public EGoodsType PayGoodsType { get => payGoodsType; }

    [SerializeField] private int price;
    public int Price { get => price; }

    [SerializeField] private int num;
    public int Num { get => num; }
}