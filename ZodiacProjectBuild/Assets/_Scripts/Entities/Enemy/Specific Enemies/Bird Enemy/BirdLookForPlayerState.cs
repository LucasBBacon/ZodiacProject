public class BirdLookForPlayerState : LookForPlayerState
{
    BirdEnemy birdEnemy;

    public BirdLookForPlayerState(EnemyEntity entity, EntityStateMachine stateMachine, BirdEnemy birdEnemy) : base(entity, stateMachine)
    {
        this.birdEnemy = birdEnemy;
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        Animator.SetBool(BirdEnemy.LOOK_FOR_PLAYER, true);
    }

    public override void StateExit()
    {
        base.StateExit();

        Animator.SetBool(BirdEnemy.LOOK_FOR_PLAYER, false);
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
