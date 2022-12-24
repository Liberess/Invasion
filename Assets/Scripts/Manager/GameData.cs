﻿using System;
using System.Collections.Generic;
using System.Numerics;
using JetBrains.Annotations;
using UnityEngine;

public enum GoodsType
{
    Stamina = 0,
    Gold,
    Dia,
    AwakeJewel      //각성석
    //EvolutionJewel  //진화석
}

[System.Serializable]
public class Goods
{
    public string name;
    public int count;

    public Goods(string _name, int _count)
    {
        name = _name;
        count = _count;
    }
}

[System.Serializable]
public class GameData
{
    public string saveTimeStr;
    public DateTime saveTime;

    public StageInfo stageInfo;
    public Dictionary<string, string> stageNameDic = new Dictionary<string, string>();

    public int[] facilGold = new int[7];
    public int[] facilLevelList = new int[7];
    public bool[] facilUnlockList = new bool[7];
    public float[] facilLimitTime = new float[7];
    public float[] facilSliderTime = new float[7];

    [SerializeField] private List<Goods> goodsList = new List<Goods>();

    public List<Goods> GoodsList
    {
        get => goodsList;
        
        set
        {
            goodsList = value;
            
            if (UIManager.Instance)
                UIManager.Instance.UpdateGoodsUIAction();
        }
    }

    public void SetElementInGoodsList(int goodsIndex, int value)
    {
        goodsList[goodsIndex].count = value;
        
        if (UIManager.Instance)
            UIManager.Instance.UpdateGoodsUIAction();
    }
    
    public void AddElementInGoodsList(int goodsIndex, int value)
    {
        goodsList[goodsIndex].count += value;
        
        if (UIManager.Instance)
            UIManager.Instance.UpdateGoodsUIAction();
    }
    
    public List<Sprite> goodsSpriteList = new List<Sprite>();
    [HideInInspector] public string[] goodsNames =
        { "Stamina", "Gold", "Dia", "Awake Jewel" };

    public float bgm;
    public float sfx;

    public bool isNew;
}