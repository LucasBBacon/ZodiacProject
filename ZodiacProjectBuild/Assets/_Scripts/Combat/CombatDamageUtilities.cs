using System.Collections.Generic;
using UnityEngine;
using Utilities;

public static class CombatDamageUtilities
{
    public static bool TryDamage(GameObject gameObject, DamageData damageData, out IDamageable damageable)
    {
        if (gameObject.TryGetComponentInChildren(out damageable))
        {
            damageable.Damage(damageData);
            return true;
        }
        return false;
    }

    public static bool TryDamage(Collider2D[] colliders, DamageData damageData, out List<IDamageable> damageables)
    {
        var hasDamaged = false;

        damageables = new List<IDamageable>();

        foreach (var collider in colliders)
        {
            if (TryDamage(collider.gameObject, damageData, out IDamageable damageable))
            {
                damageables.Add(damageable);
                hasDamaged = true;
            }
        }

        return hasDamaged;
    }
}