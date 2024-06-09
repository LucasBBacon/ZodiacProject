using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Player : Core 
{
    [HideInInspector]
    public  PlayerMovement  playerMovement;

    public CameraFollowObject followOBJ;

    [Space(20)]

    #region State Definitions

    [Header("States")]
    [SerializeField] GroundedState groundedState;
    [SerializeField] AirState airState;
    [SerializeField] WallControlState wallControlState;
    [SerializeField] AbilityState abilityState;
    [SerializeField] JumpState jumpState;
    [SerializeField] RunState runState; 
    [SerializeField] WallJumpState wallJumpState;
    [SerializeField] DashState dashState;

    #endregion

    [Space(20)]

    #region State Parameters

    [Header("State Parameters")]

    public  bool            IsFacingRight;

    public  bool            IsSliding           { get; private set; }


    [Space(10)]

    [Space(10)]

    public  bool            IsLedgeGrabbing;
    public  bool            IsLedgeClimbing;
    private bool            IsLedgeFalling;
    [SerializeField] 
    private Vector2         climbingOffset = new Vector2(1f, 1.6f);
    public  bool            moved;
    private Vector2         cornerPos;
    private float           animationTime = .5f;

    #endregion

    [Space(20)]

    #region Input Parameters

    [Header("Input Parameters")]

    public  Vector2         MoveInput;

    #endregion

    public int FacingDirection;
    public Vector2 holdPosition;

    // private RaycastHit2D[] hits;
    // [SerializeField] private Transform attackTransform;
    // [SerializeField] private float attackRange = 1.5f;
    // [SerializeField] private LayerMask attackableLayer;
    // [SerializeField] private int damageAmount;


    #region Unity Callback Methods


    private void Start()
    {
        IsFacingRight = true;
        body.gravityScale = data.gravityScale;
        SetupInstances();
        Set(airState);
        // data.TimeLastPressedDash = -1f;
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        FacingDirection = IsFacingRight ? 1 : -1;
        dashState.LastDashDir = IsFacingRight ? Vector2.right : Vector2.left;
        data.UpdateTimers();
        CheckInput();
        SelectState();
    }

    private void FixedUpdate()
    {
        CheckCollisions();
        gravity.CalculateGravity();

        if (
            !dashState.IsDashing
            )
        {
            Set(runState);
        }

        state.FixedDoBranch();
    }

    #endregion
    

    #region State Selection

    /// <summary>
    /// Select the correct state
    /// </summary>
    private void SelectState()
    {
        if (collisionSensors.IsGrounded)
        {
            if (
                UserInput.instance.MoveInput == Vector2.zero
                )
                Set(groundedState);
        }

        if (
            !dashState.IsDashing
            )
        {
            if (
                jumpState.CanJump() &&
                data.TimeLastPressedJump > 0 
                )
            {
                airState.SetIsJumping(true);
                Set(jumpState);
            }

            else if (
                wallJumpState.CanWallJump() &&
                data.TimeLastPressedJump > 0
                )
            {
                wallControlState.SetWallJumping(true);
                Set(wallControlState);
            }
        }

        
        if (
            !collisionSensors.IsGrounded
            )
        {
            Set(airState);
        }


        if (
            !collisionSensors.IsGrounded &&
            body.velocity.y >= 0 &&
            data.TimeLastOnWall > 0
            )
        {
            Set(wallControlState);
        }

        

        if (
            dashState.CanDash() &&
            data.TimeLastPressedDash > 0
            )
        {
            Set(abilityState);
        }

        state.DoBranch();
    }

    #endregion


    #region Input Handler

    /// <summary>
    /// Fetches the inputs from <c>UserInput</c>.
    /// </summary>
    private void CheckInput()
    {
        MoveInput = UserInput.instance.MoveInput;
        if(!IsLedgeGrabbing && !IsLedgeClimbing)
            if(MoveInput.x != 0)                CheckDirectionToFace(MoveInput.x > 0);

        if(UserInput.instance.JumpJustPressed)  OnJumpInput();
        if(UserInput.instance.JumpReleased)     OnJumpUpInput();

        if(UserInput.instance.DashInput)        OnDashInput();

        if(UserInput.instance.GrabInput)        OnGrabInput();
        if(UserInput.instance.GrabBeingHeld)    OnGrabHeldInput();

        //if(UserInput.instance.AttackInput)      OnAttackInput();
    }

    #endregion


    #region Collision Methods

    /// <summary>
    /// Resets the ground timer based on ground check collisions.
    /// </summary>
    private void CheckCollisions()
    {
        if(collisionSensors.IsGrounded)
            data.ResetGroundTime();

        if(collisionSensors.IsWallRight)
            data.ResetWallRightTime();

        if(collisionSensors.IsWallLeft)
            data.ResetWallLeftTime();
    }

    #endregion


    #region Input Callbacks

    /// <summary>
    /// Method called when jump button is pressed.
    /// </summary>
    public void OnJumpInput()
    {
        // resets the jump timer
        data.ResetJumpTime();
    }

    /// <summary>
    /// Method called when jump button is released.
    /// </summary>
    public void OnJumpUpInput()
    {
        // checks if the jump can be cut, and cuts it
        if(jumpState.CanJumpCut() || wallJumpState.CanWallJumpCut())
            airState.SetIsJumpCut(true);
    }

    public void OnDashInput()
    {
        data.ResetDashTime();
    }

    public void OnGrabInput()
    {
        CalculateHoldPos();
    }

    public void OnGrabHeldInput()
    {
        data.ResetGrabTime();
    }

    // public void OnAttackInput()
    // {
    //     Debug.Log("Attacking");
    //     Attack();
    // }

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

    private bool CanGrab()
    {
        if
        (
            data.TimeLastOnWall > 0 &&
            !airState.IsJumping &&
            !wallControlState.IsWallJumping &&
            data.TimeLastOnGround <= 0
        )
            return true;
        else
            return false;
    }

    #endregion

    // private void Attack()
    // {
    //     hits = Physics2D.CircleCastAll(attackTransform.position, attackRange, transform.right, 0f, attackableLayer);

    //     for(int i = 0; i < hits.Length; i++)
    //     {
    //         IDamageable iDamageable = hits[i].collider.gameObject.GetComponent<IDamageable>();

    //         if(iDamageable != null)
    //         {
    //             iDamageable.Damage(damageAmount, Vector2.right);
    //         }
    //     }
    // }


    #region Helper Methods

    private void CalculateHoldPos()
    {
        holdPosition = collisionSensors.FindWallPos(FacingDirection) - (Vector2.right * 0.5f * FacingDirection);
    }

    private void AdjustPlayerPosition()
    {
        float xdist = Physics2D.Raycast
            (
                new Vector2
                    (
                        transform.position.x, 
                        transform.position.y + collisionSensors.middleCheckOffset.y - (collisionSensors.middleCheckSize.y/2)
                    ), Vector2.right * transform.localScale.x,
                2f,
                collisionSensors.groundMask
            ).point.x;
        
        float ydist = Physics2D.Raycast
            (
                new Vector2
                    (
                        xdist + (0.1f * transform.localScale.x),
                        transform.position.y + collisionSensors.topCheckOffset.y
                    ), Vector2.down,
                2f,
                collisionSensors.groundMask
            ).point.y;
        
        cornerPos = new Vector2
            (
                xdist,
                ydist
            );

        if(!moved)
        {
            moved = true;

            transform.position = new Vector2
                    (
                        cornerPos.x - (transform.localScale.x * 0.5f),
                        cornerPos.y - 0.8f
                    ); 
        }
    }

    private void NotFalling()
    {
        IsLedgeFalling = false;
    }

    private void NotSliding()
    {
        IsSliding = false;
    }

    #endregion

    #region Debug

    // private void OnDrawGizmos()
    // {
    //     Gizmos.DrawWireSphere(attackTransform.position, attackRange);
    //     Gizmos.color = Color.blue;
    //     Gizmos.DrawLine((Vector2)transform.position + new Vector2(0, collisionSensors.middleCheckOffset.y * 0.5f), (Vector2)transform.position + new Vector2(transform.localScale.x * 2f, collisionSensors.middleCheckOffset.y/2));
    // }

    #endregion
}