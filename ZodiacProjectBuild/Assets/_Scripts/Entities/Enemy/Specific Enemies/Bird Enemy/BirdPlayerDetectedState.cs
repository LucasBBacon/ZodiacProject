using UnityEngine;

public class BirdPlayerDetectedState : PlayerDetectedState
{
    BirdEnemy birdEnemy;

    bool _shouldAttack;

    public BirdPlayerDetectedState(EnemyEntity entity, EntityStateMachine stateMachine, BirdEnemy birdEnemy, string animBoolName) : base(entity, stateMachine, animBoolName)
    {
        this.birdEnemy = birdEnemy;
    }

    #region Callback Functions

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

        if (
            PerformCloseRangeAction
            && _shouldAttack
            )
        {
            _shouldAttack = !_shouldAttack;
            ChangeState(birdEnemy.MeleeAttackState);
        }

        else if (
            PerformCloseRangeAction
            && !_shouldAttack
            )
        {
            Debug.Log("I should be doing special attack rn!");
            _shouldAttack = !_shouldAttack;
            ChangeState(birdEnemy.SpecialAttackState);
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
            Movement.Turn();
            ChangeState(birdEnemy.MoveState);
        }
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
    }

    #endregion
}
