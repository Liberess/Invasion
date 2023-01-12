using UnityEngine;

[System.Serializable]
public class HumalData : UnitData
{
    [SerializeField] private bool isLeader;          // 영웅 파티의 리더인가?
    public bool IsLeader => isLeader;
    public void SetLeader(bool _isLeader) => isLeader = _isLeader;

    public HumalData(UnitData unitData)
    {
        SetDataSo(unitData.Data);
        SetHumalData(unitData);
    }

    public HumalData(UnitDataSO unitDataSO)
    {
        SetDataSo(unitDataSO);
        SetHumalData(unitDataSO);
    }

    public void SetHumalData(UnitData unitData)
    {
        hp = unitData.HP;
        ap = unitData.Ap;
        moveSpeed = unitData.MoveSpeed;
        critical = unitData.Critical;
        dodge = unitData.Dodge;
        distance = unitData.Distance;
        attackDelay = unitData.AttackDelay;

        //mySprite = unitData.mySprite;
        //animCtrl = unitData.animCtrl;
    }

    public void SetHumalData(UnitDataSO unitDataSO)
    {
        hp = unitDataSO.HP;
        ap = unitDataSO.Ap;
        moveSpeed = unitDataSO.MoveSpeed;
        critical = unitDataSO.Critical;
        dodge = unitDataSO.Dodge;
        distance = unitDataSO.Distance;
        attackDelay = unitDataSO.AttackDelay;

        //mySprite = unitData.mySprite;
        //animCtrl = unitData.animCtrl;
    }
}
