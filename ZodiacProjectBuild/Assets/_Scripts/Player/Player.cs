using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Player : Core 
{
    [HideInInspector]
    public  PlayerMovement  playerMovement;

    [Space(20)]

    public  PlayerData      Data;

    [Space(20)]

    #region State Definitions

    [Header("States")]
    public  GroundedState   groundedState;
    public  AirState        airState;
    public  JumpState       jumpState;
    public  DashState       dashState;
    public  WallSlideState  slideState;
    
    #endregion

    [Space(20)]

    #region State Parameters

    [Header("State Parameters")]

    public  bool            IsFacingRight;

    // Jump
    public  bool            IsJumping           { get; private set; }
    public  bool            IsJumpFalling       { get; private set; }
    public  bool            IsJumpCut           { get; private set; }

    // Walljump
    public  bool            IsWallJumping       { get; private set; }
    public  bool            IsSliding           { get; private set; }
    public  float           TimeWallJumpStart   { get; private set; }
    public  int             LastWallJumpDir     { get; private set; }

    // Dash
    public  bool            IsDashing;
    public  int             DashesLeft;
    public  bool            DashRefilling;
    public  Vector2         LastDashDir;
    public  bool            IsDashAttacking;

    #endregion

    [Space(5)]

    #region Input Parameters

    [Header("Input Parameters")]

    public  Vector2         MoveInput;
    public  float           TimeLastPressedJump;
    public  float           TimeLastPressedDash;

    #endregion


    #region Timer Parameters

    public  float           TimeLastOnGround;
    public  float           TimeLastOnWall;
    public  float           TimeLastOnRightWall;
    public  float           TimeLastOnLeftWall;

    #endregion


    #region Unity Callback Methods

    private void Start()
    {
        IsFacingRight = true;
        SetGravityScale(Data.gravityScale);
        SetupInstances();
        Set(airState);
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        UpdateTimers();
        CheckInput();
        if(!IsDashAttacking)
            CalculateGravity();
        if(!IsJumping)
            CheckCollisions();  
        SelectState();
    }

    private void FixedUpdate()
    {
        if(!IsDashing)
        {
            if(IsWallJumping)
                playerMovement.Run(Data.wallJumpRunLerp);
            else
                playerMovement.Run(1);
        }
        else if(IsDashAttacking)
            playerMovement.Run(Data.dashEndRunLerp);

        if(IsSliding)
            playerMovement.Slide();
    }

    #endregion
    

    #region State Selection

    /// <summary>
    /// Select the correct state
    /// </summary>
    private void SelectState()
    {
        // if currently jumping AND falling, player is falling, and not jumping
        if
        (
            IsJumping           &&
            body.velocity.y < 0 
        )
        {   
            IsJumping       = false;
            IsJumpFalling   = true;

            Set(airState);
            // player is no longer jumping, is falling, set in air state
        }

        // if currently wall jumping AND the time that its been wall jumping is over wall jumping time
        if
        (
            IsWallJumping                                       &&
            Time.time - TimeWallJumpStart > Data.wallJumpTime
        )
        {
            IsWallJumping   = false;
            // is no longer wall jumping
        }

        // if touching ground, AND not jumping and walljumping
        if
        (
            TimeLastOnGround > 0    &&
            !IsJumping              &&
            !IsWallJumping
        )
        {
            IsJumpCut       = false;
            IsJumpFalling   = false;
            // is not jump cutting and not falling
        }

        // if is not dashing
        if(!IsDashing)
        {
            // if can jump, AND jump key has pressed
            if
            (
                CanJump()               &&
                TimeLastPressedJump > 0
            )
            {
                IsJumping       = true;
                IsWallJumping   = false;
                IsJumpCut       = false;
                IsJumpFalling   = false;
                
                Set(jumpState);
                playerMovement.Jump();
                // is jumping, and not jump cutting, walljumping, or falling
            }

            // if can wall jump, AND jump key is pressed
            else if
            (
                CanWallJump()           &&
                TimeLastPressedJump > 0
            )
            {
                IsWallJumping   = true;
                IsJumping       = false;
                IsJumpCut       = false;
                IsJumpFalling   = false;

                TimeWallJumpStart   = Time.time;
                LastWallJumpDir     = (TimeLastOnRightWall > 0) ? -1 : 1;

                playerMovement.WallJump(LastWallJumpDir);
                // is wall jumping, and not jumping, jump cutting, or jump falling
            }
        }

        // if can dash, AND dash key is pressed
        if
        (
            CanDash()               &&
            TimeLastPressedDash > 0
        )
        {
            IsDashing       = true;
            IsJumping       = false;
            IsWallJumping   = false;
            IsJumpCut       = false;

            // is dashing, and not jumping, wall jumping, or jump cutting

            Set(dashState);
            Sleep(Data.dashSleepTime);

            if(MoveInput != Vector2.zero)
                LastDashDir = MoveInput;
            else
                LastDashDir = IsFacingRight ? Vector2.right : Vector2.left;

            StartCoroutine(playerMovement.StartDash(LastDashDir));
            // start dash method, check for direction
        }

        // if can slide, AND the player is pointing towards the wall they are touching
        if
        (
            CanSlide() &&
            (
                (TimeLastOnLeftWall     > 0 &&  MoveInput.x < 0) ||
                (TimeLastOnRightWall    > 0 &&  MoveInput.x > 0)
            )
        )
        {
            IsSliding       = true;
            IsJumpFalling   = false;

            Set(slideState);
            // is sliding, and not jump falling
        }
        // otherwise is not sliding
        else IsSliding = false;

        // if is not jumping
        if(!IsJumping)
        {
            // AND if is grounded and not watching
            if(collisionSensors.IsGrounded && !IsDashing)
            {
                Set(groundedState);
                // is grounded
            }
            // AND is not grounded and not sliding 
            else if(!collisionSensors.IsGrounded && !IsSliding)
            {
                Set(airState);
                // is in air
            }
        }
        
        // otherwise is in air
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
        TimeLastOnWall      -= Time.deltaTime;
        TimeLastOnRightWall -= Time.deltaTime;
        TimeLastOnLeftWall  -= Time.deltaTime;

        TimeLastPressedJump -= Time.deltaTime;
        TimeLastPressedDash -= Time.deltaTime;
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

        if(UserInput.instance.DashInput)        OnDashInput();
    }

    #endregion


    #region Gravity

    /// <summary>
    /// Calculates and sets gravity strength value based on player state.
    /// </summary>
    private void CalculateGravity()
    {
        if(IsSliding)
            SetGravityScale(0);

        // if currently falling downwards, AND the down button is pressed, change gravity to fast fall gravity
        else if(body.velocity.y < 0 && MoveInput.y < 0)
        {
            SetGravityScale(Data.gravityScale * Data.fallGravityMultiplierFast);
        }

        // if the jump is cut, change gravity to jump cut gravity
        else if(IsJumpCut)
        {
            SetGravityScale(Data.gravityScale * Data.jumpCutGravityMultiplier);
            // sets x veloctiy to input velocity so player is able to move in the air
            body.velocity = new Vector2
                (
                    body.velocity.x,
                    Mathf.Max(body.velocity.y, -Data.fallMaxSpeed)
                );
        }

        else if
        (
            (IsJumping || IsWallJumping || IsJumpFalling)               &&
            Mathf.Abs(body.velocity.y)  < Data.jumpHangTimeThreshold
        )
        {
            SetGravityScale(Data.gravityScale * Data.jumpHangGravityMultiplier);
        }

        // if the player is currently falling downwards, change gravity to fall gravity
        else if(body.velocity.y < 0)
        {
            SetGravityScale(Data.gravityScale * Data.fallGravityMultiplier);
            // sets x veloctiy to input velocity so player is able to move in the air
            body.velocity = new Vector2
                (
                    body.velocity.x,
                    Mathf.Max(body.velocity.y, -Data.fallMaxSpeed)
                );
        }

        // otherwise, change gravity to default gravity
        else
        {
            SetGravityScale(Data.gravityScale);
        }
    }

    /// <summary>
    /// Sets RigidBody2D gravity scale.
    /// </summary>
    /// <param name="gravityScale">Gravity scale to be set.</param>
    public void SetGravityScale(float gravityScale)
    => body.gravityScale = gravityScale;

    #endregion


    #region Collision Methods

    /// <summary>
    /// Resets the ground timer based on ground check collisions.
    /// </summary>
    private void CheckCollisions()
    {
        if(collisionSensors.IsGrounded)
            TimeLastOnGround    = Data.coyoteTime;

        if(collisionSensors.IsWallLeft)
            TimeLastOnLeftWall  = Data.coyoteTime;

        if(collisionSensors.IsWallRight)
            TimeLastOnRightWall = Data.coyoteTime;

        TimeLastOnWall = Mathf.Max(TimeLastOnLeftWall, TimeLastOnRightWall);
    }

    #endregion


    #region Input Callbacks

    /// <summary>
    /// Method called when jump button is pressed.
    /// </summary>
    public void OnJumpInput()
    {
        // resets the jump timer
        TimeLastPressedJump = Data.jumpInputBufferTime;
    }

    /// <summary>
    /// Method called when jump button is released.
    /// </summary>
    public void OnJumpUpInput()
    {
        // checks if the jump can be cut, and cuts it
        if(CanJumpCut() || CanWallJumpCut())
            IsJumpCut = true;
    }

    public void OnDashInput()
    {
        TimeLastPressedDash = Data.dashInputBufferTime;
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
    private bool CanJump()      
    =>  TimeLastOnGround    >   0   &&
        !IsJumping;                     // if the player has touched the ground AND is not currently jumping, return true
    
    /// <summary>
    /// Checks if the player is able to cut their jump.
    /// </summary>
    /// <returns>True if the player is able to jump cut.</returns>
    private bool CanJumpCut()   
    =>  IsJumping                   &&
        body.velocity.y     >   0;      // if the player is currently jumping, AND their upwards velocity is positive, return true

    /// <summary>
    /// Checks if the player is able to wall jump.
    /// </summary>
    /// <returns>True if player can wall jump.</returns>
    private bool CanWallJump() 
    =>  TimeLastPressedJump >   0   &&
        TimeLastOnWall      >   0   &&
        TimeLastOnGround    <=  0   &&
        (
            !IsWallJumping                                          ||
            (TimeLastOnRightWall    > 0 &&  LastWallJumpDir == 1)   ||
            (TimeLastOnLeftWall     > 0 &&  LastWallJumpDir == -1)
        );                              // if the player has pressed the jump key, AND is currently touching a wall AND not touching the ground, AND is not currently wall jumping, return true

    /// <summary>
    /// Checks if the player can cut their wall jump.
    /// </summary>
    /// <returns>True if the player can wall jump cut.</returns>
    private bool CanWallJumpCut()
    =>  IsWallJumping           &&
        body.velocity.y > 0;            // if the player is currentyl wall jumping AND their upwards velocity is positive, return true

    /// <summary>
    /// Checks if the player is able to wall slide.
    /// </summary>
    /// <returns>True if the player can wall slide.</returns>
    private bool CanSlide()
    {
        if
        (
            TimeLastOnWall      > 0     &&
            !IsJumping                  &&
            !IsWallJumping              &&
            !IsDashing                  &&
            TimeLastOnGround    <= 0
        )
            return true;                // if the player is currently touching a wall, AND is not jumping, walljumping or dashing, AND is not touching the ground, return true
        else
            return false;
    }

    /// <summary>
    /// Checks if the player is able to dash.
    /// </summary>
    /// <returns>True if the player has the required amount of dashes left.</returns>
    public bool CanDash()
    {
        if
        (
            !IsDashing                              &&
            DashesLeft          < Data.dashAmount   &&
            TimeLastOnGround    > 0                 &&
            !DashRefilling
        )
            StartCoroutine(playerMovement.DashRefill(1));
                // if the player is not dashing, AND has less dashes than the dash amount, AND is currently touching the ground, AND the dash is not currently refilling, start dash refill
        
        return DashesLeft > 0;
    }

    #endregion


    #region Helper Methods

    private void Sleep(float duration)
    {
        StartCoroutine(nameof(PerformSleep) ,duration);
    }

    private IEnumerator PerformSleep(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1;
    }

    #endregion
}