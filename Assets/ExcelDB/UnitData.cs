using UnityEngine;

[System.Serializable]
public class UnitData
{
    [SerializeField] private int id;                      // ���� �Ǻ� ID.
    public int ID => id;

    [SerializeField] private string koName;            // ���� �г���.
    public string KoName => koName;

    [SerializeField] private string enName;            // ���� �г���.
    public string EnName => enName;

    [SerializeField] private int level;                  // ���� ����.
    public int Level => level;

    [SerializeField] private int cost;                   // ���� ���� ���.
    public int Cost => cost;

    [SerializeField] private int hp;                      // ü��. Health Point
    public int HP => hp;

    [SerializeField] private int ap;                      // ���ݷ�. Attack Power
    public int Ap => ap;

    [SerializeField] private int dp;                      // ����. Defense Power
    public int Dp => dp;

    [SerializeField] private float moveSpeed;    // �̵��ӵ�.
    public float MoveSpeed => moveSpeed;

    [SerializeField] private float critical;            // ġ��Ÿ��. Critical Chance
    public float Critical => critical;

    [SerializeField] private float dodge;             // ȸ����.
    public float Dodge => dodge;

    [SerializeField] private float distance;         // ���� �Ÿ�. Attack Distance
    public float Distance => distance;

    [SerializeField] private float attackDelay;   // ���� ������. Attack Delay Time
    public float AttackDelay => attackDelay;

    public float DPS =>           // �ʴ� ���ݷ�. Damage per Second
        (1f / attackDelay) * ap;

    [SerializeField] private bool isLeader;          // ���� ��Ƽ�� �����ΰ�?
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
