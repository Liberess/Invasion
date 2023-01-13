using System;
using UnityEngine;
using UnityEngine.Events;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float originHealth = 100f;
    public float HP { get; protected set; }
    public bool Dead { get; protected set; }

    public UnityAction OnDeathAction;

    private const float minTimeBetDamaged = 0.1f;
    private float lastDamagedTime;

    protected bool IsInvulnerabe //무적 상태?
    {
        get
        {
            if (Time.time >= lastDamagedTime + minTimeBetDamaged) return false;

            return true;
        }
    }

    protected virtual void OnEnable()
    {
        Dead = false;
        HP = originHealth;
    }

    public virtual bool ApplyDamage(DamageMessage dmgMsg)
    {
        if (IsInvulnerabe || dmgMsg.damager == gameObject || Dead)
            return false;

        lastDamagedTime = Time.time;
        HP -= dmgMsg.amount;

        if (HP <= 0)
            Die();

        return true;
    }

    public virtual void RestoreHealth(float newHealth)
    {
        if (Dead)
            return;

        HP += newHealth;

        if (HP >= originHealth)
            HP = originHealth;
    }

    public virtual void Die()
    {
        Dead = true;
        OnDeathAction?.Invoke();
    }
}
