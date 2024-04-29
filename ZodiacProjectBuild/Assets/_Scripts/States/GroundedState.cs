using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedState : State
{
    [Header("States")]
    public  IdleState   idleState;
    public  RunState    runState;
    public  LandState   landState;

    public override void Enter()
    {
        base.Enter();
        
        Set(landState);
    }

    public override void Do()
    {
        base.Do();

        if(core.collisionSensors.IsGrounded)
        {
            if(Body.velocity.x != 0)
            {
                Set(runState);
            }
            else
                Set(idleState);
        }
        
        else
            IsComplete = true;
    }
}