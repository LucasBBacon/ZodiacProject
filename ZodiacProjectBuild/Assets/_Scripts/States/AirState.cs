using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirState : State
{
    [Header("Animation Clip")]
    public  AnimationClip   animClip;

    [Header("States")]
    [SerializeField] private WallSlideState slideState;
    [SerializeField] private GroundedState groundedState;

    [HideInInspector] public bool IsJumping { get; private set; }
    [HideInInspector] public bool IsJumpFalling { get; private set; }
    [HideInInspector] public bool IsJumpCut { get; private set; }

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

        if (core.collisionSensors.IsGrounded)
        {
            Animator.speed = 1;
            Set(groundedState);
            IsComplete = true;
        }
    }

    #endregion

    public override void Exit()
    {
        Animator.speed = 1;
    }

    public void SetIsJumping() => IsJumping = true;
    public void SetIsJumpFalling() => IsJumpFalling = true;
    public void SetIsJumpCut() => IsJumpCut = true;
}