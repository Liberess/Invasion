using UnityEngine;

public struct DamageMessage
{
    public GameObject damager;
    public float amount;
    public Vector2 hitPoint;

    public DamageMessage(GameObject _damager, float _amount, Vector2 _hitPoint)
    {
        damager= _damager;
        amount = _amount;
        hitPoint = _hitPoint;
    }
}
