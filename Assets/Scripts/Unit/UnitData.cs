using UnityEngine;

[CreateAssetMenu(fileName = "Unit Data",
    menuName = "Scriptable Object/Unit Data", order = int.MaxValue)]
public class UnitData : ScriptableObject
{
    public int ID;                      // ���� �Ǻ� ID.
    public string name;            // ���� �г���.
    public int level;                  // ���� ����.
    public int cost;                   // ���� ���� ���.
    public int hp;                      // ü��. Health Point
    public int ap;                      // ���ݷ�. Attack Power
    public int dp;                      // ����. Defense Power
    public float moveSpeed;    // �̵��ӵ�.
    public float critical;            // ġ��Ÿ��. Critical Chance
    public float dodge;             // ȸ����.
    public float distance;         // ���� �Ÿ�. Attack Distance
    public float attackDelay;   // ���� ������. Attack Delay Time
    public float DPS =>           // �ʴ� ���ݷ�. Damage per Second
        (1f / attackDelay) * ap;

    public bool isLeader;          // ���� ��Ƽ�� �����ΰ�?

    public Sprite mySprite;
    public RuntimeAnimatorController animCtrl;
}
