using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoodsText : MonoBehaviour
{
    [SerializeField] private GoodsType goodsType;
    public GoodsType GoodsType { get => goodsType; }

    private Text text;

    private void Awake()
    {
        text = GetComponentInChildren<Text>();
    }

    private void Start()
    {
        UIManager.Instance.UpdateGoodsUIAction += UpdateGoodsText;
    }

    public void UpdateGoodsText()
    {
        try
        {
            text.text = string.Format("{0:n0}",
                DataManager.Instance.GameData.GoodsList[(int)goodsType].count);
        }
        catch
        {
            
        }

    }
}