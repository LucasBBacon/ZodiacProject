using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirState : State
{
    public  AnimationClip   animClip;
    //public  LandState       landState;

    private float jumpSpeed;

    public override void Enter() {
        Animator.Play(animClip.name);
        jumpSpeed = Body.velocity.y;
    }

    public override void Do() {
        //seek the animator to the frame based on our y velocity
        float time = Utility.Map(Body.velocity.y, jumpSpeed, -jumpSpeed, 0, 1, true);
        Animator.Play(animClip.name, 0, time);
        Animator.speed = 0;

        if (core.collisionSensors.IsGrounded) {
            Animator.speed = 1;
            IsComplete = true;
            //Set(landState, true);
        }
    }

    public override void Exit() {
        Animator.speed = 1;
    }
}