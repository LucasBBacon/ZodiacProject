using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWallJumpState : PlayerState
{
    public bool UseWallJumpMoveStats { get; set; }
    public float WallJumpPostBufferTimer { get; set; }

    bool _isAbilityDone;

    public PlayerWallJumpState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    #region Callback Methods

    public override void StateEnter()
    {
        base.StateEnter();

        _isAbilityDone = false;

        InitiateWallJump();
        ChangeState(player.AirborneState);
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
    
        Animator.SetFloat("yVelocity", Movement.CurrentVelocity.y);
        Animator.SetFloat("xVelocity", Mathf.Abs(Movement.CurrentVelocity.x));
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();

        player.AirborneState.WallJumpPhysics();

        player.Move(
            MoveData.WallJumpMoveAcceleration, 
            MoveData.WallJumpMoveDeceleration, 
            InputManager.instance.MoveInput
            );
    }

    #endregion

    
    #region Timers

    public void WallJumpTimers()
    {
        if (!ShouldApplyPostWallJumpBuffer())
            WallJumpPostBufferTimer -= Time.deltaTime;
    }

    #endregion

    #region Checks

    public void WallJumpChecks()
    {
        if (ShouldApplyPostWallJumpBuffer())
        {
            WallJumpPostBufferTimer = MoveData.WallJumpPostBufferTime;
        }

        if (InputManager.instance.JumpReleased)
        {
            WallJumpReleased();
        }
    }

    public bool CanWallJumpDueToPostBufferTimer()
    {
        Debug.Log(WallJumpPostBufferTimer);
        if (WallJumpPostBufferTimer > 0f)
            return true;

        return false;
    }

    void WallJumpReleased()
    {
        if (
            !player.WallSlideState.IsWallSliding
            && !CollisionSensors.IsWall
            && player.AirborneState.IsWallJumping
            )
        {
            if (
                player.AirborneState.IsWallJumping
                && Movement.CurrentVelocity.y > 0f
                )
            {
                if (player.AirborneState.IsPastWallJumpApexThreshold)
                {
                    player.AirborneState.IsPastWallJumpApexThreshold = false;
                    player.AirborneState.IsWallJumpFastFalling = false;
                    player.AirborneState.WallJumpFastFallTime = MoveData.TimeForUpwardsCancel;

                    Movement.SetVelocityY(0f);
                }
                else
                {
                    player.AirborneState.IsWallJumpFastFalling = true;
                    player.AirborneState.WallJumpFastFallReleaseSpeed = Movement.CurrentVelocity.y;
                }
            }
        }
    }

    public bool ShouldApplyPostWallJumpBuffer()
    {
        if (
            !CollisionSensors.IsGrounded
            && (CollisionSensors.IsWall || player.WallSlideState.IsWallSliding)
            )
            return true;
        else
            return false;
    }

    #endregion


    #region Functionality

    public void InitiateWallJump()
    {
        if (!player.AirborneState.IsWallJumping)
        {
            player.AirborneState.IsWallJumping = true;
            UseWallJumpMoveStats = true;
        }

        player.WallSlideState.StopWallSliding();

        player.AirborneState.ResetJumpValues();
        player.AirborneState.WallJumpTime = 0f;

        Movement.SetVelocityY(MoveData.InitialWallJumpVelocity);

        int dirMultiplier;
        Vector2 hitDir = CollisionSensors.LastWallHit.collider.ClosestPoint(CollisionSensors.BodyColl.bounds.center);
    
        if (hitDir.x > player.transform.position.x)
            dirMultiplier = -1;
        else
            dirMultiplier = 1;

        Movement.SetVelocityX(Mathf.Abs(MoveData.WallJumpDirection.x) * dirMultiplier);
    }

    #endregion
}
