using UnityEngine;

public class BirdChargeState : ChargeState
{
    BirdEnemy birdEnemy;

    public BirdChargeState(EnemyEntity entity, EntityStateMachine stateMachine, BirdEnemy birdEnemy) : base(entity, stateMachine)
    {
        this.birdEnemy = birdEnemy;
        
    }

    public override void StateEnter()
    {
        base.StateEnter();

        Animator.SetBool(BirdEnemy.IS_WALKING, true);

        
    }

    public override void StateExit()
    {
        base.StateExit();

        Animator.SetBool(BirdEnemy.IS_WALKING, false);
    }

    public override void StateChecks()
    {
        base.StateChecks();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (PerformCloseRangeAction)
        {
            ChangeState(birdEnemy.MeleeAttackState);
        }

        if (
            !IsDetectingLedge ||
            IsDetectingWall
            )
        {
            ChangeState(birdEnemy.LookForPlayerState);
        }

        else if (
            ChargeTimeOver
            )
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

    
}
