using System;
using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string saveTimeStr;
    public DateTime saveTime;

    public int[] facilLevelList = new int[7];
    public bool[] facilUnlockList = new bool[7];
    public float[] facilLimitTime = new float[7];
    public float[] facilSliderTime = new float[7];

    public int dia;
    public string strGold;
    public string strSoulGem;
    public string strDrink;

    public BigInteger gold;
    public BigInteger soulGem;
    public BigInteger drink;
    public BigInteger maxSoul = BigInteger.Pow(10, (int)Unit.Max * 3);

    public string[] strFacilGold = new string[(int)Size.max];
    public BigInteger[] facilGold = new BigInteger[7];
    public int[,] facilGoldUnit = new int[7, (int)Unit.Max];

    public int[] goldUnit = new int[(int)Unit.Max];
    public int[] soulUnit = new int[(int)Unit.Max];

    public string[] strMoneyUnit =
        {
            "", "a", "b", "c", "d", "e", "f", "g", "h", "i",
            "j", "k", "l", "m", "n", "o", "p", "q", "r",
            "s", "t", "u", "v", "w", "x", "y", "z"
        };

    public float bgm;
    public float sfx;

    public bool isNew;
    public bool isClear;
}

public enum Size
{
    max = 7
}

public enum Unit
{
    a = 0, b, c, d, e, f, g,
    h, i, j, k, l, m, n,
    o, p, q, r, s, t, u,
    v, w, x, y, z, Max
}