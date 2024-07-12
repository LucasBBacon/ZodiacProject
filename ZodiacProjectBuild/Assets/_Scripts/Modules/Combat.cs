using System;
using System.Collections;
using UnityEngine;

public class Combat : MonoBehaviour, IDamageable, IKnockbackable
{
    [field:SerializeField] public float InvincibilityTime { get; set; } = 0.3f;
    public bool IsInvincible { get; set; } = false;
    public bool HasTakenDamage { get; set; }

    [field:SerializeField] public float MaxKnockbackTime { get; set; } = 0.4f;
    public bool HasKnockbacked { get; set; }
    public bool IsNotKnockbackable { get; set; } = false;

    #region Entity References

    Entity entity;
    Stats Stats => entity.Stats;
    GameObject DamageParticles => entity.DamageParticles;
    Movement Movement => entity.Movement;
    CollisionSensors CollisionSensors => entity.CollisionSensors;
    ParticleManager ParticleManager => entity.ParticleManager;
    DamageFlash DamageFlash => entity.DamageFlash;

    #endregion


    float knockbackStartTime;

    #region Callback Functions

    private void Awake()
    {
        entity = GetComponent<Entity>();
    }

    private void Update()
    {
        CheckKnockback();
    }

    private void OnEnable()
    {
        Stats.Health.OnCurrentValueZero += Die;
    }

    private void OnDisable()
    {
        Stats.Health.OnCurrentValueZero -= Die;    
    }

    #endregion


    #region Functionality

    public void Damage(DamageData data)
    {
        HasTakenDamage = true;
        entity.ChangeToDamageState = true;
        DamageFlash.CallDamageFlash();
        

        if (data.Amount <= 0f)
        {
            return;
        }

        Stats.Health.Decrease(data.Amount);
        Debug.Log(Stats.Health.CurrentValue);

        Vector3 relativePos = gameObject.transform.InverseTransformDirection(data.Source.transform.position);
        Quaternion rotation = Quaternion.FromToRotation
            (
                Vector2.right,
                Vector2.right * (-Mathf.Sign(relativePos.x))
            );
        ParticleManager.SpawnParticlesRelative
            (
                DamageParticles,
                new Vector2(0.2f, 0f),
                rotation
            );

        IsInvincible = true;
        StartCoroutine(ResetInvicible());
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public void Knockback(KnockbackData data)
    {
        HasKnockbacked = true;
        
        Movement.SetForce(data.Strength, data.Angle, data.Direction);
        knockbackStartTime = Time.time;
        IsNotKnockbackable = true;
    }

    private void CheckKnockback()
    {
        if (
            HasKnockbacked &&
            ((Movement.Body.velocity.y <= 0.01f && CollisionSensors.IsGrounded)
            || (Time.time >= knockbackStartTime + MaxKnockbackTime))    
        )
        {  
            HasKnockbacked = false;
            Movement.SetVelocityZero();     
        }
    }

    IEnumerator ResetInvicible()
    {
        yield return new WaitForSeconds(InvincibilityTime);

        IsInvincible = false;
        IsNotKnockbackable = false;
    }

    #endregion
}
