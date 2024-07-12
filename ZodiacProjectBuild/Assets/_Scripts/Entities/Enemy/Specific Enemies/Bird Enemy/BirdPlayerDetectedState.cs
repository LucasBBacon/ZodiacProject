using UnityEngine;

public class BirdPlayerDetectedState : PlayerDetectedState
{
    BirdEnemy birdEnemy;

    public BirdPlayerDetectedState(EnemyEntity entity, EntityStateMachine stateMachine, BirdEnemy birdEnemy) : base(entity, stateMachine)
    {
        this.birdEnemy = birdEnemy;
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        Animator.SetBool(BirdEnemy.PLAYER_DETECTED, true);
    }

    public override void StateExit()
    {
        base.StateExit();

        Animator.SetBool(BirdEnemy.PLAYER_DETECTED, false);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (
            PerformCloseRangeAction
            )
        {
            ChangeState(birdEnemy.MeleeAttackState);
        }

        else if (
            PerformLongRangeAction
            )
        {
            ChangeState(birdEnemy.ChargeState);
        }

        else if (
            !IsPlayerInMaxAgroRange
            )
        {
            ChangeState(birdEnemy.LookForPlayerState);
        }

        else if (
            !IsDetectingLedge
            )
        {
            Movement?.Turn();
            ChangeState(birdEnemy.MoveState);
        }
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
    }

    #endregion
}
