using UnityEngine;

public class LedgeClimbingState : State
{
    [Header("Animation Clip")]
    public AnimationClip animClip;

    public override void Enter()
    {
        base.Enter();
        Animator.Play(animClip.name);
    }

    public override void Do()
    {
        base.Do();
        
        
    }
}