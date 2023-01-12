using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroDetailInfoPanel : MonoBehaviour
{
    [SerializeField] private HeroDetailInfo heroInfo;

    public void UpdateHeroInfo(HumalData HeroData)
    {
        heroInfo.nameTxt.text = HeroData.Data.KoName;
        heroInfo.levelTxt.text = string.Concat("Lv.", HeroData.Data.Level);
        heroInfo.heroImg.sprite = HeroData.sprite;
        //heroInfo.gradeImg.sprite = 

        heroInfo.dpsTxt.text = string.Concat("전투력 : ", HeroData.DPS);
        heroInfo.hpTxt.text = HeroData.Data.HP.ToString();
        heroInfo.criticalTxt.text = string.Concat(HeroData.Data.Critical, "%");
        heroInfo.apTxt.text = HeroData.Data.Ap.ToString();
        heroInfo.dodgeTxt.text = string.Concat(HeroData.Data.Dodge, "%");
        heroInfo.dpTxt.text = HeroData.Data.Dp.ToString();
        heroInfo.costTxt.text = HeroData.Data.Cost.ToString();

        heroInfo.pieceTxt.text = DataManager.Instance.GetHumalPieceAmount(HeroData.Data.KoName).ToString();
    }
}