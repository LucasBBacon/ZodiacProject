using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMeleeAttackState : MeleeAttackState
{
    BirdEnemy birdEnemy;

    SOWeaponData weaponData;

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

    public BirdMeleeAttackState(EnemyEntity entity, EntityStateMachine stateMachine, string animBoolName, Transform attackPosition, BirdEnemy enemy, SOWeaponData weaponData) : base(entity, stateMachine, animBoolName, attackPosition)
    {
        this.birdEnemy = enemy;
        this.weaponData = weaponData;
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        isAbilityDone = false;
    }

    public override void StateExit()
    {
        base.StateExit();
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

    public override void TriggerAttack()
    {
        base.TriggerAttack();
    }

    public override void FinishAttack()
    {
        base.FinishAttack();
    }
}
