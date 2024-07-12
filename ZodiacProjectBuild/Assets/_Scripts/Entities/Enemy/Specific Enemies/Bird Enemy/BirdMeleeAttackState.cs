using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMeleeAttackState : MeleeAttackState
{
    BirdEnemy birdEnemy;

    WeaponData weaponData;

    public float AttackTimer;

    bool isAbilityDone;
    bool meleeAttack;
    
    bool isDamageActive;

    // damage lists
    RaycastHit2D[] hits;
    List<IDamageable> iDamaged = new List<IDamageable>();
    List<IKnockbackable> iKnockbacked = new List<IKnockbackable>();

    // attack direction
    bool collided;
    bool downwardStrike;
    Vector2 direction;
    public Vector2 attackPos;

    public BirdMeleeAttackState(EnemyEntity entity, EntityStateMachine stateMachine, Transform attackPosition, BirdEnemy enemy, WeaponData weaponData) : base(entity, stateMachine, attackPosition)
    {
        this.birdEnemy = enemy;
        this.weaponData = weaponData;
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();
        
        Animator.SetBool(BirdEnemy.ATTACK, true);

        birdEnemy.StartCoroutine(AttackDamage());

        isAbilityDone = false;
    }

    public override void StateExit()
    {
        base.StateExit();

        Animator.SetBool(BirdEnemy.ATTACK, false);
    }

    public override void StateChecks()
    {
        base.StateChecks();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (IsAnimationFinished)
        {
            if (
                IsPlayerInMinAgroRange
                )
            {
                ChangeState(birdEnemy.PlayerDetectedState);
            }
            else
            {
                ChangeState(birdEnemy.LookForPlayerState);
            }
        }
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
    }

    #endregion

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();

        isDamageActive = true;
    }

    public override void AnimationFinishedTrigger()
    {
        base.AnimationFinishedTrigger();

        isDamageActive = false;
    }

    public IEnumerator AttackDamage()
    {
        isDamageActive = true;

        while (isDamageActive)
        {
            hits = Physics2D.CircleCastAll
                (
                    attackPos,
                    weaponData.AttackRange,
                    Vector2.right,
                    0f,
                    weaponData.AttackLayer
                );
            
            if (hits.Length > 0)
            {
                // Debug.Log("hit objects");
                for (int i = 0; i < hits.Length; i++)
                {
                    IDamageable iDamageable = hits[i].collider.gameObject.GetComponent<IDamageable>();
                    if (
                        iDamageable != null &&
                        !iDamageable.HasTakenDamage &&
                        !iDamageable.IsInvincible
                        )
                    {
                        iDamageable.Damage
                            (
                                new DamageData
                                    (
                                        weaponData.DamageAmount,
                                        birdEnemy.gameObject
                                    )
                            );
                        iDamaged.Add(iDamageable);

                        // _player.ParticleManager.StartEffect(birdEnemy.HitEffect, hits[i].point, Quaternion.identity, 0.6f);
                        
                        collided = true;
                    }

                    IKnockbackable iKnockbackable = hits[i].collider.gameObject.GetComponent<IKnockbackable>();
                    if (
                        iKnockbackable != null &&
                        !iKnockbackable.HasKnockbacked &&
                        !iKnockbackable.IsNotKnockbackable
                        )
                    {
                        iKnockbackable.Knockback
                            (
                                new KnockbackData
                                    (
                                        weaponData.KnockbackAngle,
                                        weaponData.KnockbackStrength,
                                        Movement.FacingDirection,
                                        birdEnemy.gameObject
                                    )
                            );
                        iKnockbacked.Add(iKnockbackable);
                    }
                }
            }

            yield return null;
        }

        ResetLists();
    }

    private void ResetLists()
    {
        foreach (IDamageable damaged in iDamaged)
        {
            damaged.HasTakenDamage = false;
        }

        collided = false;

        iDamaged.Clear();
        iKnockbacked.Clear();
    }
}
