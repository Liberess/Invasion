using UnityEngine;

[System.Serializable]
public class PriceComponent : MonoBehaviour
{
    [SerializeField] private EGoodsType goodsType;
    public EGoodsType GoodsType { get => goodsType; }

    [SerializeField] private int price;
    public int Price { get => price; }

    [SerializeField] private int num;
    public int Num { get => num; }
}