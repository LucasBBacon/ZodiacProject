using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : State
{
    [Header("Animation Clip")]
    public AnimationClip animClip;


    #region States

    [Header("States")]
    [SerializeField] AirState airState;

    #endregion

    [Space(20)]

    [Header("Effects")]
    [SerializeField] GameObject _jumpEffect;

    #region Callback Functions

    public override void Enter()
    {
        base.Enter();

        Animator.Play(animClip.name);

        Jump();
        JumpParticles();
    }

    public override void Do()
    {
        base.Do();

        Set(airState);
        
        IsComplete = true;
        return;
    }

    public override void FixedDo()
    {
        base.FixedDo();
    }

    #endregion

    // public void ResetAmountOfJumpsLeft()    => amountOfJumpsLeft = playerData.jumpAmount;
    // public void DecreaseAmountOfJumpsLeft() => amountOfJumpsLeft--;

    #region Checks

    /// <summary>
    /// Check if entity is able to jump.
    /// </summary>
    /// <returns>True if entity is able to jump.</returns>
    public bool CanJump()
    => !airState.IsJumping && Data.TimeLastOnGround > 0; // if touching ground AND not currently jumping, return true

    public bool CanJumpCut()
    => airState.IsJumping && Body.velocity.y > 0;

    #endregion


    #region Functionality

    /// <summary>
    /// Adds a vertical impulse force upwards to the entitiy's <c>RigidBody2D</c>
    /// </summary>
    public void Jump()
    {
        float force = Data.jumpForce;

        if(Body.velocity.y < 0)
            force -= Body.velocity.y;

        Body.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    #endregion


    #region Effects

    private void JumpParticles()
    {
        GameObject obj = Instantiate(
            _jumpEffect,
            transform.position - (Vector3.up * 0.8f),
            Quaternion.Euler(-90, 0, 0)
            );
        Destroy(obj, 1);  
    }

    #endregion
}