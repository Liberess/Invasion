using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroSlot : MonoBehaviour
{
    [SerializeField] private Text lvTxt;
    [SerializeField] private Image heroImg;
    [SerializeField] private UnitStatus myStatus;
    public UnitStatus MyStatus { get => myStatus; }

    private void Start()
    {
        if (lvTxt == null)
            lvTxt = transform.Find("LvTxt").GetComponent<Text>();

        if(heroImg == null)
            heroImg = transform.Find("HeroImg").GetComponent<Image>();
    }

    public void UnitSetup(UnitStatus status)
    {
        myStatus = status;
        heroImg.sprite = myStatus.mySprite;
    }
}