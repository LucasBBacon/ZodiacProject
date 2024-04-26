using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Core 
{
    [HideInInspector]
    public  PlayerMovement  playerMovement;

    [Space(20)]

    public  PlayerData      playerData;

    [Space(20)]

    #region State Definitions

    [Header("States")]
    public  GroundedState   groundedState;
    public  JumpState       jumpState;
    public  AirState        airState;
    
    #endregion

    [Space(20)]

    #region State Parameters

    [Header("State Parameters")]

    public  bool            IsFacingRight;

    // Jump
    public  bool            IsJumping           { get; private set; }
    public  bool            IsJumpFalling       { get; private set; }
    public  bool            IsJumpCut           { get; private set; }

    #endregion

    [Space(5)]

    #region Input Parameters

    [Header("Input Parameters")]

    public  Vector2         MoveInput;
    public  float           TimeLastPressedJump;

    #endregion


    #region Timer Parameters

    public  float           TimeLastOnGround;

    #endregion


    #region Unity Callback Methods

    private void Start()
    {

        SetupInstances();
        Set(airState);
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        UpdateTimers();
        CheckInput();
        //UpdateJump();
        CalculateGravity();
        if(!IsJumping) CheckGround();  
        SelectState();
    }

    private void FixedUpdate()
    {
        playerMovement.Run(1);
    }

    #endregion
    

    #region State Selection

    private void SelectState()
    {
        // if currently jumping AND falling, player is falling, and not jumping
        if(IsJumping && body.velocity.y < 0)
        {   
            IsJumping       = false;
            IsJumpFalling   = true;
            Set(airState);
        }

        // if touching ground, AND not jumping, player is not jump cutting and not falling
        if(TimeLastOnGround > 0 && !IsJumping)
        {
            IsJumpCut       = false;
            IsJumpFalling   = false;
        }

        // if can jump, AND jump key has pressed, player is jumping, and not jump cutting, and not falling
        if(CanJump() && TimeLastPressedJump > 0)
        {
            IsJumping       = true;
            IsJumpCut       = false;
            IsJumpFalling   = false;
            
            // make player jump
            Set(jumpState);
            playerMovement.Jump();
        }

        if(!IsJumping)
        {
            if(collisionSensors.IsGrounded)
            {
                Set(groundedState);
            }
            else
            {
                Set(airState);
            }
        }
        
        else
            Set(airState);

        state.DoBranch();
    }

    #endregion


    #region Timer Methods

    /// <summary>
    /// Updates the various timers in the code with Time.deltaTime.
    /// </summary>
    public void UpdateTimers()
    {
        TimeLastOnGround    -= Time.deltaTime;
        TimeLastPressedJump -= Time.deltaTime;
    }

    #endregion


    #region Input Handler

    /// <summary>
    /// Fetches the inputs from <c>UserInput</c>.
    /// </summary>
    private void CheckInput()
    {
        MoveInput = UserInput.instance.MoveInput;
        if(MoveInput.x != 0)   CheckDirectionToFace(MoveInput.x > 0);

        if(UserInput.instance.JumpJustPressed)  OnJumpInput();
        if(UserInput.instance.JumpReleased)     OnJumpUpInput();
    }

    #endregion


    #region State Parameter Update

    /// <summary>
    /// Updates the various jump state parameters.
    /// </summary>
    private void UpdateJump()
    {
        // if currently jumping AND falling, player is not jumping and not falling
        if(IsJumping && body.velocity.y < 0)
        {
            IsJumping       = false;
            IsJumpFalling   = true;
        }

        // if touching ground, AND not jumping, player is not jump cutting and not falling
        if(TimeLastOnGround > 0 && !IsJumping)
        {
            IsJumpCut       = false;
            IsJumpFalling   = false;
        }

        // if can jump, AND jump key has pressed, player is jumping, and not jump cutting, and not falling
        if(CanJump() && TimeLastPressedJump > 0)
        {
            IsJumping       = true;
            IsJumpCut       = false;
            IsJumpFalling   = false;
            
            // make player jump
            playerMovement.Jump();
        }
    }

    #endregion


    #region Gravity

    /// <summary>
    /// Calculates and sets gravity strength value based on player state.
    /// </summary>
    private void CalculateGravity()
    {
        // if currently falling downwards, AND the down button is pressed, change gravity to fast fall gravity
        if(body.velocity.y < 0 && MoveInput.y < 0)
        {
            SetGravityScale(playerData.gravityScale * playerData.fallGravityMultiplierFast);
        }

        // if the jump is cut, change gravity to jump cut gravity
        else if(IsJumpCut)
        {
            SetGravityScale(playerData.gravityScale * playerData.jumpCutGravityMultiplier);
            // sets x veloctiy to input velocity so player is able to move in the air
            body.velocity = new Vector2
                (
                    body.velocity.x,
                    Mathf.Max(body.velocity.y, -playerData.fallMaxSpeed)
                );
        }

        // if the player is currently falling downwards, change gravity to fall gravity
        else if(body.velocity.y < 0)
        {
            SetGravityScale(playerData.gravityScale * playerData.fallGravityMultiplier);
            // sets x veloctiy to input velocity so player is able to move in the air
            body.velocity = new Vector2
                (
                    body.velocity.x,
                    Mathf.Max(body.velocity.y, -playerData.fallMaxSpeed)
                );
        }

        // otherwise, change gravity to default gravity
        else
        {
            SetGravityScale(playerData.gravityScale);
        }
    }

    /// <summary>
    /// Sets RigidBody2D gravity scale.
    /// </summary>
    /// <param name="gravityScale">Gravity scale to be set.</param>
    private void SetGravityScale(float gravityScale)    => body.gravityScale = gravityScale;

    #endregion


    #region Collision Methods

    /// <summary>
    /// Resets the ground timer based on ground check collisions.
    /// </summary>
    private void CheckGround()
    {
        if(collisionSensors.IsGrounded)
        {
            //if(LastOnGroundTime < -0.1f)
            TimeLastOnGround = playerData.coyoteTime;
        }
    }

    #endregion


    #region Input Callbacks

    /// <summary>
    /// Method called when jump button is pressed.
    /// </summary>
    public void OnJumpInput()
    {
        // resets the jump timer
        TimeLastPressedJump = playerData.jumpInputBufferTime;
    }

    /// <summary>
    /// Method called when jump button is released.
    /// </summary>
    public void OnJumpUpInput()
    {
        // checks if the jump can be cut, and cuts it
        if(CanJumpCut())
            IsJumpCut = true;
    }

    #endregion


    #region Check Methods

    /// <summary>
    /// Checks and sets the players facing direction.
    /// </summary>
    /// <param name="isMovingRight">If the player is moving right.</param>
    public void CheckDirectionToFace(bool isMovingRight)
    {
        // if the player is not facing the direction of their movement, call turn method
        if(isMovingRight != IsFacingRight)
            playerMovement.Turn();
    }

    /// <summary>
    /// Checks if player is able to jump.
    /// </summary>
    /// <returns>True if player is able to jump</returns>
    private bool CanJump()      => TimeLastOnGround > 0 && !IsJumping;  // if the player has touched the ground and is not currently jumping, return true
    
    /// <summary>
    /// Checks if the player is able to cut their jump.
    /// </summary>
    /// <returns>True if the player is able to jump cut.</returns>
    private bool CanJumpCut()   => IsJumping && body.velocity.y > 0;    // if the player is currently jumping, and their upwards velocity is positive, return true

    #endregion
}