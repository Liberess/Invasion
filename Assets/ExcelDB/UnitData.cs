using UnityEngine;

[System.Serializable]
public class UnitData
{
    [SerializeField] private int id;                      // 고유 판별 ID.
    public int ID => id;

    [SerializeField] private string koName;            // 고유 닉네임.
    public string KoName => koName;

    [SerializeField] private string enName;            // 고유 닉네임.
    public string EnName => enName;

    [SerializeField] private int level;                  // 유닛 레벨.
    public int Level => level;

    [SerializeField] private int cost;                   // 유닛 생성 비용.
    public int Cost => cost;

    [SerializeField] private int hp;                      // 체력. Health Point
    public int HP => hp;

    [SerializeField] private int ap;                      // 공격력. Attack Power
    public int Ap => ap;

    [SerializeField] private int dp;                      // 방어력. Defense Power
    public int Dp => dp;

    [SerializeField] private float moveSpeed;    // 이동속도.
    public float MoveSpeed => moveSpeed;

    [SerializeField] private float critical;            // 치명타율. Critical Chance
    public float Critical => critical;

    [SerializeField] private float dodge;             // 회피율.
    public float Dodge => dodge;

    [SerializeField] private float distance;         // 공격 거리. Attack Distance
    public float Distance => distance;

    [SerializeField] private float attackDelay;   // 공격 딜레이. Attack Delay Time
    public float AttackDelay => attackDelay;

    public float DPS =>           // 초당 공격력. Damage per Second
        (1f / attackDelay) * ap;

    [SerializeField] private bool isLeader;          // 영웅 파티의 리더인가?
    public bool IsLeader => isLeader;
    public void SetLeader(bool _isLeader) => isLeader = _isLeader;

    public Sprite sprite;
    public RuntimeAnimatorController animCtrl;

    public UnitData(UnitData unitData)
    {
        id = unitData.id;
        koName = unitData.koName;
        enName = unitData.enName;
        level = unitData.level;
        cost = unitData.cost;
        hp = unitData.HP;
        ap = unitData.Ap;
        dp = unitData.Dp;
        moveSpeed = unitData.MoveSpeed;
        critical = unitData.Critical;
        dodge = unitData.Dodge;
        distance = unitData.Distance;
        attackDelay = unitData.AttackDelay;

        sprite = unitData.sprite;
        animCtrl = unitData.animCtrl;
    }
}
