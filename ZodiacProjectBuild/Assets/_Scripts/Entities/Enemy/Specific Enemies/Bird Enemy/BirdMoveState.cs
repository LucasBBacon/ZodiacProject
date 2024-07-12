using UnityEngine;

public class BirdMoveState : MoveState
{
    BirdEnemy enemy;

    public BirdMoveState(EnemyEntity entity, EntityStateMachine stateMachine, BirdEnemy enemy) : base(entity, stateMachine)
    {
        this.enemy = enemy;
    }

    #region Callback Functions

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

    public override void StateUpdate()
    {
        base.StateUpdate();
    
        if (
            IsPlayerInMinAgroRange
            )
        {
            ChangeState(enemy.PlayerDetectedState);
        }

        else if (
            CollisionSensors.IsWallFront || !IsDetectingLedge
            )
        {
            enemy.IdleState.SetFlipAfterIdle(true);
            ChangeState(enemy.IdleState);
        }
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
    }

    #endregion
}