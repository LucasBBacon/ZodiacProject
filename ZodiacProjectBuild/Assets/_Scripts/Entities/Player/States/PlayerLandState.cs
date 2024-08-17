public class PlayerLandState : PlayerState
{
    bool _isGrounded;
    bool _isCeiling;
    bool _isLedge;
    bool _isWall;

    public PlayerLandState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        player.JumpState.ResetJumps();
        player.JumpState.IsJumpCut = false;

        player.TrailRenderer.emitting = false;
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
            _isCeiling = CollisionSensors.IsCeiling;
            _isLedge = CollisionSensors.IsLedgeHorizontal;
            _isWall = CollisionSensors.IsWall;
        }
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (InputManager.instance.AttackInput && player.AttackState.CanAttack())
        {
            ChangeState(player.AttackState);
        }
        else if (player.JumpState.LastPressedJumpTime > 0 && player.JumpState.CanJump())
        {
            ChangeState(player.JumpState);
        }
        else if (!_isGrounded)
        {
            ChangeState(player.AirborneState);
        }
        else if (InputManager.instance.DashInput && player.DashState.CanDash() && player.DashEnabled)
        {
            ChangeState(player.DashState);
        }

        if (!IsExitingState)
        {
            if (InputManager.instance.MoveInput.x != 0)
            {
                ChangeState(player.RunState);
            }
            else if (IsAnimationFinished)
            {
                ChangeState(player.IdleState);
            }
        }
    }

    #endregion
}

