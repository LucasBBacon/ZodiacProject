using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Player      _player;
    private Vector2     MoveInput   => _player.MoveInput;
    private PlayerData  Data        => _player.Data;
    private Rigidbody2D Body        => _player.body;
    [SerializeField] private GameObject colliderTransform;

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

        Vector3 colScale = colliderTransform.transform.localScale;
        colScale.x *= -1;
        colliderTransform.transform.localScale = colScale;

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

    public void WallJump(int dir)
    {
        _player.TimeLastPressedJump = 0;
        _player.TimeLastOnGround    = 0;
        _player.TimeLastOnRightWall = 0;
        _player.TimeLastOnLeftWall  = 0;

        Vector2 force = new Vector2
            (
                Data.wallJumpForce.x,
                Data.wallJumpForce.y
            );
        force.x *= dir;
        
        if(Mathf.Sign(Body.velocity.x) != Mathf.Sign(force.x))
            force.x -= Body.velocity.x;
        
        if(Body.velocity.y < 0)
            force.y -= Body.velocity.y;

        Body.AddForce(force, ForceMode2D.Impulse);
    }

    #endregion


    #region Slide Method

    public void Slide()
    {
        float speedDiff = Data.slideSpeed - Body.velocity.y;
        float movement = speedDiff * Data.slideAcceleration;

        movement = Mathf.Clamp
            (
                movement,
                -Mathf.Abs(speedDiff) * (1 / Time.fixedDeltaTime),
                Mathf.Abs(speedDiff) * (1 / Time.fixedDeltaTime)
            );
        
        Body.AddForce(movement * Vector2.up);
    }

    public void HoldPosition(Vector2 position)
    {
        if(position != new Vector2(-0.5f, 0f) && position != new Vector2(0.5f, 0f))
        {
            Body.gameObject.transform.position = position;   
            Body.velocity = new Vector2(0f, 0f);
        }
    }

    #endregion


    #region Dash Method

    public IEnumerator StartDash(Vector2 dir)
    {
        _player.TimeLastOnGround    = 0;
        _player.TimeLastPressedDash = 0;

        float startTime             = Time.time;

        _player.DashesLeft--;
        _player.IsDashAttacking     = true;

        _player.SetGravityScale(0);


        while(Time.time - startTime <= Data.dashAttackTime)
        {
            Body.velocity = dir.normalized * Data.dashSpeed;

            yield return null;
        }

        startTime               = Time.time;
        _player.IsDashAttacking = false;

        _player.SetGravityScale(Data.gravityScale);
        Body.velocity = Data.dashEndSpeed * dir.normalized;

        while(Time.time - startTime <= Data.dashEndTime)
            yield return null;
        
        _player.IsDashing = false;
    }

    public IEnumerator DashRefill(int amount)
    {
        _player.DashRefilling = true;

        yield return new WaitForSeconds(Data.dashRefillTime);

        _player.DashRefilling = false;
        _player.DashesLeft = Mathf.Min(Data.dashAmount, _player.DashesLeft + amount);
    }

    #endregion
}