using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    #region Blackboard Variables

    public bool IsWallSliding { get; private set; }
    public bool IsWallSlideFalling { get; set; }

    #endregion

    public PlayerWallSlideState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        _player.AirborneState.ResetJumpValues();
        _player.AirborneState.ResetWallJumpValues();

        IsWallSliding = true;
        IsWallSlideFalling = false;

        if (_player.MoveStats.ResetJumpsOnWallSlide)
        {
            _player.JumpState.ResetJumps();
        }
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (ShouldStopWallSliding())
        {
            IsWallSlideFalling = true;
            StopWallSliding();

            ChangeState(_player.AirborneState);
        }

        else if (
            _player.AirborneState.HasLanded()
            )
        {
            ChangeState(_player.IdleState);
        }

        else if (
            InputManager.JumpJustPressed
            && _player.WallJumpState.CanWallJumpDueToPostBufferTimer()
            )
        {  
            //_player.WallJumpState.DetermineWallJumpDirection(CollisionSensors.IsWallFront);
            _player.AirborneState.UseWallJumpMoveStats = true;

            ChangeState(_player.WallJumpState);     
        }

        else if (
            !CollisionSensors.IsWall || 
            (InputManager.MoveInput.x != Movement.FacingDirection)
            )
        {
            ChangeState(_player.AirborneState);
        }
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();

        Movement.SetVerticalVelocity(Mathf.Lerp(Movement.VerticalVelocity, -MoveStats.WallSlideVelocity, MoveStats.WallSlideDecelerationSpeed * Time.fixedDeltaTime));
    
        Movement.Move(InputManager.MoveInput, MoveStats.MaxRunSpeed);
    }

    #endregion


    #region Checks

    public bool ShouldWallSlide()
    {
        if (
            CollisionSensors.IsWall
            && !CollisionSensors.IsGrounded
            && !_player.DashState.IsDashing
            )
        {
            if (
                Movement.VerticalVelocity < 0f &&
                !IsWallSliding
                )
            {
                return true;
            }
        }

        return false;
    }

    public bool ShouldStopWallSliding()
    => IsWallSliding
    && CollisionSensors.IsWall
    && !CollisionSensors.IsGrounded
    && !_player.AirborneState.IsWallSlideFalling;

    #endregion


    #region Functionality

    public void StopWallSliding()
    {
        if (IsWallSliding)
        {
            _player.JumpState.NumberOfJumpsUsed++;

            IsWallSliding = false;
        }
    }

    #endregion
}