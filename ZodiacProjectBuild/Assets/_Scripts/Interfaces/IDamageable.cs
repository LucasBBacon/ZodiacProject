using UnityEngine;

public interface IDamageable
{
    public void Damage(int damageAmount, Vector2 attackDirection);

    void Die();
}