using System;
using UnityEngine;
using UnityEngine.Events;

public class LivingEntity : MonoBehaviour, IDamageable
{
    [Space(5), Header("==== Living Entity ====")]
    public float originHealth = 100f;

    [SerializeField] protected float hp;
    public float HP => hp;
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
        hp = originHealth;
    }

    public virtual bool ApplyDamage(DamageMessage dmgMsg)
    {
        if (IsInvulnerabe || dmgMsg.damager == gameObject || Dead)
            return false;

        lastDamagedTime = Time.time;
        hp -= dmgMsg.amount;

        if (HP <= 0)
            Die();

        return true;
    }

    public virtual void RestoreHealth(float newHealth)
    {
        if (Dead)
            return;

        hp += newHealth;

        if (HP >= originHealth)
            hp = originHealth;
    }

    public virtual void Die()
    {
        Dead = true;
        OnDeathAction?.Invoke();
    }
}
