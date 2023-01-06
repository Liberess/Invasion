using System;
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
        UIManager.Instance.UpdateCurrencyUIActionList[(int)currencyType] += (v) => UpdateGoodsText(v);
    }

    public void UpdateGoodsText(int value)
    {
        text.text = string.Format("{0:n0}", value);
    }
}