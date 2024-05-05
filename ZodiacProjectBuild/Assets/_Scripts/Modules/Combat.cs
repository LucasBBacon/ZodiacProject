using UnityEngine;

public class Combat : Core, IDamageable
{
    private Stats stats;

    private void Start()
    {
        stats = GetComponentInParent<Stats>();
    }
    public void Damage(int damageAmount, Vector2 attackDirection)
    {
        stats.currentHealth -= damageAmount;

        
    }

    public void Die()
    {

    }
}