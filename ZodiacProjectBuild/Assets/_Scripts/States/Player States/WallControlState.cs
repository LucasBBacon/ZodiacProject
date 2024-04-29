using UnityEngine;

public class WallControlState : State
{
    [Header("Animation Clip")]
    public  AnimationClip   animClip;

    [Header("States")]
    [SerializeField] private WallControlState wallControlState;

    private float jumpSpeed;

    public override void Enter()
    {
        Animator.Play(animClip.name);
        jumpSpeed = Body.velocity.y;
    }

    public override void Do()
    {
        
    }

    public override void Exit()
    {
        Animator.speed = 1;
    }
}