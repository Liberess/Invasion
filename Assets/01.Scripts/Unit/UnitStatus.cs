[System.Serializable]
public class UnitStatus
{
    public string name;
    public int ID;
    public float exp;       // 경험치. Experience
    public int level;
    public int cost;
    public int hp;            // 체력. Health Point
    public int ap;            // 공격력. Attack Power
    public float dp;         // 방어력. Defensive Power
    public float critical;  // 치명타율. Critical Chance
    public float dodge;   // 회피율. Dodge Chance

    /// <summary>
    /// Lobby에서 생성되는 Hero Unit들의 Status를 설정한다.
    /// </summary>
    public UnitStatus(string _name, int _id, float _exp, int _level)
    {
        this.name = _name;
        this.ID = _id;
        this.exp = _exp;
        this.level = _level;
    }

    /// <summary>
    /// HeroPanel에서 표시될 Hero Unit들의 Status를 설정한다.
    /// </summary>
    public UnitStatus(string _name, int _id, float _exp, int _level,
        int _hp, int _ap, float _dp, float _critical, float _dodge)
    {
        this.name = _name;
        this.ID = _id;
        this.exp = _exp;
        this.level = _level;
        this.hp = _hp;
        this.ap = _ap;
        this.dp = _dp;
        this.critical = _critical;
        this.dodge = _dodge;
    }

    /// <summary>
    /// Battle을 진행할 때, 생성되는 Unit들의 Status를 설정한다.
    /// </summary>
    public UnitStatus(string _name, int _id, float _exp, int _level, int _cost,
        int _hp, int _ap, float _dp, float _critical, float _dodge)
    {
        this.name = _name;
        this.ID = _id;
        this.exp = _exp;
        this.level = _level;
        this.cost = _cost;
        this.hp = _hp;
        this.ap = _ap;
        this.dp = _dp;
        this.critical = _critical;
        this.dodge = _dodge;
    }
}