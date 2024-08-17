public class FlyingBirdLookForPlayerState : LookForPlayerState
{
    FlyingBirdEnemy enemy;

    public FlyingBirdLookForPlayerState(EnemyEntity entity, EntityStateMachine stateMachine, FlyingBirdEnemy enemy, string animBoolName) : base(entity, stateMachine, animBoolName)
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

    public override void StateChecks()
    {
        base.StateChecks();
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
            IsAllTurnsDone
            )
        {
            ChangeState(enemy.MoveState);
        }
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
    }

    #endregion
}