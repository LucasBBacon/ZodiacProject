using UnityEngine;

public class WallControlState : State
{
    [Header("Animation Clip")]
    public  AnimationClip   animClip;

    #region States

    [SerializeField] WallJumpState wallJumpState;
    [SerializeField] WallSlideState wallSlideState;
    [SerializeField] WallGrabState wallGrabState;
    [SerializeField] AirState airState;

    #endregion

    #region Blackboard Variables

    [HideInInspector] public bool IsWallJumping { get; private set; }
    [HideInInspector] public bool IsWallSliding { get; private set; }

    #endregion

    Player _player;

    #region Callback Functions

    private void Awake() {
        _player = GetComponentInParent<Player>();
    }

    public override void Enter()
    {
        base.Enter();
        
    }

    public override void Do()
    {
        base.Do();
        
        if (
            IsWallJumping
        )
        {
            airState.SetIsJumping(false);
            airState.SetIsJumpCut(false);
            airState.SetIsJumpFalling(false);

            Set(wallJumpState);
        }

        else if (
            wallSlideState.CanSlide() &&
            (
                (UserInput.instance.MoveInput.x > 0 && Data.TimeLastOnRightWall > 0) ||
                (UserInput.instance.MoveInput.x < 0 && Data.TimeLastOnLeftWall > 0)
            )
            )
        {
            IsWallSliding = true;
            Set(wallSlideState);
        }

        // else if (
        //     !IsWallJumping &&
        //     (Data.TimeLastOnWall <= 0 ||
        //     (UserInput.instance.MoveInput.x != _player.FacingDirection))
        //     )
        // {
        //     Set(airState);
        // }

        if (
            Data.TimeLastOnWall < 0
            )
        {
            IsComplete = true;
        }
    }

    public override void Exit()
    {
        base.Exit();

        IsWallJumping = false;
        IsWallSliding = false;
    }

    #endregion

    
    #region Checks

    public void SetWallJumping(bool value)
    => IsWallJumping = value;

    public void SetWallSliding(bool value)
    => IsWallSliding = value;

    #endregion
}