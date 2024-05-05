using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedState : State
{
    [Header("States")]
    public  IdleState   idleState;
    public  RunState    runState;
    public  LandState   landState;
    public  CrouchIdleState crouchIdleState;
    public  CrouchMoveState  crouchMoveState;

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
            if(UserInput.instance.MoveInput.y < 0)
            {
                if(UserInput.instance.MoveInput.x != 0)
                    Set(crouchMoveState);
                else
                    Set(crouchIdleState);
            }

            else if(UserInput.instance.MoveInput.y == 0 && !core.collisionSensors.IsCeiling)
            {
                if(UserInput.instance.MoveInput.y == 0 && Body.velocity.x != 0)
                {
                    Set(runState);
                }
                
                else
                    Set(idleState);
            }
        }
        
        else
            IsComplete = true;
    }
}