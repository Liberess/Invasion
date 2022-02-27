using UnityEngine;

[System.Serializable]
public class UnitStatus
{
    public string name;            // 고유 닉네임
    public int ID;                      // 고유 판별 ID
    public int level;                  // 유닛 레벨
    public int cost;                   // 유닛 생성 비용
    public int hp;                      // 체력. Health Point
    public int ap;                      // 공격력. Attack Power
    public float critical;            // 치명타율. Critical Chance
    public float distance;         // 공격 거리. Attack Distance
    public float attackDelay;   // 공격 딜레이. Attack Delay Time
    public float DPS =>           // 초당 공격력. Damage per Second
        (1f / attackDelay) * ap;

    public bool isLeader;          // 영웅 파티의 리더인가?

    public Sprite mySprite;
    public RuntimeAnimatorController animCtrl;

    public UnitStatus(UnitStatus status)
    {
        this.name = status.name;
        this.ID = status.ID;
        this.level = status.level;
        this.cost = status.cost;
        this.hp = status.hp;
        this.ap = status.ap;
        this.critical = status.critical;
        this.distance = status.distance;
        this.mySprite = status.mySprite;
        this.animCtrl = status.animCtrl;
    }

    /// <summary>
    /// Lobby에서 생성되는 Hero Unit들의 Status를 설정한다.
    /// </summary>
    public UnitStatus(string _name, int _id, int _level)
    {
        this.name = _name;
        this.ID = _id;
        this.level = _level;
    }

    /// <summary>
    /// Base의 Status를 설정한다.
    /// </summary>
    public UnitStatus(string _name, int _hp)
    {
        this.name = _name;
        this.hp = _hp;
    }

    /// <summary>
    /// HeroPanel에서 표시될 Hero Unit들의 Status를 설정한다.
    /// </summary>
    public UnitStatus(string _name, int _id, int _level, int _hp, int _ap, float _critical)
    {
        this.name = _name;
        this.ID = _id;
        this.level = _level;
        this.hp = _hp;
        this.ap = _ap;
        this.critical = _critical;
    }

    /// <summary>
    /// Battle을 진행할 때, 생성되는 Unit들의 Status를 설정한다.
    /// </summary>
    public UnitStatus(string _name, int _id, int _level, int _cost,
        int _hp, int _ap, float _critical, float _distance)
    {
        this.name = _name;
        this.ID = _id;
        this.level = _level;
        this.cost = _cost;
        this.hp = _hp;
        this.ap = _ap;
        this.critical = _critical;
        this.distance = _distance;
    }
}