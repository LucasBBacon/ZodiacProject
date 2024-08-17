using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "newEntityData", menuName = "Data/Entity Data/Entity Data")]
public class SOEntityData : ScriptableObject
{
    [Header("Health")]
    public int MaxHealth = 30;
    public float DamageHopSpeed = 3f;
    public float InvincibilityTime = 2f;


    [Header("Knockback")]
    public float maxKnockbackTime = 2f;


    [Header("Stun")]
    public float StunResistance = 3f;
    public float StunRecoveryTime = 2f;


    [Header("Check Distances")]
    public float GroundCheckRadius = 0.3f;
    public float WallCheckDistance = 0.2f;
    public float LedgeCheckDistance = 0.4f;
    public float MaxAgroDistance = 4f;
    public float MinAgroDistance = 3f;
    public float CloseRangeActionDistance = 1f;


    [Header("Layer Masks")]
    public LayerMask PlayerMask;


    [Header("Idle State")]
    public float MinIdleTime = 1f;
    public float MaxIdleTime = 2f;

    [Header("Movement State")]
    public float MovementSpeed  =3f;

    [Header("Charge State")]
    public float ChargeSpeed = 5f;
    public float ChargeTime = 2f;

    [Header("Look For Player State")]
    public int AmountOfTurns = 2;
    public float TimeBetweenTurns = 0.75f;

    [Header("Player Detected State")]
    public float LongRangeActionTime = 1.5f;

    [Header("Melee Attack")]
    public float AttackRadius = 0.5f;
    public int AttackDamage = 10;
    public Vector2 KnockbackAngle = Vector2.one;
    public float KnockbackStrength = 10f;
    public float PoiseDamage = 3f;

    [Header("Special Attack")]
    public GameObject Projectile;
    public Vector2 ProjectileOffset = new Vector2(1f, 0f);

    [Header("Dead State")]
    public GameObject DeathEntity;
    public GameObject DeathParticles;
}