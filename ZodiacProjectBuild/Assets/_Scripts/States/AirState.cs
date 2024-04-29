using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirState : State
{
    [Header("Animation Clip")]
    public  AnimationClip   animClip;

    [Header("States")]
    [SerializeField] private WallSlideState slideState;

    private float jumpSpeed;

    public override void Enter()
    {
        Animator.Play(animClip.name);
        jumpSpeed = Body.velocity.y;
    }

    public override void Do()
    {
        float time = Utility.Map(Body.velocity.y, jumpSpeed, -jumpSpeed, 0, 1, true);
        Animator.Play(animClip.name, 0, time);
        Animator.speed = 0;

        if (core.collisionSensors.IsGrounded)
        {
            Animator.speed = 1;
            IsComplete = true;
        }
        // else if(core.collisionSensors.IsWallBack || core.collisionSensors.IsWallFront)
        // {
        //     Animator.speed = 1;
        //     Set(slideState);
        // }
    }

    public override void Exit()
    {
        Animator.speed = 1;
    }
}