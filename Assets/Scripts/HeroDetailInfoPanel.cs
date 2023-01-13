using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroDetailInfoPanel : MonoBehaviour
{
    [SerializeField] private HeroDetailInfo heroInfo;

    public void UpdateHeroInfo(UnitData HeroData)
    {
        heroInfo.nameTxt.text = HeroData.KoName;
        heroInfo.levelTxt.text = string.Concat("Lv.", HeroData.Level);
        heroInfo.heroImg.sprite = HeroData.sprite;
        //heroInfo.gradeImg.sprite = 

        heroInfo.dpsTxt.text = string.Concat("전투력 : ", HeroData.DPS);
        heroInfo.hpTxt.text = HeroData.HP.ToString();
        heroInfo.criticalTxt.text = string.Concat(HeroData.Critical, "%");
        heroInfo.apTxt.text = HeroData.Ap.ToString();
        heroInfo.dodgeTxt.text = string.Concat(HeroData.Dodge, "%");
        heroInfo.dpTxt.text = HeroData.Dp.ToString();
        heroInfo.costTxt.text = HeroData.Cost.ToString();

        heroInfo.pieceTxt.text = DataManager.Instance.GetHumalPieceAmount(HeroData.KoName).ToString();
    }
}