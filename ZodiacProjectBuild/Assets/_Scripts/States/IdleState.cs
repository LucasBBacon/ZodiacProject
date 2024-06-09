using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    [Header("Animation Clip")]
    public  AnimationClip   animClip;

    #region States

    

    #endregion


    #region Callback Functions

    public override void Enter()
    {
        base.Enter();

        Body.velocity = new Vector2(0f, 0f);

        Animator.Play(animClip.name);
    }

    public override void Do()
    {
        base.Do();

        if (
            Body.velocity != new Vector2(Mathf.Epsilon, Mathf.Epsilon)
            )
        {
            IsComplete = true;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    #endregion
}