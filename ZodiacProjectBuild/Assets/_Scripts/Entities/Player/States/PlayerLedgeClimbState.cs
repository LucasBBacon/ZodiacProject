using System;
using System.Collections;
using UnityEngine;

public class PlayerLedgeClimbState : PlayerState
{
    public bool IsLedgeHanging { get; private set; }
    public bool IsLedgeClimbing { get; private set; }

    Vector2 _detectedPosition;
    Vector2 _cornerPosition;
    Vector2 _startPosition;
    Vector2 _endPosition;
    Vector2 _workspace;

    public PlayerLedgeClimbState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        if (player.TrailRenderer.emitting)
            player.TrailRenderer.emitting = false;
        
        Movement.SetVelocityZero();
        player.transform.position = _detectedPosition;

        _cornerPosition = DetermineCornerPosition();

        _startPosition.Set(
            _cornerPosition.x - (Movement.FacingDirection * MoveData.StartOffset.x),
            _cornerPosition.y - MoveData.StartOffset.y
        );
        _endPosition.Set(
            _cornerPosition.x + (Movement.FacingDirection * MoveData.EndOffset.x),
            _cornerPosition.y + MoveData.EndOffset.y
        );
        player.transform.position = _startPosition;
    }

    

    public override void StateExit()
    {
        base.StateExit();

        IsLedgeHanging = false;

        if (IsLedgeClimbing)
        {
            player.transform.position = _endPosition;
            IsLedgeClimbing = false;
        }
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (IsAnimationFinished)
        {
            ChangeState(player.IdleState);
        }
        else
        {
            Movement.SetVelocityZero();
            player.transform.position = _startPosition;

            if (
                InputManager.instance.MoveInput.x == Movement.FacingDirection
                && IsLedgeHanging
                && !IsLedgeClimbing
                )
            {
                IsLedgeClimbing = true;
                Animator.SetBool("climbLedge", true);
            }
            else if (
                InputManager.instance.MoveInput.normalized.y == -1
                && IsLedgeHanging
                && !IsLedgeClimbing
                )
            {
                player.AirborneState.IsFalling = true;
                ChangeState(player.AirborneState);
            }
            else if (
                player.JumpState.LastPressedJumpTime > 0
                && !IsLedgeClimbing
                )
            {
                ChangeState(player.WallJumpState);
            }
        }
    }

    public override void AnimationFinishedTrigger()
    {
        base.AnimationFinishedTrigger();

        Animator.SetBool("climbLedge", false);
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();

        IsLedgeHanging = true;
    }

    #endregion


    #region Functionality

    public void SetDetectedPosition(Vector2 position)
    => _detectedPosition = position;

    Vector2 DetermineCornerPosition()
    {
        RaycastHit2D xHit = Physics2D.Raycast(
            CollisionSensors.WallCheck.position,
            Vector2.right * Movement.FacingDirection,
            CollisionSensors.WallCheckDistance,
            CollisionSensors.GroundMask
        );
        float xDist = xHit.distance;

        _workspace.Set((xDist + 0.015f) * Movement.FacingDirection, 0f);

        RaycastHit2D yHit = Physics2D.Raycast(
            CollisionSensors.LedgeCheckHorizontal.position + (Vector3)_workspace,
            Vector2.down,
            CollisionSensors.LedgeCheckHorizontal.position.y - CollisionSensors.WallCheck.position.y + 0.015f,
            CollisionSensors.GroundMask
        );
        float yDist = yHit.distance;

        _workspace.Set(
            CollisionSensors.WallCheck.position.x + (xDist * Movement.FacingDirection),
            CollisionSensors.LedgeCheckHorizontal.position.y - yDist
            );
        return _workspace;
    }

    #endregion
 
}