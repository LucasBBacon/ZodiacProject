using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirState : State
{
    [Header("Animation Clip")]
    public  AnimationClip   animClip;

    #region States

    [Header("States")]
    [SerializeField] JumpState jumpState;
    [SerializeField] LandState landState;
    [SerializeField] RunState runState;
    [SerializeField] WallControlState wallControlState;
    [SerializeField] WallJumpState wallJumpState;
    [SerializeField] DashState dashState;

    #endregion


    #region Blackboard Variables

    [HideInInspector] public bool IsJumping { get; private set; }
    [HideInInspector] public bool IsJumpFalling { get; private set; }
    [HideInInspector] public bool IsJumpCut { get; private set; }

    #endregion

    private float jumpSpeed;

    #region Callback Functions

    public override void Enter()
    {
        Animator.Play(animClip.name);
        jumpSpeed = Body.velocity.y;
    }

    public override void Do()
    {
        float time = Utilities.MappingUtil.Map(Body.velocity.y, jumpSpeed, -jumpSpeed, 0, 1, true);
        Animator.Play(animClip.name, 0, time);
        Animator.speed = 0;

        if (
            core.collisionSensors.IsGrounded &&
            Body.velocity.y < Mathf.Epsilon
            )
        {
            IsJumping = false;
            IsJumpFalling = false;
            IsJumpCut = false;

            Set(landState);

            IsComplete = true;
        }


        if (
            IsJumping
            )
        {
            wallControlState.SetWallJumping(false);
            
            IsJumpCut = false;
            IsJumpFalling = false;
        }

        if (
            IsJumping && 
            Body.velocity.y < Mathf.Epsilon
            )
        {
            IsJumping = false;
            IsJumpFalling = true;
            IsJumpCut = false;
        }

        if (
            (jumpState.CanJumpCut() || wallJumpState.CanWallJumpCut()) &&
            UserInput.instance.JumpReleased
        )
        {
            IsJumpCut = true;
        }

        if (
            !IsJumping &&
            Data.TimeLastOnGround > 0
            )
        {
            IsJumpCut = false;
            IsJumpFalling = false;
        }
    }

    public override void FixedDo()
    {
        base.FixedDo();

        if(!dashState.IsDashing)
            runState.Run(1);
    }

    public override void Exit()
    {
        base.Exit();

        Animator.speed = 1;

        // IsJumping = false;
        // IsJumpFalling = false;
        // IsJumpCut = false;
    }
    
    #endregion


    #region Checks

    public void SetIsJumping(bool value) => IsJumping = value;
    public void SetIsJumpFalling(bool value) => IsJumpFalling = value;
    public void SetIsJumpCut(bool value) => IsJumpCut = value;

    #endregion


    #region Functionality

    

    #endregion
}