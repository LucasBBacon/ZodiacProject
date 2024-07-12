using UnityEngine;

[CreateAssetMenu(fileName = "newWeaponData", menuName = "Data/Entity Data/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Stats")]
    public float DamageAmount = 10f;
    public float AttackRange = 1;
    public float TimeBetweenAttacks = 0.3f;
    public LayerMask AttackLayer;
    public int NumberOfAttacks = 2;
    public float AttackCounterResetCooldown = 1f;

    [Header("Weapon Knockback")]
    public Vector2 KnockbackAngle = Vector2.one;
    public float KnockbackStrength = 10f;
}
