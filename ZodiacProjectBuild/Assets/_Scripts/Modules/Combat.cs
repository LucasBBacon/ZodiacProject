using UnityEngine;

public class Combat : MonoBehaviour, IDamageable
{
    private Stats stats;
    private EntityData _entityData;

    private void Start()
    {
        stats = GetComponentInParent<Stats>();
    }
    public void Damage(int damageAmount, Vector2 attackDirection)
    {
        stats.currentHealth -= damageAmount;

        if(stats.currentHealth < 0)
            Die();
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}