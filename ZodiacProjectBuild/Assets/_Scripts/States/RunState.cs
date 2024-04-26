using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : State
{
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