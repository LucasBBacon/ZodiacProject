using System;
using System.Collections;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour 
{
    public PlayerData playerData;

    #region Components

    public Rigidbody2D      RB          { get; private set; }
    public PlayerAnimator   AnimHandler { get; private set; }

    #endregion

    // Variables for the various actions the player can perform at any time
    // these are public gett fields for other scripts to read them
    // but can only be privately written to
    #region State Parameters

    public bool     IsFacingRight       { get; private set; }
    public bool     IsJumping           { get; private set; }
    public bool     IsWallJumping       { get; private set; }
    public bool     IsDashing           { get; private set; }
    public bool     IsSliding           { get; private set; }
    //public bool     IsInteracting       { get; private set; }

    // timers for variable use
    public float    LastOnGroundTime    { get; private set; }
    public float    LastOnWallTime      { get; private set; }
    public float    LastOnWallRightTime { get; private set; }
    public float    LastOnWallLeftTime  { get; private set; }

    
    // Jump
    private bool    _isJumpCut;
    private bool    _isJumpFalling;

    // Wall Jump
    private float   _wallJumpStartTime;
    private int     _lastWallJumpDir;

    // Dash
    private int     _dashesLeft;
    private bool    _dashRefiling;
    private Vector2 _lastDashDir;
    private bool    _isDashAttacking;

    #endregion


    #region Input Parameters

    private Vector2 _moveInput;
    public float    LastPressedJumpTime { get; private set; }
    public float    LastPressedDashTime { get; private set; }

    #endregion


    #region Check Parameters

    [Header("Checks")]
    [SerializeField] private Transform  _groundCheckPoint;
    // size of groundCHeck depends on the size of the character, generally they should be slightly smaller than the width (for ground) and height (for wall)
    [SerializeField] private Vector2    _groundCheckSize    = new Vector2(0.49f, 0.03f);
    [Space(5)]
    [SerializeField] private Transform  _frontWallCheckPoint;
    [SerializeField] private Transform  _backWallCheckPoint;
    [SerializeField] private Vector2    _wallCheckSize      = new Vector2(0.5f, 1f);

    #endregion


    #region Layers and Tags

    [Header("Layers and Tags")]
    [SerializeField] private LayerMask  _groundLayer;

    [Header("Camera")]
    [SerializeField] private GameObject _cameraGameObj;
    private CameraFollowObjects         _cameraFollowObj;

    #endregion

    private void Awake() 
    {
        RB                  = GetComponent<Rigidbody2D>();
        AnimHandler         = GetComponent<PlayerAnimator>();

        _cameraFollowObj    = _cameraGameObj.GetComponent<CameraFollowObjects>(); 
    }

    private void Start() 
    {
        SetGravityScale(playerData.gravityScale);
        IsFacingRight = true;
    }

    private void Update()
    {

        #region Timers
        // starts decreasing timers

        LastOnGroundTime    -= Time.deltaTime;
        LastOnWallTime      -= Time.deltaTime;
        LastOnWallRightTime -= Time.deltaTime;
        LastOnWallLeftTime  -= Time.deltaTime;

        LastPressedJumpTime -= Time.deltaTime;
        LastPressedDashTime -= Time.deltaTime;

        #endregion

        #region Input Handler
        // handles any player input (will change)

        _moveInput = UserInput.instance.MoveInput;

        if(_moveInput.x != 0)                   CheckDirectionToFace(_moveInput.x > 0);
        if(UserInput.instance.JumpJustPressed)  OnJumpInput();
        if(UserInput.instance.JumpReleased)     OnJumpUpInput();
        if(UserInput.instance.DashInput)        OnDashInput();

        #endregion

        #region Collision Checks
        // checks if the different colliders are currently touching their respective layers

        if(!IsJumping)
        {
            // ground check
            if(Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer))
            {
                if(LastOnGroundTime < -0.1f)    AnimHandler.JustLanded = true;

                LastOnGroundTime = playerData.coyoteTime;
            }

            // right wall check
            if(((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight) || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)) && !IsWallJumping) 
                LastOnWallRightTime = playerData.coyoteTime;
        
            // left wall check
            if(((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight) || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)) && !IsWallJumping) 
                LastOnWallLeftTime = playerData.coyoteTime;

            // two checks neede dfor both left and righ walls since whenever the player turns the wall checkpoints swap sides
            LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
        }

        #endregion

        // if(UserInput.PlayerInput.actions["Rumble"].WasPressedThisFrame()) 
        // {   Debug.Log("Rumble Pressed");
        //     RumbleManager.instance.RumblePulse(0.3f, 0.8f, 1f); 
        // }

        #region Jump Checks
        // checks the different jump states the player can do

        // if player is currently jumping AND their vertical velocity is less than 0
        if(IsJumping && RB.velocity.y < 0)
        {
            // player is no longer jumping
            IsJumping = false;

            // if the player is not wall jumping either, the player is currenlty falling
            _isJumpFalling = true;
        }

        // if the player is wall jumping, AND the time since the wall jump started is greater than the time alloted for wall jumps
        if(IsWallJumping && Time.time - _wallJumpStartTime > playerData.wallJumpTime)
        {
            // player is no longer wall jumping
            IsWallJumping = false;
        }

        // if the player is touching the ground, AND is not jumping, AND is not wall jumping
        if(LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
        {
            // player is not cutting the jump short
            _isJumpCut = false;
            
            // if the player is still not jumping, then the player is no logner falling
            _isJumpFalling = false;

            //RumbleManager.instance.RumblePulse(0.3f, 0.8f, 0.1f);
        }

        // Jump
        // if the player is not dashing
        if(!IsDashing)
        {
            // if the player can jump AND has just pressed the jump key
            if(CanJump() && LastPressedJumpTime > 0)
            {
                IsJumping       = true;     // the player is jumping
                IsWallJumping   = false;    // the player is not wall jumping
                _isJumpCut      = false;    // the player is not cutting the jump short
                _isJumpFalling  = false;    // the player is not falling after jumping
                
                // call the jump method
                Jump();

                AnimHandler.StartedJumping = true;
            }

            // Wall jump
            // else if the player can wall jump AND has just pressed the jump key
            else if(CanWallJump() && LastPressedJumpTime > 0)
            {
                IsWallJumping   = true;     // the player is wall jumping
                IsJumping       = false;    // the player is not jumping
                _isJumpCut      = false;    // the player is not cutting the jump short
                _isJumpFalling  = false;    // the player is not falling after jumping

                // set the start of the wall jump to the current time
                _wallJumpStartTime  = Time.time;
                // check to see if the right wall is being touched, and determine which wall is currently being touched, pass that into a variable
                _lastWallJumpDir    = (LastOnWallRightTime > 0) ? -1 : 1;

                // call the wall jump method with the wall jump direction
                WallJump(_lastWallJumpDir);
            }
        }

        #endregion

        #region Dash Checks
        // checks the different dash states the player can do

        // if the player can dash AND has just pressed the dash key
        if(CanDash() && LastPressedDashTime > 0)
        {
            // freeze game for a split second, adds a bit of forgiveness over directional input
            Sleep(playerData.dashSleepTime);

            // if no direction is pressed, dash in the direction player was facing
            if(_moveInput != Vector2.zero)  _lastDashDir = _moveInput;
            else                            _lastDashDir = IsFacingRight ? Vector2.right : Vector2.left;

            IsDashing       = true;     // the player is dashing
            IsJumping       = false;    // the player is not jumping
            IsWallJumping   = false;    // the player is not wall jumping
            _isJumpCut      = false;    // the player is not cutting a jump short

            // call dash coroutine and pass the direction of the dash
            StartCoroutine(nameof(StartDash), _lastDashDir);
        }

        #endregion

        #region Slide Checks
        // checks the different states the player can slide in

        // if the player can slide, AND either is touching a wall on their left AND trying to go left, OR touching a wall on the right AND trying to go right
        if(CanSlide() && ((LastOnWallLeftTime > 0 && _moveInput.x < 0) || (LastOnWallRightTime > 0 && _moveInput.x > 0)))
        {
            IsSliding = true;   // the player is sliding
            _isJumpFalling = false;
            RB.velocity = new Vector2(RB.velocity.x, 0);
        }
        else
            IsSliding = false;  // the player is not sliding

        #endregion

        #region Gravity

        if(!_isDashAttacking)
        {
            if(IsSliding) SetGravityScale(0);

            else if(RB.velocity.y < 0 && _moveInput.y < 0)
            {
                // higher gravity if holding Down button
                SetGravityScale(playerData.gravityScale * playerData.fastFallGravityMult);
            }
            
            else if(_isJumpCut)
            {
                // higher gravity if jump button is released
                SetGravityScale(playerData.gravityScale * playerData.jumpCutGravityMult);
                RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -playerData.maxFallSpeed));
            }

            else if((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < playerData.jumpHangTimeThreshold)
            {
                SetGravityScale(playerData.gravityScale * playerData.jumpHangGravityMult);
            }

            else if(RB.velocity.y < 0)
            {
                // higher gravity if falling
                SetGravityScale(playerData.gravityScale * playerData.fallGravityMult);
                // caps maximum fall speed, so when falling over large distances not accelerated to insanely high speeds
                RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -playerData.maxFallSpeed));
            }

            // Defaut gravity if standing on a platform or moving upwards
            else SetGravityScale(playerData.gravityScale);
        }
        // highger gravity if the jump input is release or is falling


        #endregion

    }

    private void FixedUpdate() 
    {
        // Run handler
        if(!IsDashing)
        {
            if(IsWallJumping)       Run(playerData.wallJumpRunLerp);
            else                    Run(1);
        }
        else if(_isDashAttacking)   Run(playerData.dashEndRunLerp);

        // Slide handler
        if(IsSliding)               Slide();
    }

    #region Input Callbacks
    // Methods which handle input detected in Update()

    /// <summary>
    /// Resets the player jump buffer.
    /// </summary>
    public void OnJumpInput()
    {
        LastPressedJumpTime = playerData.jumpInputBufferTime;
    }

    /// <summary>
    /// Sets the jump cut variable to true, allowing the player to stop the jump on button release.
    /// </summary>
    public void OnJumpUpInput()
    {
        if(CanJumpCut() || CanWallJumpCut()) _isJumpCut = true;
    }

    /// <summary>
    /// Resets the player dash buffer.
    /// </summary>
    public void OnDashInput()
    {
        LastPressedDashTime = playerData.dashInputBufferTime;
    }

    #endregion

    #region General Methods
    // Methods that do not fall in specific regions

    /// <summary>
    /// Sets the gravity scale in the player's <c>RigidBody2D</c> component.
    /// </summary>
    /// <param name="scale">The <c>Gravity Scale</c> to be set.</param>
    public void SetGravityScale(float scale)
    {
        RB.gravityScale = scale;
    }

    /// <summary>
    /// Method used so <c>StartCoroutine(PerformSleep)</c> does not needs to be called everywhere.
    /// </summary>
    /// <param name="duration">Duration of sleep.</param>
    private void Sleep(float duration)
    {
        // method used so coroutines do not need to be called everywhere
        // nameof() notation means a string does not need to be input direcly
        // removes chance of spelling mistakes and will improve any error messages present
        StartCoroutine(nameof(PerformSleep), duration);
    }

    /// <summary>
    /// Freezes the game for a set duration of time.
    /// </summary>
    /// <param name="duration">Duration for which the game freezes for.</param>
    /// <returns>Time scales the game to 0 for <c>duration</c> amount of real time.</returns>
    private IEnumerator PerformSleep(float duration)
    {
        Time.timeScale = 0;
        // must be realtime since timescale is set to 0
        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1;
    }

    #endregion

    #region Movement Methods
    // methods that handle player movements

    /// <summary>
    /// Method that applies a force onto the player's <c>RigidBody2D</c> component in the x-axis.
    /// </summary>
    /// <param name="lerpAmount">Smoothing amount of changes in direction and speed.</param>
    private void Run(float lerpAmount)
    {
        // calculate the direction to move in and the desired velocity
        float targetSpeed = _moveInput.x * playerData.runMaxSpeed;
        
        // control can be reduced using Lerp(), this smooths changes to direction and speed
        targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);


        #region Calculate AccelRate

        float accelRate;

        // gets acceleration value based on if the player is accelerating (including turning) or decellerating (stopping), as well as applying a multiplier if airborn
        if(LastOnGroundTime > 0)    accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? playerData.runAccelAmount : playerData.runDeccelAmount;

        else                        accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? playerData.runAccelAmount * playerData.accelInAir : playerData.runDeccelAmount * playerData.deccelInAir;

        #endregion


        #region Conserve Momentum

        // the player is not slowed down if htey are moving in their desired direction but at a greater speed than their max speed
        if(playerData.doConserveMomentum && Mathf.Abs(RB.velocity.x) > Math.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            // prevent any decceleration from happeing - conserve the current moment
            accelRate = 0;
        }

        #endregion


        // calculate the difference between the current velocity and desired velocity
        float speedDiff = targetSpeed - RB.velocity.x;
        // calculate the force along x-axis to apply to the player
        float movement = speedDiff * accelRate;
        // convert to a vector and apply to rigidbody
        RB.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    /// <summary>
    /// Switches the player's scale to flip their orientation.
    /// </summary>
    private void Turn()
    {
        // store scale and flips the player along the x axis
        Vector3 scale           =   transform.localScale;
        scale.x                 *=  -1;
        transform.localScale    =   scale;

        IsFacingRight           =   !IsFacingRight;
        _cameraFollowObj.CallTurn();
    }

    #endregion

    #region Jump Methods
    // methods that handle player vertical movement

    /// <summary>
    /// Method that applies a force in the player's <c>RigidBody2D</c> component in the y-axis.
    /// </summary>
    private void Jump()
    {
        // ensures jump can't be called multiple times from one press
        LastPressedJumpTime = 0;
        LastOnGroundTime    = 0;

        #region Perform Jump

        // the force applied is increased if falling
        // this means the player will feel like they jump the same amount
        // setting the player's Y velocity to 0 beforehand will likely work the same, but this is nicer

        float force = playerData.jumpForce;
        if(RB.velocity.y < 0) force -= RB.velocity.y;

        // Impulse force is added to the Rigidbody component
        RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        #endregion
    }

    /// <summary>
    /// Method that applies a force in the player's <c>RigidBody2D</c> component in the x and y-axis, if the player is in contact with a wall.
    /// </summary>
    /// <param name="dir">Direction which player jumps from the wall.</param>
    private void WallJump(int dir)
    {
        // ensures wall jump can't be called multiple times from one press
        LastPressedJumpTime = 0;
        LastOnGroundTime    = 0;
        LastOnWallRightTime = 0;
        LastOnWallLeftTime  = 0;

        #region Perform Wall Jump

        Vector2 force = new Vector2(playerData.wallJumpForce.x, playerData.wallJumpForce.y);
        // applies force in opposite direction of wall
        force.x *= dir;

        if(Mathf.Sign(RB.velocity.x) != Mathf.Sign(force.x))    force.x -= RB.velocity.x;

        // checks wheter player is falling, if so the velocity.y (force counteracting gravity force) is subtracted. This ensures player always reaches the desired jump force or greater
        if(RB.velocity.y < 0)                                   force.y -= RB.velocity.y;

        RumbleManager.instance.RumblePulse(0.4f, 0.6f, 0.3f);

        // unlike in the run, the impulse mode should be used here
        // the default mode will apply the force instantly ignoring the mass
        RB.AddForce(force, ForceMode2D.Impulse);

        #endregion
    }

    #endregion

    #region Dash Movement Method
    // methods that handle player dash

    /// <summary>
    /// Applies a variable velocity to the player's <c>RigidBody2D</c> component in a single direction.
    /// </summary>
    /// <param name="dir">Direction in which velocity is applied.</param>
    /// <returns>A period of time for which gravity does not affect the player <c>RigidBody2D</c> component.</returns>
    private IEnumerator StartDash(Vector2 dir)
    {
        // this method mimics Celeste dash
        // for a more physics based approach, try a method similar to jump

        LastOnGroundTime    = 0;
        LastPressedDashTime = 0;

        float startTime     = Time.time;

        _dashesLeft--;
        _isDashAttacking    = true;

        SetGravityScale(0);

        // player's velocity is kept at the dash speed during the "attack" phase (in celeste the first 0.15s)
        while(Time.time - startTime <= playerData.dashAttackTime)
        {
            RB.velocity = dir.normalized * playerData.dashSpeed;
            // pauses loop until the next frame, creatime a form of update loop
            // cleaner implementation (does not use multiple timers) this is also what is used in celeste
            yield return null;
        }

        startTime           = Time.time;

        _isDashAttacking    = false;

        //begins the end of the dash where some control is returned to the player but run acceleration is still limited (see update and run)
        SetGravityScale(playerData.gravityScale);
        RB.velocity = playerData.dashEndSpeed * dir.normalized;

        while(Time.time - startTime <= playerData.dashEndTime)  yield return null;
        
        // dash over
        IsDashing           = false;
    }

    /// <summary>
    /// Sets the cooldown for <seealso cref="PlayerMovementForce.StartDash()"/>.
    /// </summary>
    /// <param name="amount">The time for which the player must wait to dash again.</param>
    /// <returns>Increases the number of dashes after a certain time <c>amount</c> has passed.</returns>
    private IEnumerator RefillDash(int amount)
    {
        // cooldown, so player cannot constantly dash along the ground
        _dashRefiling = true;

        yield return new WaitForSeconds(playerData.dashRefillTime);

        _dashRefiling = false;
        
        _dashesLeft = Mathf.Min(playerData.dashAmount, _dashesLeft + 1);
    }

    #endregion

    #region Slide Movement Method
    // method that handles player movement in the y-axis in contact with walls

    /// <summary>
    /// Counteracts gravity on the player's <c>RigidBody2D</c> component while input is held.
    /// </summary>
    private void Slide()
    {
        // works the same as the run, but only in the y-axis
        float speedDiff = playerData.slideSpeed - RB.velocity.y;
        float movement = speedDiff * playerData.slideAccel;

        // the movement is clamped here to prevent any over corrections (not that noticable in the run, so not implemented there)
        // the force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDiff) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDiff) * (1 / Time.fixedDeltaTime));

        RB.AddForce(movement * Vector2.up);
    }

    #endregion


    #region Check Methods
    // methods that check if a certain action can be done

    /// <summary>
    /// Checks the direction player should be facing.
    /// </summary>
    /// <param name="isMovingRight">True if the player is moving right.</param>
    public void CheckDirectionToFace(bool isMovingRight)
    {
        if(isMovingRight != IsFacingRight) Turn();
    }

    /// <summary>
    /// Checks if player is able to jump.
    /// </summary>
    /// <returns><c>True</c> if the player is in contact with the ground and is not in the process of jumping. <c>False</c> otherwise.</returns>
    private bool CanJump()
    {
        return LastOnGroundTime > 0 && !IsJumping;
    }

    /// <summary>
    /// Checks if player is able to walljump.
    /// </summary>
    /// <returns><c>True</c> if the player has pressed jump, AND is in contact with a wall, AND is not in contact with the ground, AND is either not wall jumping, OR trying to jump right AND touching a wall on their right, OR trying to jump left AND touching a wall on their left. <c>False</c> otherwise.</returns>
    private bool CanWallJump()
    {
        return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!IsWallJumping || (LastOnWallRightTime > 0 && _lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && _lastWallJumpDir == -1));
    }

    /// <summary>
    /// Checks if the player can stop applying vertical jump force.
    /// </summary>
    /// <returns><c>True</c> if player is currently jumping AND their y-velocity is greater than 0. <c>False</c> otherwise.</returns>
    private bool CanJumpCut()
    {
        return IsJumping && RB.velocity.y > 0;
    }

    /// <summary>
    /// Checks if player can stop applying diagonal jump force.
    /// </summary>
    /// <returns><c>True</c> if player is currently wall jumping AND their y velocity is greater than 0. <c>False</c> otherwise.</returns>
    private bool CanWallJumpCut()
    {
        return IsWallJumping && RB.velocity.y > 0;
    }

    /// <summary>
    /// Checks if player can dash. Calls Coroutine to refill dash <seealso cref="PlayerMovementForce.StartDash()"/>, if empty. 
    /// </summary>
    /// <returns><c>True</c> if player has more than 0 dashes left.</returns>
    private bool CanDash()
    {
        if(!IsDashing && _dashesLeft < playerData.dashAmount && LastOnGroundTime > 0 && !_dashRefiling)
        {
            StartCoroutine(nameof(RefillDash), 1);
        }
        
        return _dashesLeft > 0;
    }

    /// <summary>
    /// Checks if the player can slide.
    /// </summary>
    /// <returns><c>True</c> if the player is currently touching a wall, AND is not jumping, AND is not wall jumping AND is not touching the ground. <c>False</c> otherwise.</returns>
    public bool CanSlide()
    {
		if (LastOnWallTime > 0 && !IsJumping && !IsWallJumping && !IsDashing && LastOnGroundTime <= 0)
			return true;
		else
			return false;
	}

    #endregion

    
    #region Editor Methods
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
        Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
    }
    #endregion
}