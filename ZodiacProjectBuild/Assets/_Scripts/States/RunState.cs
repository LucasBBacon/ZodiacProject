using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : State
{
    [Header("Animation Clip")]
    public AnimationClip animClip;

    

    #region Callback Functions

    public override void Enter()
    {
        base.Enter();
        Animator.Play(animClip.name);
        
    }

    public override void Do()
    {
        base.Do();
        
        if(!core.collisionSensors.IsGrounded)
        {
            IsComplete = true;
        }
    }

    public override void FixedDo()
    {
        base.FixedDo();

        Movement.Run(1);
    }

    #endregion



}
