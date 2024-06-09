using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedState : State
{
    #region States

    [Header("States")]
    [SerializeField] IdleState idleState;
    [SerializeField] LandState landState;
    [SerializeField] CrouchIdleState crouchIdleState;
    [SerializeField] CrouchMoveState crouchMoveState;

    #endregion

    #region Callback Functions

    public override void Enter()
    {
        base.Enter();
    }

    public override void Do()
    {
        base.Do();

        
    }

    public override void FixedDo()
    {
        base.FixedDo();

        if (
            Body.velocity == new Vector2(0, 0)
            )
            Set(idleState);

        if (
            !core.collisionSensors.IsGrounded
            )
        {
            IsComplete = true;
            return;
        }
    }

    #endregion


    #region Functionality




    #endregion
}