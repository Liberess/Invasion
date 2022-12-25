using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroDetailInfoPanel : MonoBehaviour
{
    [SerializeField] private HeroDetailInfo heroInfo;

    public void UpdateHeroInfo(UnitData HeroData)
    {
        heroInfo.nameTxt.text = HeroData.name;
        heroInfo.levelTxt.text = "Lv." + HeroData.level.ToString();
        heroInfo.heroImg.sprite = HeroData.mySprite;
        //heroInfo.gradeImg.sprite = 

        heroInfo.dpsTxt.text = "전투력 : " + HeroData.DPS.ToString();
        heroInfo.hpTxt.text = HeroData.hp.ToString();
        heroInfo.criticalTxt.text = HeroData.critical.ToString() + "%";
        heroInfo.apTxt.text = HeroData.ap.ToString();
        heroInfo.dodgeTxt.text = HeroData.dodge.ToString() + "%";
        heroInfo.dpTxt.text = HeroData.dp.ToString();
        heroInfo.costTxt.text = HeroData.cost.ToString();
    }
}