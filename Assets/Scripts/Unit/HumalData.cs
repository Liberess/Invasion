using UnityEngine;

[System.Serializable]
public class HumalData
{
    [SerializeField] private int id;                      // 고유 판별 ID.
    public int ID { get => id; }

    [SerializeField] private string m_name;            // 고유 닉네임.
    public string Name { get => m_name; }

    [SerializeField] private int level;                  // 유닛 레벨.
    public int Level { get => level; }

    [SerializeField] private int cost;                   // 유닛 생성 비용.
    public int Cost { get => cost; }

    [SerializeField] private int hp;                      // 체력. Health Point
    public int HP { get => hp; }

    [SerializeField] private int ap;                      // 공격력. Attack Power
    public int Ap { get => ap; }

    [SerializeField] private int dp;                      // 방어력. Defense Power
    public int Dp { get => dp; }

    [SerializeField] private float moveSpeed;    // 이동속도.
    public float MoveSpeed { get => moveSpeed; }

    [SerializeField] private float critical;            // 치명타율. Critical Chance
    public float Critical { get => critical; }

    [SerializeField] private float dodge;             // 회피율.
    public float Dodge { get => dodge; }

    [SerializeField] private float distance;         // 공격 거리. Attack Distance
    public float Distance { get => distance; }

    [SerializeField] private float attackDelay;   // 공격 딜레이. Attack Delay Time
    public float AttackDelay { get => attackDelay; }

    public float DPS =>           // 초당 공격력. Damage per Second
        (1f / attackDelay) * ap;

    [SerializeField] private bool isLeader;          // 영웅 파티의 리더인가?
    public bool IsLeader => isLeader;
    public void SetLeader(bool _isLeader) => isLeader = _isLeader;

    public Sprite mySprite;
    public RuntimeAnimatorController animCtrl;

    [SerializeField] private UnitData m_unitData;
    public UnitData UnitData => m_unitData;

    public HumalData(UnitData unitData)
    {
        m_unitData = unitData;
        SetHumalData(unitData);
    }

    public void SetHumalData(UnitData unitData)
    {
        id = unitData.ID;
        m_name = unitData.Name;
        level = unitData.Level;
        cost = unitData.Cost;
        hp = unitData.HP;
        ap = unitData.Ap;
        moveSpeed = unitData.MoveSpeed;
        critical = unitData.Critical;
        dodge = unitData.Dodge;
        distance = unitData.Distance;
        attackDelay = unitData.AttackDelay;

        mySprite = unitData.mySprite;
        animCtrl = unitData.animCtrl;
    }
}
