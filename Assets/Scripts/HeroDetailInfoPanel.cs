using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroDetailInfoPanel : MonoBehaviour
{
    [SerializeField] private HeroDetailInfo heroInfo;

    public void UpdateHeroInfo(UnitData HeroData)
    {
        heroInfo.nameTxt.text = HeroData.name;
        heroInfo.levelTxt.text = "Lv." + HeroData.Level.ToString();
        heroInfo.heroImg.sprite = HeroData.mySprite;
        //heroInfo.gradeImg.sprite = 

        heroInfo.dpsTxt.text = "전투력 : " + HeroData.DPS.ToString();
        heroInfo.hpTxt.text = HeroData.HP.ToString();
        heroInfo.criticalTxt.text = HeroData.Critical.ToString() + "%";
        heroInfo.apTxt.text = HeroData.Ap.ToString();
        heroInfo.dodgeTxt.text = HeroData.Dodge.ToString() + "%";
        heroInfo.dpTxt.text = HeroData.Dp.ToString();
        heroInfo.costTxt.text = HeroData.Cost.ToString();
    }
}