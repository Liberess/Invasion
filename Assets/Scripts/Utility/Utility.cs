using System;
using System.Linq;
using System.Collections.Generic;

public static class Utility
{
    public static bool GetThisChanceResult(float Chance)
    {
        if (Chance < 0.0000001f)
            Chance = 0.0000001f;

        bool Success = false;
        int RandAccuracy = 10000000;
        float RandHitRange = Chance * RandAccuracy;
        int Rand = UnityEngine.Random.Range(1, RandAccuracy + 1);
        if (Rand <= RandHitRange)
            Success = true;

        return Success;
    }

    public static bool GetThisChanceResult_Percentage(float Percentage_Chance)
    {
        if (Percentage_Chance < 0.0000001f)
            Percentage_Chance = 0.0000001f;

        Percentage_Chance = Percentage_Chance / 100;

        bool Success = false;

        int RandAccuracy = 10000000;
        float RandHitRange = Percentage_Chance * RandAccuracy;
        int Rand = UnityEngine.Random.Range(1, RandAccuracy + 1);

        if (Rand <= RandHitRange)
            Success = true;

        return Success;
    }

    public static void SwapListElement<T> (this List<T> list, int from, int to)
    {
        T temp = list[from];
        list[from] = list[to];
        list[to] = temp;
    }

    public static int FindIndexOf<T> (this List<T> list, T element)
    {
        return list.IndexOf(element);
    }

    public static bool DownCastingItem<T> (Item item, out T childItem) where T : Item
    {
        if(item is T)
        {
            childItem = (T)item;
            return true;
        }
        else
        {
            childItem = default;
            return false;
        }
    }

    public static string ConcatCurrency(int value, int goalValue)
    {
        return string.Concat($"{value:n0}", "/", $"{goalValue:n0}");
    }
}