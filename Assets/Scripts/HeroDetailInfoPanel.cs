using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroDetailInfoPanel : MonoBehaviour
{
    [SerializeField] private HeroDetailInfo heroInfo;

    public void UpdateHeroInfo(UnitData HumalData)
    {
        heroInfo.nameTxt.text = HumalData.KoName;
        heroInfo.levelTxt.text = string.Concat("Lv.", HumalData.Level);
        heroInfo.heroImg.sprite = HumalData.sprite;

        heroInfo.dpsTxt.text = string.Concat("전투력 : ", HumalData.DPS);
        heroInfo.hpTxt.text = HumalData.HP.ToString();
        heroInfo.criticalTxt.text = string.Concat(HumalData.Critical, "%");
        heroInfo.apTxt.text = HumalData.Ap.ToString();
        heroInfo.dodgeTxt.text = string.Concat(HumalData.Dodge, "%");
        heroInfo.dpTxt.text = HumalData.Dp.ToString();
        heroInfo.costTxt.text = HumalData.Cost.ToString();

        heroInfo.pieceTxt.text = DataManager.Instance.GetHumalPieceAmount(HumalData.KoName).ToString();
    }
}