public class BirdLookForPlayerState : LookForPlayerState
{
    BirdEnemy birdEnemy;

    public BirdLookForPlayerState(EnemyEntity entity, EntityStateMachine stateMachine, BirdEnemy birdEnemy, string animBoolName) : base(entity, stateMachine, animBoolName)
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
            ChangeState(birdEnemy.PlayerDetectedState);
        }

        else if (
            IsAllTurnsDone
            )
        {
            ChangeState(birdEnemy.MoveState);
        }
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
    }

    #endregion
}
