using UnityEngine;

public class WallGrabState : State
{
    [Header("Animation Clip")]
    public  AnimationClip   animClip;

    public override void Enter()
    {
        Animator.Play(animClip.name);
    }

    public override void Do()
    {
        if (core.collisionSensors.IsGrounded || UserInput.instance.GrabReleased)
        {
            IsComplete = true;
        }
    }
}