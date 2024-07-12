public interface IDamageable
{
    public void Damage(DamageData data);

    public bool HasTakenDamage { get; set; }
    public float InvincibilityTime { get; set; }
    public bool IsInvincible { get; set; }

    void Die();
}
