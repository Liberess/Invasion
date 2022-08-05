using UnityEngine;

[System.Serializable]
public class PriceComponent : MonoBehaviour
{
    [SerializeField] private GoodsType goodsType;
    public GoodsType GoodsType { get => goodsType; }

    [SerializeField] private int price;
    public int Price { get => price; }

    [SerializeField] private int num;
    public int Num { get => num; }
}