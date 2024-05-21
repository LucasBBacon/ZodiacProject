using UnityEngine;

public class WallControlState : State
{
    [Header("Animation Clip")]
    public  AnimationClip   animClip;

    [Header("States")]
    [SerializeField] private WallControlState wallControlState;

    public override void Enter()
    {
        base.Enter();
        Animator.Play(animClip.name);
        
    }

    public override void Do()
    {
        base.Do();
        
        
    }

    public override void Exit()
    {
        
    }
}