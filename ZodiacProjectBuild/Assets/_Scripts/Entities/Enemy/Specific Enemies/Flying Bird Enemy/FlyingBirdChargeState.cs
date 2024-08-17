public class FlyingBirdChargeState : ChargeState
{
    FlyingBirdEnemy enemy;

    public FlyingBirdChargeState(EnemyEntity entity, EntityStateMachine stateMachine, FlyingBirdEnemy birdEnemy, string animBoolName) : base(entity, stateMachine, animBoolName)
    {
        this.enemy = birdEnemy;
        
    }

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
            !IsDetectingLedge ||
            IsDetectingWall
            )
        {
            ChangeState(enemy.LookForPlayerState);
        }

        else if (ChargeTimeOver)
        {
            if (
                IsPlayerInMinAgroRange
                )
            {
                ChangeState(enemy.PlayerDetectedState);
            }
            else
            {
                ChangeState(enemy.LookForPlayerState);
            }
        }
    }
}
