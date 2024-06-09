using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class WallJumpState : State
{
    [Header("Animation Clip")]
    public  AnimationClip   animClip;

    #region States

    [Header("States")]
    [SerializeField] WallControlState wallControlState;
    [SerializeField] RunState runState;
    [SerializeField] AirState airState;

    #endregion

    Player _player;


    #region Callback Functions

    private void Awake() {
        _player = GetComponentInParent<Player>();
    }

    public override void Enter()
    {
        Animator.Play(animClip.name);

        // Data.LastWallJumpDir = (Data.TimeLastOnRightWall > 0) ? -1 : 1;
        WallJump(WallJumpDirection(Data.TimeLastOnRightWall > 0));
    }

    public override void Do()
    {
        // runState.Run(Data.wallJumpRunLerp);

        if (
            Time.time >= startTime + Data.wallJumpTime ||
            Data.TimeLastOnGround > 0
            )
        {
            
            wallControlState.SetWallJumping(false);
            IsComplete = true;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    #endregion


    #region Checks

    public bool CanWallJump()
    =>  Data.TimeLastOnGround <= 0 &&
        Data.TimeLastOnWall > 0 &&
        Data.TimeLastPressedJump > 0 &&
        (
            !wallControlState.IsWallJumping || 
            (Data.TimeLastOnRightWall > 0 && Data.LastWallJumpDir == 1) ||
            (Data.TimeLastOnLeftWall > 0 && Data.LastWallJumpDir == -1)
        );

    public bool CanWallJumpCut()
    => wallControlState.IsWallJumping && Body.velocity.y > Mathf.Epsilon;

    #endregion


    #region Functionality

    private void WallJump(int dir)
    {
        Vector2 force = new Vector2
            (
                Data.wallJumpForce.x * dir,
                Data.wallJumpForce.y
            );
        
        // if (Mathf.Sign(Body.velocity.x) != Mathf.Sign(force.x))
        //     force.x -= Body.velocity.x;

        // if (Body.velocity.y < 0)
        //     force.y -= Body.velocity.y;

        Debug.Log(force);

        Body.AddForce(force, ForceMode2D.Impulse);
    }

    public int WallJumpDirection(bool isTouchingRightWall)
    {
        return isTouchingRightWall ? -1 : 1;
    }

    #endregion
}