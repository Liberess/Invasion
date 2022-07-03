using UnityEngine;
using UnityEngine.UI;

public enum MenuBtnTypes
{
    Hero = 0,
    Facility,
    Inventory,
    Book,
    Shop,
    Letterbox,
    Etc,
    Dungeon,
    Adventure
}

public class MenuBtn : MonoBehaviour
{
    // UIManager
    [SerializeField] private MenuBtnTypes btnType;

    [SerializeField] private GameObject myPanel;

    // Transition of Menu Button is Sprite Swap
    [SerializeField] private Sprite showSprite;
    [SerializeField] private Sprite hideSprite;

    [SerializeField] private bool isClick;
    [SerializeField] private bool isShow;

    private void Start()
    {
        isClick = false;
        isShow = false;

        if(btnType != MenuBtnTypes.Dungeon && btnType != MenuBtnTypes.Adventure)
            UIManager.Instance.HidePanelAction += () => MyPanelHide();
    }

    private void MyPanelShow()
    {
        if(!isShow)
        {
            isShow = true;
            GetComponent<Image>().sprite = showSprite;
            myPanel.GetComponent<Animator>().SetTrigger("doShow");
        }
    }

    private void MyPanelHide()
    {
        if(!isClick && isShow)
        {
            isClick = false;
            isShow = false;
            GetComponent<Image>().sprite = hideSprite;
            myPanel.GetComponent<Animator>().SetTrigger("doHide");
        }
    }

    public void OnClickBtn(bool active)
    {
        isClick = active;

        if (btnType != MenuBtnTypes.Dungeon && btnType != MenuBtnTypes.Adventure)
        {
            if(active)
                MyPanelShow();
            else
                MyPanelHide();
        }
        else
        {
            if (active)
                myPanel.SetActive(true);
            else
                myPanel.SetActive(false);
        }

        UIManager.Instance.HidePanelAction();
        SoundManager.Instance.PlaySFX("Button");
    }
}