public enum EGoodsType
{
    Stamina = 0,
    Gold,
    Dia,
    AwakeJewel      //각성석
    //EvolutionJewel  //진화석
}

public enum EBuyingType
{
    Humal = 0,
    CatAde,
    Can
}

public enum EItem
{
    CatAde = 0,
    Can
}

public enum EBtnType
{
    GPGSLogin,
    GPGSLogout,
    QuitGame,
    InitializedData,
    Option,
    CloseUI,
    GoToMain,
    RestartBattle,
    PlayFabLogin,
    Buy
}

public enum EStageLevel { Easy = 0, Normal, Hard }

public enum EPopUpType { Notice = 0, Caution, Warning }
public enum EPopUpResponse { Ok, Yes, No, Error }

public enum EArrangerType { Party = 0, Hero, Item }

public enum EUnitQueueType { Hero = 0, Enemy }

public enum EUnitJob { ShortRange = 0, LongRange, Bullet }

public enum EShopMenu { Humal = 0, Item, Package }

public enum EItemType { None = 0, Consume, Equipment }

public enum EGrade { Normal = 0, Rare, Epic, Legend }

[System.Serializable]
public class Enums
{

}