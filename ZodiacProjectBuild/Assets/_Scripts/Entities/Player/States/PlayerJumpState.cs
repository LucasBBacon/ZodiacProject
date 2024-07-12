using UnityEngine;

public class PlayerJumpState : PlayerState
{
    
    #region Blackboard Variables

    
    public int NumberOfJumpsUsed { get; set; }
    public float JumpBufferTimer { get; set; }
    public bool JumpReleasedDuringBuffer { get; set;}

    public float CoyoteTimer { get; set; }

    #endregion

    public PlayerJumpState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }


    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        Animator.SetBool("inAir", true);

        _player.AirborneState.InitiateJump();

        // if (CollisionSensors.IsWall)
        //     ChangeState(_player.TouchingWallState);

        
        ChangeState(_player.AirborneState);
    }

    public override void StateExit()
    {
        base.StateExit();

        Animator.SetBool("inAir", false);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        // if (
        //     InputManager.DashInput &&
        //     (_player.DashState.CanDash() || _player.DashState.CanAirDash())
        //     )
        // {
        //     ChangeState(_player.DashState);
        // }

        if (
            InputManager.AbilityOne &&
            (_player.AbilityOneState.CanCheck() || _player.AbilityOneState.CanAirCheck())
            )
        {
            ChangeState(_player.AbilityOneState);
        }
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();

        Movement.Move(InputManager.MoveInput, MoveStats.MaxRunSpeed);
        
        Body.velocity = new Vector2(Body.velocity.x, Movement.VerticalVelocity);
    }

    #endregion


    #region Checks

    public bool CanJump()
    {
        if (
            JumpBufferTimer > 0f &&
            !_player.AirborneState.IsJumping &&
            (CollisionSensors.IsGrounded || CoyoteTimer > 0f)
        )
        {
            JumpBufferTimer = 0f;

            return true;
        }
        return false;
    }

    public bool CanAirJump()
    {
        // double jump
        if (
            JumpBufferTimer > 0f &&
            _player.AirborneState.IsJumping &&
            !CollisionSensors.IsWall &&
            NumberOfJumpsUsed < MoveStats.NumberOfJumpsAllowed
            )
        {
            JumpBufferTimer = 0f;

            _player.AirborneState.IsFastFalling = false;

            return true;
        }

        // handle air jump AFTER coyote time has elapsed
        else if (
            JumpBufferTimer > 0f &&
            _player.AirborneState.IsFalling &&
            NumberOfJumpsUsed < MoveStats.NumberOfJumpsAllowed - 1
            )
        {
            NumberOfJumpsUsed++;
            JumpBufferTimer = 0f;

            _player.AirborneState.IsFastFalling = false;

            return true;
        }

        return false;
    }

    public bool JumpBufferedOrCoyoteTimed()
    {
        if (
            JumpBufferTimer > 0f &&
            !_player.AirborneState.IsJumping &&
            (CollisionSensors.IsGrounded || CoyoteTimer > 0f)
            )
        {
            JumpBufferTimer = 0;

            return true;
        }

        return false;
    }

    #endregion


    #region Functionality

    public void ResetJumps() 
    => NumberOfJumpsUsed = 0;


    public void JumpTimers()
    {
        JumpBufferTimer -= Time.deltaTime;

        //HANDLE COYOTE TIMER
        if (!CollisionSensors.IsGrounded && !CollisionSensors.IsWall)
            CoyoteTimer -= Time.deltaTime;
        else 
            CoyoteTimer = MoveStats.JumpCoyoteTime;
    }

    #endregion
}
