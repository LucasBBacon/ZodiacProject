using System;
using System.Collections;
using UnityEngine;

public class PlayerLedgeClimbState : PlayerState
{
    public bool IsLedgeGrabbing { get; private set; }
    public bool IsLedgeClimbing { get; private set; }
    public bool IsLedgeFalling { get; set; }
    //public float xDist;

    Vector2 detectedPosition;
    Vector2 cornerPosition;
    Vector2 startPosition;
    Vector2 endPosition;

    IEnumerator climbingCouroutine;

    public PlayerLedgeClimbState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        if (_player.TrailRenderer.emitting)
            _player.TrailRenderer.emitting = false;
        
        if (!IsLedgeFalling)
        {
            Movement.SetVelocityZero();
            Movement.VerticalVelocity = 0;
            Animator.SetBool("LedgeHang", true);

            IsLedgeGrabbing = true;

            _player.transform.position = detectedPosition;

            cornerPosition = CollisionSensors.LedgeCheck.transform.position;

            startPosition.Set
                (
                    cornerPosition.x - (Movement.FacingDirection * MoveStats.StartOffset.x),
                    cornerPosition.y - MoveStats.StartOffset.y
                );
            endPosition.Set
                (
                    cornerPosition.x + (Movement.FacingDirection * MoveStats.EndOffset.x),
                    cornerPosition.y + MoveStats.EndOffset.y
                );

            _player.transform.position = startPosition;
        }

        else
        {
            ChangeState(_player.AirborneState);
        }
        
    }

    public override void StateExit()
    {
        base.StateExit();

        IsLedgeGrabbing = false;
        Animator.SetBool("LedgeHang", false);
        Animator.SetBool("LedgeClimb", false);

        if (IsLedgeClimbing)
        {  
            _player.transform.position = endPosition;
            IsLedgeClimbing = false;
            //ChangeState(_player.AirborneState);
        }
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        Movement.SetVelocityZero();
        _player.transform.position = startPosition;

        if (
            (InputManager.MoveInput.y == 1 || InputManager.MoveInput.x == Movement.FacingDirection) &&
            IsLedgeGrabbing &&
            !IsLedgeClimbing
            )
        {
            IsLedgeClimbing = true;
            
            climbingCouroutine = ClimbingLedge(endPosition, 0.5f);
            _player.StartCoroutine(climbingCouroutine);
            
        }

        else if (
            InputManager.MoveInput.y == -1 &&
            IsLedgeGrabbing &&
            !IsLedgeClimbing
            )
        {  
            IsLedgeFalling = true;
            IsLedgeGrabbing = false;

            ChangeState(_player.AirborneState);
        }

        else if (
            InputManager.JumpJustPressed &&
            IsLedgeGrabbing &&
            !IsLedgeClimbing
            )
        {
            IsLedgeGrabbing = false;

            //_player.WallJumpState.DetermineWallJumpDirection(true);
            ChangeState(_player.WallJumpState);
        }
    }

    #endregion

    
    #region Functionality

    public void SetDetectedPosition(Vector2 position) => detectedPosition = position;


    public IEnumerator ClimbingLedge(Vector2 endPosition, float duration)
    {
        Animator.SetBool("LedgeClimb", true);
        IsLedgeClimbing = true;
        
        //float time = 0;

        Vector2 startPos = _player.transform.position;

        CollisionSensors.UseGroundChecks = false;

        // while (time < duration)
        // {
        //     _player.transform.position = Vector2.Lerp(startPos, endPosition, time / duration);

        //     time += Time.deltaTime;

        //     yield return null;
        // }

        yield return new WaitForSeconds(duration);

        _player.transform.position = endPosition;

        CollisionSensors.UseGroundChecks = true;

        IsLedgeFalling = false;
        IsLedgeGrabbing = false;
        IsLedgeClimbing = false;

        _player.AirborneState.IsFalling = true;

        ChangeState(_player.AirborneState);
    }

    #endregion
 
}