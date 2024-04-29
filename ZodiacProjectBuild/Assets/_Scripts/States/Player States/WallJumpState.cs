using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumpState : State
{
    [Header("Animation Clip")]
    public  AnimationClip   animClip;

    public override void Enter()
    {
        Animator.Play(animClip.name);
    }

    public override void Do()
    {
        if (core.collisionSensors.IsGrounded)
        {
            IsComplete = true;
        }
    }
}