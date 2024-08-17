using UnityEngine;

public class PlayerJumpState : PlayerState
{
    
    #region Blackboard Variables

    public int AmountOfJumpsLeft { get; set; }
    public bool IsJumpCut { get; set; }
    public float LastPressedJumpTime { get; set; }
    public float JumpBufferTimer { get; set; }
    public float CoyoteTimer { get; set; }

    #endregion

    bool _isGrounded;
    bool _isWall;


    public PlayerJumpState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        AmountOfJumpsLeft = MoveData.NumberOfJumpsAllowed;
    }


    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        player.SpawnParticles(player.JumpDustParticles);
        
        InitiateJump();
        ChangeState(player.AirborneState);
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSensors)
        {
            _isGrounded = CollisionSensors.IsGrounded;
            _isWall = CollisionSensors.IsWall;
        }
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (
            InputManager.instance.AbilityUse[(int)AbilityInputs.First] &&
            (player.AbilityOneState.CanCheck() || player.AbilityOneState.CanAirCheck())
            )
        {
            ChangeState(player.AbilityOneState);
        }
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();

        player.Move(MoveData.AirAcceleration, MoveData.AirDeceleration, InputManager.instance.MoveInput);
    }

    #endregion


    #region Timers

    public void JumpTimers()
    {
        JumpBufferTimer -= Time.deltaTime;

        if (!CollisionSensors.IsGrounded)
            CoyoteTimer -= Time.deltaTime;

        else
            CoyoteTimer = MoveData.JumpCoyoteTime;
    }

    #endregion


    #region Checks

    public bool CanJump()
    {
        if (
            JumpBufferTimer > 0f
            && !player.AirborneState.IsJumping
            && (CollisionSensors.IsGrounded || CoyoteTimer > 0f)
            )
        {
            
            if (player.AirborneState.IsDashFastFalling)
            {
                player.AirborneState.IsDashFastFalling = false;
            }
            JumpBufferTimer = 0f;
            return true;
        }
        return false;
    }

    public bool CanAirJump()
    {
        if (
            JumpBufferTimer > 0f
            && (
                player.AirborneState.IsJumping
                || player.AirborneState.IsWallJumping
                || player.AirborneState.IsWallSlideFalling
                || player.AirborneState.IsAirDashing
                || player.AirborneState.IsDashFalling
                )
            && !_isWall
            && AmountOfJumpsLeft > 0
            )
        {
            if (player.AirborneState.IsDashFastFalling)
            {
                player.AirborneState.IsDashFastFalling = false;
            }
            JumpBufferTimer = 0f;
            player.AirborneState.IsFastFalling = false;
            
            return true;
        }
        
        else if (
            JumpBufferTimer > 0f
            && player.AirborneState.IsFalling
            && !player.AirborneState.IsWallSlideFalling
            && AmountOfJumpsLeft > 1
            )
        {
            AmountOfJumpsLeft--;
            JumpBufferTimer = 0f;
            player.AirborneState.IsFastFalling = false;

            return true;
        }
        return false;
    }

    public bool JumpBufferedOrCoyoteTimed()
    {
        if (
            JumpBufferTimer > 0f
            && !player.AirborneState.IsJumping
            && (
                _isGrounded
                || CoyoteTimer > 0f
                )
            )
        {
            JumpBufferTimer = 0f;

            return true;
        }
        
        return false;
    }

    public bool CanJumpCut()
    => player.AirborneState.IsJumping
        && Movement.CurrentVelocity.y > 0;

    //public bool CanWallJumpCut()

    #endregion


    #region Functionality

    public void ResetJumps() 
    => AmountOfJumpsLeft = MoveData.NumberOfJumpsAllowed;

    void InitiateJump()
    {
        Movement.SetVelocityY(MoveData.InitialJumpVelocity);

        player.AirborneState.ResetWallJumpValues();
        
        player.AirborneState.IsJumping = true;
        AmountOfJumpsLeft--;

        if (IsJumpCut)
        {
            player.AirborneState.IsFastFalling = true;
            player.AirborneState.FastFallReleaseSpeed = Movement.CurrentVelocity.y;
        }
    }

    #endregion
}
