using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }
    private DataManager dataMgr;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);
    }

    private void Start()
    {
        dataMgr = DataManager.Instance;
    }

    public void OnClickHeroPick(PriceComponent priceComponent)
    {
        try
        {
            DataManager.Instance.gameData.SetGoods(
                priceComponent.GoodsType, -priceComponent.Price);

            for (int i = 0; i < priceComponent.Num; i++)
            {

            }
        }
        catch (Exception exp)
        {
            if (exp.Message.Contains("Insufficient"))
                Debug.LogError(priceComponent.GoodsType.ToString() + " 재화가 부족합니다.");

            return;
        }
    }

    public void OnClickBuyItem(PriceComponent priceComponent)
    {
        try
        {
            DataManager.Instance.gameData.SetGoods(
                priceComponent.GoodsType, -priceComponent.Price);

            for (int i = 0; i < priceComponent.Num; i++)
            {

            }
        }
        catch (Exception exp)
        {
            if (exp.Message.Contains("Insufficient"))
                Debug.LogError(priceComponent.GoodsType.ToString() + " 재화가 부족합니다.");

            return;
        }
    }
}