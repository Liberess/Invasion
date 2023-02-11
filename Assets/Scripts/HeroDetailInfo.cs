using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class HeroDetailInfo
{
    public Text nameTxt;
    public Text levelTxt;
    public Image heroImg;
    public Image[] gradeImgs = new Image[5];

    public Text dpsTxt;
    public Text hpTxt;
    public Text criticalTxt;
    public Text apTxt;
    public Text dodgeTxt;
    public Text dpTxt;
    public Text costTxt;
}