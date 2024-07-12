using System;
using UnityEngine;

public class PlayerWallJumpState : PlayerState
{
    /*

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        _player.JumpState.NumberOfJumpsUsed = 0;

        // Movement.SetVelocity(MoveStats.WallJumpVelocity, MoveStats.WallJumpAngle, wallJumpDirection);

        Movement.SetVerticalVelocity(MoveStats.WallJumpVelocity * MoveStats.WallJumpAngle.normalized.y);

        Movement.SetHorizontalVelocity
            (
                MoveStats.WallJumpVelocity * MoveStats.WallJumpAngle.normalized.x * wallJumpDirection
            );

        Movement.TurnCheck(wallJumpDirection);
        _player.AirborneState.IsWallJumping = true;

        _player.JumpState.NumberOfJumpsUsed = MoveStats.NumberOfJumpsAllowed;

        if (!_player.TrailRenderer.emitting)
            _player.TrailRenderer.emitting = true;

        // if (_player.JumpState.JumpReleasedDuringBuffer)
        // {
        //     _player.AirborneState.IsFastFalling = true;
        //     _player.AirborneState.FastFallReleaseSpeed = Movement.VerticalVelocity;
        // }

        ChangeState(_player.AirborneState);
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();

        Movement.Move(InputManager.MoveInput, MoveStats.MaxRunSpeed);
        
        // Body.velocity = new Vector2(Body.velocity.x, Movement.VerticalVelocity);
    }

    #endregion


    #region Checks

    public bool CanWallJump()
    {
        if (
            _player.JumpState.JumpBufferTimer > 0f &&
            !_player.AirborneState.IsWallJumping &&
            (CollisionSensors.IsWall || _player.JumpState.CoyoteTimer > 0f)
        )
        {
            _player.JumpState.JumpBufferTimer = 0f;

            _player.AirborneState.IsFastFalling = false;
            _player.AirborneState.IsFalling = false;

            
            return true;
        }
        
        return false;
    }

    public void DetermineWallJumpDirection(bool isTouchingRightWall)
    {
        if (isTouchingRightWall)
            wallJumpDirection = -(Movement.IsFacingRight ? 1 : -1);
        else
            wallJumpDirection = Movement.IsFacingRight ? 1 : -1;
    }

    #endregion

    */

    public float WallJumpPostBufferTimer { get; set; }

    int wallJumpDirection;

    public PlayerWallJumpState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();

        _player.AirborneState.InitiateWallJump();
        ChangeState(_player.AirborneState);
    }

    public override void StateExit()
    {
        base.StateExit();

    }

    public override void StateUpdate()
    {
        base.StateUpdate();
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();

        _player.AirborneState.WallJumpPhysics();

        Movement.Move(InputManager.MoveInput, MoveStats.MaxRunSpeed);
    }

    #region Checks

    public bool CanWallJumpDueToPostBufferTimer()
    {
        if (WallJumpPostBufferTimer > 0f)
            return true;
        
        return false;
    }

    public void WallJumpChecks()
    {
        if (ShouldApplyPostWallJumpBuffer())
        {
            WallJumpPostBufferTimer = MoveStats.WallJumpPostBufferTime;
        }

        if (InputManager.JumpReleased)
        {
            _player.AirborneState.WallJumpWasReleased();
        }
    }

    private bool ShouldApplyPostWallJumpBuffer()
    {
        if (
            CollisionSensors.IsGrounded
            && (CollisionSensors.IsWall || _player.WallSlideState.IsWallSliding)
            )
            return true;
        else
            return false;
    }

    

    public void WallJumpTimers()
    {
        if(!ShouldApplyPostWallJumpBuffer())
        {
            WallJumpPostBufferTimer -= Time.deltaTime;
        }
    }

    #endregion
}
