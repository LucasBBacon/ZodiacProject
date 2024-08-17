using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Movement : MonoBehaviour
{
    public Rigidbody2D Body { get; private set; }

    public bool CanSetVelocity { get; set; }
    public Vector2 CurrentVelocity { get; set; }

    public bool IsFacingRight { get; private set; }
    public int FacingDirection {
        get => IsFacingRight ? 1 : -1;
    }

    public bool IsOnPlatform;
    public Rigidbody2D PlatformBody;

    [HideInInspector] public UnityEvent TurnEvent;


    Vector2 _workspace;
    Vector2 _workspaceForce;


    #region Callback Methods

    private void Awake()
    {
        Body = GetComponentInParent<Rigidbody2D>();

        IsFacingRight = true;
        CanSetVelocity = true;
    }

    private void Update()
    {
        CurrentVelocity = Body.velocity;
    }

    #endregion


    #region Velocity Methods

    public void SetVelocityZero()
    {
        _workspace.Set(0f, 0f);

        SetFinalVelocity();
    }

    public void SetVelocity(
        float velocityX,
        float velocityY
    )
    {
        _workspace.Set(velocityX, velocityY);

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
        _workspace.Set
            (
                angle.x * velocity * direction,
                angle.y * velocity
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

    public void SetVelocityX(float velocity)
    {
        _workspace.Set(velocity, CurrentVelocity.y);

        SetFinalVelocity();
    }

    public void SetVelocityY(float velocity)
    {
        _workspace.Set(CurrentVelocity.x, velocity);

        SetFinalVelocity();
    }

    void SetFinalVelocity()
    {
        if (CanSetVelocity)
        {
            if (IsOnPlatform)
            {
                _workspace.Set(_workspace.x + PlatformBody.velocity.x, _workspace.y);
            }
            else
            {
                _workspace.Set(_workspace.x, _workspace.y);
            }

            CurrentVelocity = _workspace;
            Body.velocity = _workspace;
        }
    }

    #endregion

    
    #region Turn Methods

    public void TurnCheck(Vector2 moveInput)
    {
        if (IsFacingRight && moveInput.x < 0)
        {
            Turn(false);
        }

        else if (!IsFacingRight && moveInput.x > 0)
        {
            Turn(true);
        }
    }

    public void Turn()
    {

        IsFacingRight = !IsFacingRight;
        Body.transform.Rotate(0f, 180f, 0f);
        if (gameObject.GetComponentInParent<Player>())
            TurnEvent.Invoke();
    }

    public void Turn(bool turnRight)
    {

        if (turnRight)
        {
            IsFacingRight = true;
            Body.transform.Rotate(0f, 180f, 0f);
        }

        else
        {
            IsFacingRight = false;
            Body.transform.Rotate(0f, -180f, 0f);
        }
        if (gameObject.GetComponentInParent<Player>())
            TurnEvent.Invoke();
    }

    public Vector2 FindRelativePoint(Vector2 offset)
    {
        offset.x *= FacingDirection;

        return transform.position + (Vector3)offset;
    }


    #endregion
}