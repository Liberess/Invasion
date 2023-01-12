using UnityEngine;

[System.Serializable]
public class UnitData
{
    [SerializeField] private UnitDataSO dataSO;
    public UnitDataSO Data => dataSO;

    public void SetDataSo(UnitDataSO dataSO) => this.dataSO = dataSO;

    [SerializeField] protected int hp;                      // Health Point
    public int HP => hp;

    [SerializeField] protected int ap;                      // Attack Power
    public int Ap => ap;

    [SerializeField] protected int dp;                      // Defense Power
    public int Dp => dp;

    [SerializeField] protected float moveSpeed;
    public float MoveSpeed => moveSpeed;

    [SerializeField] protected float critical;            // ġ��Ÿ��. Critical Chance
    public float Critical => critical;

    [SerializeField] protected float dodge;             // ȸ����.
    public float Dodge => dodge;

    [SerializeField] protected float distance;         // ���� �Ÿ�. Attack Distance
    public float Distance => distance;

    [SerializeField] protected float attackDelay;   // ���� ������. Attack Delay Time
    public float AttackDelay => attackDelay;

    public float DPS =>           // 초당 공격력. Damage per Second
        (1f / attackDelay) * ap;

    public Sprite sprite;
    public RuntimeAnimatorController animCtrl;
}
