using UnityEngine;

public class BirdMoveState : MoveState
{
    BirdEnemy enemy;

    public BirdMoveState(EnemyEntity entity, EntityStateMachine stateMachine, BirdEnemy enemy, string animBoolName) : base(entity, stateMachine, animBoolName)
    {
        this.enemy = enemy;
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

        //Debug.Log(IsDetectingWall + ", " + !IsDetectingLedge);
    
        if (IsPlayerInMinAgroRange)
        {
            ChangeState(enemy.PlayerDetectedState);
        }

        else if (
            IsDetectingWall
            || !IsDetectingLedge
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
