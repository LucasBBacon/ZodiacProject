using System;
using UnityEngine;

public class DamageReceiver : IDamageable
{
    [SerializeField] GameObject _damageParticles;

    public Modifiers<Modifier<DamageData>, DamageData> Modifiers { get; } = new();
    public bool HasTakenDamage { get; set; }
    public float InvincibilityTime { get; set; }
    public bool IsInvincible { get; set; }

    [SerializeField] Stats _stats;
    [SerializeField] ParticleManager _particleManager;

    public void Damage(DamageData data)
    {
        data = Modifiers.ApplyAllModifiers(data);

        if (data.Amount <= 0f)
        {
            return;
        }

        _stats.Health.Decrease(data.Amount);
    }

    public void Die()
    {
        throw new NotImplementedException();
    }
}