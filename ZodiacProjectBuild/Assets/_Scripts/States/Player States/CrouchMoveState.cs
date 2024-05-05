using UnityEngine;

public class CrouchMoveState : State
{
    [Header("Animation Clip")]
    public AnimationClip animClip;

    public override void Enter()
    {
        base.Enter();

        core.collisionSensors.SetAllColliderHeight(0.8f);
        Animator.Play(animClip.name);
    }

    public override void Do()
    {
        base.Do();
        

    }

    public override void Exit()
    {
        base.Exit();

        core.collisionSensors.SetAllColliderHeight(1.6f);
    }
}