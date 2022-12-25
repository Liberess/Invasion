using UnityEngine;

[CreateAssetMenu(fileName = "Unit Data",
    menuName = "Scriptable Object/Unit Data", order = int.MaxValue)]
public class UnitData : ScriptableObject
{
    public int ID;                      // 고유 판별 ID.
    public string name;            // 고유 닉네임.
    public int level;                  // 유닛 레벨.
    public int cost;                   // 유닛 생성 비용.
    public int hp;                      // 체력. Health Point
    public int ap;                      // 공격력. Attack Power
    public int dp;                      // 방어력. Defense Power
    public float moveSpeed;    // 이동속도.
    public float critical;            // 치명타율. Critical Chance
    public float dodge;             // 회피율.
    public float distance;         // 공격 거리. Attack Distance
    public float attackDelay;   // 공격 딜레이. Attack Delay Time
    public float DPS =>           // 초당 공격력. Damage per Second
        (1f / attackDelay) * ap;

    public bool isLeader;          // 영웅 파티의 리더인가?

    public Sprite mySprite;
    public RuntimeAnimatorController animCtrl;
}
