using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    public AnimationClip animClip;

    public override void Enter()
    {
        base.Enter();

        Animator.Play(animClip.name);
        //print("hello from idle state");
    }

    public override void Do()
    {
        base.Do();

        if(!core.collisionSensors.IsGrounded)
            IsComplete = true;
    }

    public override void Exit()
    {
        base.Exit();
    }
}