using UnityEngine;

[System.Serializable]
public class HumalData
{
    [SerializeField] private int id;                      // ���� �Ǻ� ID.
    public int ID { get => id; }

    [SerializeField] private string m_name;            // ���� �г���.
    public string Name { get => m_name; }

    [SerializeField] private int level;                  // ���� ����.
    public int Level { get => level; }

    [SerializeField] private int cost;                   // ���� ���� ���.
    public int Cost { get => cost; }

    [SerializeField] private int hp;                      // ü��. Health Point
    public int HP { get => hp; }

    [SerializeField] private int ap;                      // ���ݷ�. Attack Power
    public int Ap { get => ap; }

    [SerializeField] private int dp;                      // ����. Defense Power
    public int Dp { get => dp; }

    [SerializeField] private float moveSpeed;    // �̵��ӵ�.
    public float MoveSpeed { get => moveSpeed; }

    [SerializeField] private float critical;            // ġ��Ÿ��. Critical Chance
    public float Critical { get => critical; }

    [SerializeField] private float dodge;             // ȸ����.
    public float Dodge { get => dodge; }

    [SerializeField] private float distance;         // ���� �Ÿ�. Attack Distance
    public float Distance { get => distance; }

    [SerializeField] private float attackDelay;   // ���� ������. Attack Delay Time
    public float AttackDelay { get => attackDelay; }

    public float DPS =>           // �ʴ� ���ݷ�. Damage per Second
        (1f / attackDelay) * ap;

    [SerializeField] private bool isLeader;          // ���� ��Ƽ�� �����ΰ�?
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
