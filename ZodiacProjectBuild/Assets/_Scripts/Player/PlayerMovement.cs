using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Player      _player;
    private Vector2     MoveInput   => _player.MoveInput;
    private PlayerData  Data        => _player.playerData;
    private Rigidbody2D Body        => _player.body;

    private float       _targetSpeed;
    

    #region Unity Callback Methods

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    #endregion


    #region Run Methods

    /// <summary>
    /// Applies a force to the player's <c>Rigidbody2D</c> component in the x-axis.
    /// </summary>
    /// <param name="lerpAmount">Smoothing amount for entry into the run state from other states.</param>
    public void Run(float lerpAmount)
    {
        float accelRate;

        // Calculates the desired target speed
        _targetSpeed = MoveInput.x * Data.runMaxSpeed;
        _targetSpeed = Mathf.Lerp(Body.velocity.x, _targetSpeed, lerpAmount);
  

        // if on contact with ground, set the correct acceleration and decceleration values
        if(_player.TimeLastOnGround > 0)
            accelRate = (Mathf.Abs(_targetSpeed) > 0.01f) ? 
            Data.runAccelAmount : Data.runDeccelAmount;
        // otherwise set the correct air acceleration and decceleration values
        else
            accelRate = (Mathf.Abs(_targetSpeed) > 0.01f) ? 
            Data.runAccelAmount * Data.airAcceleration : Data.runDeccelAmount * Data.airDecceleration;
    
        // if currently jumping or currently falling after a jump, AND the vertical velocity is less than threshold for jump hang    
        if  (
            (_player.IsJumping || _player.IsJumpFalling) &&
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
            _player.TimeLastOnGround > 0
            )
            accelRate = 0; // set the acceleration rate to 0
    
        // calculates the speed difference from the target speed and current speed
        float speedDiff     = _targetSpeed - Body.velocity.x;
        // calculates the movement required to reach the speed with current acceleration rate
        float movement      = speedDiff * accelRate;
        // applies a force to the rigidbody
        Body.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    /// <summary>
    /// Turns the player by scaling the body by -1.
    /// </summary>
    public void Turn()
    {
        Vector3 scale           = transform.localScale;
        scale.x                 *= -1;
        transform.localScale    = scale;

        _player.IsFacingRight   = !_player.IsFacingRight;
    }

    #endregion
    

    #region Jump Methods

    /// <summary>
    /// Function to make player jump.
    /// </summary>
    public void Jump()
    {
        // resets the timers to do with jumping
        _player.TimeLastPressedJump = 0;
        _player.TimeLastOnGround    = 0;

        float force = Data.jumpForce;

        if(Body.velocity.y < 0)
            force -= Body.velocity.y;

        // applies an upward impulse force 
        Body.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    #endregion
}