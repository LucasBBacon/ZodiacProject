using UnityEngine;

public class BirdSpecialAttackState : SpecialAttackState
{
    BirdEnemy birdEnemy;
    
    public BirdSpecialAttackState(EnemyEntity entity, EntityStateMachine stateMachine, string animBoolName, Transform attackPosition, BirdEnemy birdEnemy) : base(entity, stateMachine, animBoolName, attackPosition)
    {
        this.birdEnemy = birdEnemy;
    }

    public override void StateEnter()
    {
        base.StateEnter();
    }

    public override void StateExit()
    {
        base.StateExit();  
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (IsAnimationFinished)
        {
            // if (IsPlayerInMinAgroRange)
            // {
            //     ChangeState(birdEnemy.PlayerDetectedState);
            // }
            // else
            // {
            //     ChangeState(birdEnemy.LookForPlayerState);
            // }
            birdEnemy.IdleState.SetIsResting(3f);
            ChangeState(birdEnemy.IdleState);
        }
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();
    }

    public override void FinishAttack()
    {
        base.FinishAttack();

        birdEnemy.DestroyProjectile(projectile, 1f);
    }
}
