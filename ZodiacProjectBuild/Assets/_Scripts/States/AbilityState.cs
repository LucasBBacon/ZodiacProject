using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class AbilityState : State
{
    [Header("Animation Clip")]
    public  AnimationClip   animClip;

    #region States

    [Header("States")]
    [SerializeField] DashState dashState;

    #endregion


    #region Blackboard Variables

    [HideInInspector] public bool IsDashing { get; private set; }

    #endregion


    #region Callback Functions

    public override void Enter()
    {
        
    }

    public override void Do()
    {
        if
        (
            dashState.CanDash()               &&
            Data.TimeLastPressedDash > 0
        )
        {
            IsDashing = true;

            // is dashing
            Set(dashState);
        }
    }

    public override void FixedDo()
    {
        base.FixedDo();

        
    }

    public override void Exit()
    {
        
    }
    
    #endregion


    #region Checks

    

    #endregion

    
}