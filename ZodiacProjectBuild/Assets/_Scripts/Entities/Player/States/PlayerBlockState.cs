public class PlayerBlockState : PlayerState
{
    bool _isAbilityDone;

    bool _isGrounded;

    bool _isBlocking;

    public PlayerBlockState(
        Player player,
        PlayerStateMachine stateMachine,
        string animBoolName
        ) : base(player, stateMachine, animBoolName)
    {
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        Movement.SetVelocityZero();

        _isAbilityDone = false;
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSensors)
        {
            _isGrounded = CollisionSensors.IsGrounded;
        }
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (IsAnimationFinished)
        {
            if (
                _isGrounded
                && Movement.CurrentVelocity.y < 0.01f
                )
            {
                ChangeState(player.IdleState);
            }
            else
            {
                ChangeState(player.AirborneState);
            }
        }
    }

    #endregion
}