using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Movement : MonoBehaviour
{
    public Rigidbody2D Body { get; private set; }

    public float HorizontalVelocity { get; set; }
    public float VerticalVelocity { get; set;}

    public bool IsFacingRight { get; private set; }
    public int FacingDirection {
        get => IsFacingRight ? 1 : -1;
    }

    public bool IsOnPlatform;
    public Rigidbody2D PlatformBody;

    private CollisionSensors CollisionSensors;

    

    [HideInInspector] public UnityEvent TurnEvent;


    Vector2 _workspace;
    float TurnBufferTimer;


    #region Callback Methods

    private void Awake()
    {
        Body = GetComponent<Rigidbody2D>();
        CollisionSensors = GetComponentInChildren<CollisionSensors>();

        IsFacingRight = true;
    }

    private void FixedUpdate()
    {
        if (IsOnPlatform)
        {
            _workspace = new Vector2(Body.velocity.x + PlatformBody.velocity.x, Body.velocity.y);
        }
        else
        {
            _workspace = new Vector2(Body.velocity.x, Body.velocity.y);
        }
        SetFinalVelocity();
    }

    #endregion


    #region Movement

    // public void Move
    //     (
    //         float acceleration,
    //         float deceleration,
    //         Vector2 moveInput,
    //         float MoveSpeed
    //     )
    // {
    //     if (moveInput.x != 0f)
    //     {
    //         TurnCheck(moveInput);
    //         if (CollisionSensors.IsGrounded && !isOnSlope)
    //         {
    //             float targetVelocity = moveInput.x * MoveSpeed;
        
    //             HorizontalVelocity = Mathf.Lerp
    //                 (
    //                     HorizontalVelocity,
    //                     targetVelocity,
    //                     acceleration * Time.fixedDeltaTime
    //                 );
                
    //             float speedDiff = targetVelocity - Body.velocity.x;
    //             float movement = speedDiff * acceleration;
    //             ApplyMovementForce(movement);
    //         }
    //         else if (CollisionSensors.IsGrounded && isOnSlope && canWalkOnSlope)
    //         {
    //             float targetVelocity = moveInput.x * MoveSpeed;
        
    //             HorizontalVelocity = Mathf.Lerp
    //                 (
    //                     HorizontalVelocity,
    //                     targetVelocity * slopeNormalPerp.x,
    //                     acceleration * Time.fixedDeltaTime
    //                 );
                
    //             float speedDiff = targetVelocity - Body.velocity.x;
    //             float movement = speedDiff * acceleration;
    //             ApplyMovementForce(movement);
    //         }
    //     }

    //     else
    //     {
    //         HorizontalVelocity = Mathf.Lerp
    //             (
    //                 HorizontalVelocity,
    //                 0f,
    //                 deceleration * Time.deltaTime
    //             );
            
    //         float speedDiff = 0 - Body.velocity.x;
    //         float movement = speedDiff * deceleration;
    //         ApplyMovementForce(movement);
    //     }
    // }

    public void Move
        (
            Vector2 moveInput,
            float moveSpeed,
            bool isIdleOrRun = false
        )
    {
        TurnCheck(moveInput);

        if (
            CollisionSensors.IsGrounded
            && !CollisionSensors.IsOnSlope
            && isIdleOrRun
            )
        {
            _workspace.Set(moveInput.x * moveSpeed, Body.velocity.y);
            
            
            //ApplyForce(new Vector2(-moveInput.x * slopeNormalPerp.x * MoveSpeed, -moveInput.x * slopeNormalPerp.y * MoveSpeed));
        }
        else if (
            CollisionSensors.IsGrounded
            && CollisionSensors.IsOnSlope
            && CollisionSensors.CanWalkOnSlope
            && isIdleOrRun
            )
        {
            _workspace.Set
                (
                    -moveInput.x * CollisionSensors.SlopeNormalPerp.x * moveSpeed,
                    -moveInput.x * CollisionSensors.SlopeNormalPerp.y * moveSpeed
                );

            //ApplyForce(new Vector2(moveInput.x * MoveSpeed, Body.velocity.y));
        }
        else if (!CollisionSensors.IsGrounded)
        {
            _workspace.Set(moveSpeed * moveInput.x, Body.velocity.y);
        }

        SetFinalVelocity();
    }

    #endregion


    #region Velocity Methods

    public void ApplyVelocity()
    {
        // if (!player.DashState.IsDashing)
        SetVerticalVelocity(Mathf.Clamp(VerticalVelocity, -20f, 50f));
        // else
        //     SetVerticalVelocity(Mathf.Clamp(VerticalVelocity, -50f, 50f));

        _workspace.Set(HorizontalVelocity, VerticalVelocity);

        SetFinalVelocity();
    }

    public void SetVelocityZero()
    {
        VerticalVelocity = 0f;
        HorizontalVelocity = 0f;

        _workspace.Set
            (
                HorizontalVelocity,
                VerticalVelocity
            );
        
        SetFinalVelocity();
    }

    public void SetVerticalVelocity(float changeAmount)
    {
        VerticalVelocity = changeAmount;

        _workspace.Set
            (
                Body.velocity.x,
                VerticalVelocity
            );

        SetFinalVelocity();
    }


    public void IncrementVerticalVelocity(float incrementAmount)
    {
        VerticalVelocity += incrementAmount;

        _workspace.Set
            (
                Body.velocity.x,
                VerticalVelocity
            );

        SetFinalVelocity();
    }


    public void SetHorizontalVelocity(float changeAmount)
    {
        HorizontalVelocity = changeAmount;

        _workspace.Set
            (
                HorizontalVelocity,
                Body.velocity.y
            );

        SetFinalVelocity();
    }

    public void SetVelocity
        (
            float velocity,
            Vector2 angle,
            int direction
        )
    {
        angle.Normalize();

        HorizontalVelocity = angle.x * velocity * direction;
        VerticalVelocity = angle.y * velocity;
        
        _workspace.Set
            (
                HorizontalVelocity,
                VerticalVelocity
            );
        
        SetFinalVelocity();
    }

    public void SetVelocity
        (
            float velocity,
            Vector2 direction
        )
    {
        _workspace = direction * velocity;
        
        SetFinalVelocity();
    }

    

    void SetFinalVelocity()
    => Body.velocity = _workspace;

    #endregion


    #region Force Methods

    public void SetForce
    (
            float force,
            Vector2 angle,
            int direction
        )
    {
        angle.Normalize();
        
        Body.AddForce(new Vector2(angle.x * force * direction, angle.y * force), ForceMode2D.Force);
    }

    void ApplyMovementForce(float force)
    {
        Body.AddForce(force * Vector2.right, ForceMode2D.Force);
    }

    void ApplyForce(Vector2 force)
    {
        Body.AddForce(force, ForceMode2D.Force);
    }

    #endregion

    
    #region Turn Methods

    public void TurnCheck(Vector2 moveInput)
    {
        if (
            IsFacingRight &&
            moveInput.x < 0
            )
            Turn(false);

        else if (
            !IsFacingRight &&
            moveInput.x > 0
            )
            Turn(true);
    }

    public void TurnCheck(int moveInput)
    {
        if (
            moveInput != 0 &&
            moveInput != FacingDirection
        )
        Turn();
    }

    public void Turn(bool turnRight)
    {
        TurnEvent.Invoke();

        if (turnRight)
        {
            IsFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }
        else
        {
            IsFacingRight = false;
            transform.Rotate(0f, -180f, 0f);
        }
    }

    public void Turn()
    {
        TurnEvent.Invoke();

        IsFacingRight = !IsFacingRight;
        Body.transform.Rotate(0f, 180f, 0f);
    }

    public Vector2 FindRelativePoint(Vector2 offset)
    {
        offset.x *= FacingDirection;

        return transform.position + (Vector3)offset;
    }


    #endregion
}