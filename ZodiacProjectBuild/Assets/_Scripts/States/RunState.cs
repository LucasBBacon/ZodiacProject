using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : State
{
    [Header("Animation Clip")]
    public AnimationClip animClip;

    [Space(20)]

    #region States

    [Header("States")]
    [SerializeField] AirState airState;
    [SerializeField] IdleState idleState;
    [SerializeField] CrouchMoveState crouchMoveState;

    #endregion

    #region Blackboard Variables


    #endregion


    #region Callback Functions

    public override void Enter()
    {
        base.Enter();
        Animator.Play(animClip.name);
    }

    public override void Do()
    {
        base.Do();

        // if (
        //     Body.velocity.x != 0 ||
        //     !core.collisionSensors.IsGrounded
        //     )
        // {
        //     IsComplete = true;
        //     return;
        // }
    }

    public override void FixedDo()
    {
        base.FixedDo();

        Run(1);
    }

    #endregion


    #region Functionality

    /// <summary>
    /// Applies a force to the player's <c>Rigidbody2D</c> component in the x-axis.
    /// </summary>
    /// <param name="lerpAmount">Smoothing amount for entry into the run state from other states.</param>
    public void Run(float lerpAmount)
    {
        float accelRate;

        // Calculates the desired target speed
        float _targetSpeed = UserInput.instance.MoveInput.x * Data.runMaxSpeed;
        _targetSpeed = Mathf.Lerp(Body.velocity.x, _targetSpeed, lerpAmount);
  

        // if on contact with ground, set the correct acceleration and decceleration values
        if(Data.TimeLastOnGround > 0)
            accelRate = (Mathf.Abs(_targetSpeed) > 0.01f) ? 
            Data.runAccelAmount : Data.runDeccelAmount;
        // otherwise set the correct air acceleration and decceleration values
        else
            accelRate = (Mathf.Abs(_targetSpeed) > 0.01f) ? 
            Data.runAccelAmount * Data.airAcceleration : Data.runDeccelAmount * Data.airDecceleration;
    
        // if currently jumping or currently falling after a jump, AND the vertical velocity is less than threshold for jump hang    
        if  (
            (airState.IsJumping || airState.IsJumpFalling) &&
            Mathf.Abs(Body.velocity.y) < Data.jumpHangTimeThreshold
        )
        {
            // set the correct hang acceleration rate and target speed
            accelRate       *= Data.jumpHangAccelerationMultiplier;
            _targetSpeed    *= Data.jumpHangMaxSpeedMultiplier;
        }
        
        // if is set to conserve momentum, AND the player velocity is greater than target speed, 
        // AND the direction of movement is hte same as the target speed AND the target speed is greater than 0, AND the player is currently touching the ground
        if  (
            Data.doConserveMomentum && 
            Mathf.Abs(Body.velocity.x) > Mathf.Abs(_targetSpeed) && 
            Mathf.Sign(Body.velocity.x) == Mathf.Sign(_targetSpeed) && 
            Mathf.Abs(_targetSpeed) > 0.01f && 
            Data.TimeLastOnGround > 0
            )
            accelRate = 0; // set the acceleration rate to 0
    
        // calculates the speed difference from the target speed and current speed
        float speedDiff     = _targetSpeed - Body.velocity.x;
        // calculates the movement required to reach the speed with current acceleration rate
        float movement      = speedDiff * accelRate;
        //Debug.Log(movement);
        // applies a force to the rigidbody
        Body.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    #endregion
}
