public class FlyingBirdIdleState : IdleState
{
    FlyingBirdEnemy enemy;

    public FlyingBirdIdleState(EnemyEntity entity, EntityStateMachine stateMachine, FlyingBirdEnemy enemy, string animBoolName) : base(entity, stateMachine, animBoolName)
    {
        this.enemy = enemy;
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
