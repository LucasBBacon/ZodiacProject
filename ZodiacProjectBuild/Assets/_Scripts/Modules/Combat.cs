using System;
using System.Collections;
using Unity.VisualScripting;
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


    float _knockbackStartTime;

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


    #region Damage

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

        SpawnDamageParticles(data.Source.transform.position);

        IsInvincible = true;
        if (entity.isActiveAndEnabled) StartCoroutine(ResetInvicible());
    }

    public void Die()
    {
        entity.Die();
    }

    #endregion


    #region Knockback

    public void Knockback(KnockbackData data)
    {
        HasKnockbacked = true;
        
        Movement.SetVelocity(data.Strength, data.Angle, data.Direction);
        Movement.CanSetVelocity = false;
        IsNotKnockbackable = true;
        _knockbackStartTime = Time.time;
    }

    private void CheckKnockback()
    {
        if (
            HasKnockbacked &&
            ((Movement.Body.velocity.y <= 0.01f && CollisionSensors.IsGrounded)
            || (Time.time >= _knockbackStartTime + MaxKnockbackTime))    
        )
        {  
            HasKnockbacked = false;
            Movement.CanSetVelocity = true;
        }
    }

    #endregion

    IEnumerator ResetInvicible()
    {
        yield return new WaitForSeconds(InvincibilityTime);

        IsInvincible = false;
        IsNotKnockbackable = false;
    }

    #region Effects

    void SpawnDamageParticles(Vector3 sourcePos)
    {
        Vector3 relativePos = gameObject.transform.InverseTransformDirection(sourcePos);
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
    }

    #endregion
}