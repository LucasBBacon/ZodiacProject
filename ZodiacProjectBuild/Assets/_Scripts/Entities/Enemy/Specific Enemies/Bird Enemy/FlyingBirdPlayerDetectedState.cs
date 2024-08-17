public class FlyingBirdPlayerDetectedState : PlayerDetectedState
{
    FlyingBirdEnemy enemy;

    bool _shouldAttack;

    public FlyingBirdPlayerDetectedState(
        EnemyEntity entity,
        EntityStateMachine stateMachine,
        FlyingBirdEnemy enemy,
        string animBoolName
        ) : base(entity, stateMachine, animBoolName)
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

        if (
            PerformLongRangeAction
            )
        {
            ChangeState(enemy.ChargeState);
        }

        else if (
            !IsPlayerInMaxAgroRange
            )
        {
            ChangeState(enemy.LookForPlayerState);
        }

        else if (
            !IsDetectingLedge
            )
        {
            Movement?.Turn();
            ChangeState(enemy.MoveState);
        }
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
    }

    #endregion
}
