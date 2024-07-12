using UnityEngine;

public class BirdIdleState : IdleState
{
    BirdEnemy enemy;

    public BirdIdleState(EnemyEntity entity, EntityStateMachine stateMachine, BirdEnemy enemy) : base(entity, stateMachine)
    {
        this.enemy = enemy;
    }

    public override void StateEnter()
    {
        base.StateEnter();

        Animator.SetBool(BirdEnemy.IDLE, true);
    }

    public override void StateExit()
    {
        base.StateExit();

        Animator.SetBool(BirdEnemy.IDLE, false);
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
            IdleTimeOver
            )
        {
            ChangeState(enemy.MoveState);
        }
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
    }
}
