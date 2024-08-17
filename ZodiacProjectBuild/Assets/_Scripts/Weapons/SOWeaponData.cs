using UnityEngine;

[CreateAssetMenu(fileName = "newWeaponData", menuName = "Data/Entity Data/Weapon Data")]
public class SOWeaponData : ScriptableObject
{
    [Header("Weapon Stats")]
    public float DamageAmount = 10f;
    public float AttackRange = 1;
    public float TimeBetweenAttacks = 0.3f;

    [Header("Weapon Knockback")]
    public Vector2 KnockbackAngle = Vector2.one;
    public float KnockbackStrength = 10f;

    public DirectionalInformation[] BlockDirectionInformation { get; private set; }
    public PhaseTime BlockWindowStart { get; private set; }
    public PhaseTime BlockWindowEnd { get; private set; }

    [Header("Layers")]
    public LayerMask DetectableLayers;
    // public bool IsBlocked(float angle, out DirectionalInformation directionalInformation)
    // {
    //     directionalInformation = null;

    //     foreach (var direction in BlockDirectionInformation)
    //     {
    //         var blocked = directionalInformation.IsAngleBetween(angle);

    //         if (!blocked)
    //             continue;

    //         directionalInformation = direction;
    //         return true;
    //     }

    //     return false;
    // }
}