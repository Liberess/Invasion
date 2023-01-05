using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Facility Data",
    menuName = "Scriptable Object/Facility Data", order = int.MaxValue)]
public class FacilityData : ScriptableObject
{
    [SerializeField] private int m_ID;
    public int ID => m_ID;

    [SerializeField] private string m_name;
    public string Name => m_name;

    [SerializeField] private ECurrencyType m_makingType;
    public ECurrencyType MakingGoodsType => m_makingType;

    [SerializeField] private int m_makingProgressTime;
    public int MakingProgressTime => m_makingProgressTime;

    [SerializeField] private Sprite m_facilitySprite;
    public Sprite FacilitySprite => m_facilitySprite;
}