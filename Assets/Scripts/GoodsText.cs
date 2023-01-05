using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoodsText : MonoBehaviour
{
    [SerializeField] private ECurrencyType currencyType;
    public ECurrencyType CurrencyType { get => currencyType; }

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
                DataManager.Instance.GameData.GoodsList[(int)currencyType].count);
        }
        catch
        {
            
        }

    }
}