using System;
using UnityEngine;

public class PlayerAirborneState : PlayerState
{
    #region Blackboard Variables

    public float TimePastWallJumpApexThreshold { get; set; }
    public float WallJumpFastFallReleaseSpeed { get; set; }
    public float WallJumpFastFallTime { get; set; }
    public float FastFallReleaseSpeed { get; set; }
    public float WallJumpApexPoint { get; set; }
    public float WallJumpTime { get; set; }

    public bool IsPastWallJumpApexThreshold { get; set; }
    public bool IsWallJumpFastFalling { get; set; }
    public bool IsWallSlideFalling { get; set; }
    public bool IsWallJumpFalling { get; set; }
    public bool IsDashFastFalling { get; set; }
    public bool IsDashFalling { get; set; }
    public bool IsWallJumping { get; set; }
    public bool IsAirDashing { get; set; }
    public bool IsJumping { get; set; }
    public bool IsFalling { get; set; }

    public bool IsPastApexThreshold {
        get => _isPastApexThreshold;
        set => _isPastApexThreshold = value;
    }
    
    public bool IsFastFalling {
        get => _isFastFalling;
        set => _isFastFalling = value;
    }

    public float FastFallTime {
        get => _fastFallTime;
        set => _fastFallTime = value;
    }

    float _particleCounter;

    float _apexPoint;
    private bool _isPastApexThreshold;
    private float _timePastApexThreshold;
    private bool _isFastFalling;
    private float _fastFallTime;


    #endregion

    public PlayerAirborneState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();
    }

    public override void StateExit()
    {
        base.StateExit();

        player.JumpParticles.Stop();
    }

    public override void DoChecks()
    {
        base.DoChecks();

        if (
            CollisionSensors.IsWallLedge
            && !CollisionSensors.IsLedgeHorizontal
            )
        {
            player.LedgeClimbState.SetDetectedPosition(player.transform.position);
        }
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        _particleCounter += Time.deltaTime;

        if ((_particleCounter > player.DustFormationPeriod) && Body.velocity.y > 0)
        {
            player.JumpParticles.Play();

            _particleCounter = 0f;
        }

        if (InputManager.instance.JumpJustPressed)
        {
            if (player.JumpState.CanJump())
            {
                ChangeState(player.JumpState);
            }

            if (player.JumpState.CanAirJump())
            {
                ChangeState(player.JumpState);
            }

            if (player.WallJumpState.CanWallJumpDueToPostBufferTimer() && player.WallJumpEnabled)
            {
                player.WallJumpState.UseWallJumpMoveStats = true;
                ChangeState(player.WallJumpState);
            }
        }

        else if (player.JumpState.JumpBufferedOrCoyoteTimed())
        {
            ChangeState(player.JumpState);
        }
        
        if (HasLanded())
        {
            ChangeState(player.LandState);
        }

        if (player.WallSlideState.ShouldWallSlide())
        {
            ChangeState(player.WallSlideState);
        }

        else if (
            InputManager.instance.AttackInput
            && player.AttackState.CanAttack()
            )
        {
            ChangeState(player.AttackState);
        }

        else if (
            CollisionSensors.IsWallLedge
            && !CollisionSensors.IsLedgeHorizontal
            && !CollisionSensors.IsGrounded
            )
        {
            ChangeState(player.LedgeClimbState);
        }

        else
        {
            Animator.SetFloat("yVelocity", Movement.CurrentVelocity.y);
            Animator.SetFloat("xVelocity", Mathf.Abs(Movement.CurrentVelocity.x));
        }
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();

        JumpPhysics();
        WallJumpPhysics();

        if (player.WallJumpState.UseWallJumpMoveStats)
        {
            player.Move(
                MoveData.WallJumpMoveAcceleration,
                MoveData.WallJumpMoveDeceleration,
                InputManager.instance.MoveInput
                );
        }

        else
        {
            player.Move(
                MoveData.AirAcceleration,
                MoveData.AirDeceleration,
                InputManager.instance.MoveInput
                );
        }
    }

    #endregion


    #region Checks

    public void CheckForFalling()
    {
        if (
            !CollisionSensors.IsGrounded
            && !IsJumping
            && !IsFalling
            && !player.WallSlideState.IsWallSliding
            && !IsWallJumping
            && !player.DashState.IsDashing
            && !IsDashFastFalling
            && !player.LedgeClimbState.IsLedgeClimbing
            && !player.LedgeClimbState.IsLedgeHanging
            )
        {
            if (!IsFalling)
            {
                IsFalling = true;
            }

            ChangeState(player.AirborneState);
        }
    }

    public bool HasLanded()
    {
        if (
            (
                IsJumping
                || IsFalling
                || IsWallJumping
                || IsWallJumpFalling
                || player.WallSlideState.IsWallSliding
                || IsWallSlideFalling
                || IsDashFastFalling
            )
            && CollisionSensors.IsGrounded
            && Movement.CurrentVelocity.y <= 0f
            )
        {
            ResetJumpValues();
            player.WallSlideState.StopWallSliding();

            IsWallSlideFalling = false;

            ResetWallJumpValues();
            player.DashState.ResetDashes();

            Movement.SetVelocityY(Physics2D.gravity.y);

            player.JumpState.ResetJumps();

            // if (
            //     IsDashFastFalling
            //     && _isGrounded
            //     )
            // {
            //     if 
            // }
        
            player.DashState.ResetDashes();

            return true;
        }
        return false;
    }

    #endregion


    #region Reset Values

    public void ResetJumpValues()
    {
        IsJumping = false;
        IsFalling = false;
        IsFastFalling = false;
        FastFallTime = 0f;
        IsPastApexThreshold = false;   
    }

    public void ResetWallJumpValues()
    {
        IsWallSlideFalling = false;
        player.WallJumpState.UseWallJumpMoveStats = false;
        IsWallJumping = false;
        IsWallJumpFastFalling = false;
        IsWallJumpFalling = false;
        IsPastWallJumpApexThreshold = false;
        
        WallJumpFastFallTime = 0f;
        WallJumpTime = 0f;
    }

    #endregion


    #region Functionality

    public void JumpPhysics()
    {
        if (IsJumping)
        {
            if (CollisionSensors.IsCeiling)
            {
                IsFastFalling = true;
            }

            if (Movement.CurrentVelocity.y >= 0f)
            {
                _apexPoint = Mathf.InverseLerp(
                    MoveData.InitialJumpVelocity,
                    0f,
                    Movement.CurrentVelocity.y
                    );
                if (_apexPoint >= MoveData.ApexThreshold)
                {
                    if (!IsPastApexThreshold)
                    {
                        IsPastApexThreshold = true;
                        _timePastApexThreshold = 0f;
                    }

                    if (IsPastApexThreshold)
                    {
                        _timePastApexThreshold += Time.fixedDeltaTime;
                        if (_timePastApexThreshold < MoveData.ApexHangTime)
                        {
                            Movement.SetVelocityY(0f);
                        }
                        else
                        {
                            Movement.SetVelocityY(-0.01f);
                        }
                    }
                }

                else if (!IsFastFalling)
                {
                    Movement.SetVelocityY(Movement.CurrentVelocity.y + (MoveData.Gravity * Time.fixedDeltaTime));

                    if (IsPastApexThreshold)
                    {
                        IsPastApexThreshold = false;
                    }
                }
            }

            else if  (!IsFastFalling)
            {
                Movement.SetVelocityY(Movement.CurrentVelocity.y + (MoveData.Gravity * MoveData.GravityOnReleaseMultiplier * Time.fixedDeltaTime));
            }

            else if (Movement.CurrentVelocity.y < 0f)
            {
                if (!IsFalling)
                    IsFalling = true;
            }
        }

        if (
            IsFalling
            && !IsJumping
            && !CollisionSensors.IsGrounded
            )
        {
           Movement.SetVelocityY(Movement.CurrentVelocity.y + (MoveData.Gravity * Time.fixedDeltaTime));
        }

        if (IsFastFalling)
        {
            if (FastFallTime >= MoveData.TimeForUpwardsCancel)
            {
                Movement.SetVelocityY(Movement.CurrentVelocity.y + (MoveData.Gravity * MoveData.GravityOnReleaseMultiplier * Time.fixedDeltaTime));
            }
            else if (FastFallTime < MoveData.TimeForUpwardsCancel)
            {
                Movement.SetVelocityY(
                    Mathf.Lerp(
                        FastFallReleaseSpeed,
                        0f,
                        FastFallTime / MoveData.TimeForUpwardsCancel
                        )
                    );
            }

            FastFallTime += Time.fixedDeltaTime;
        }
    }

    public void WallJumpPhysics()
    {
        if (IsWallJumping)
        {
            WallJumpTime += Time.fixedDeltaTime;
            if (WallJumpTime >= MoveData.TimeTillJumpApex)
            {
                player.WallJumpState.UseWallJumpMoveStats = false;
            }

            if (CollisionSensors.IsCeiling)
            {
                IsWallJumpFastFalling = true;
                player.WallJumpState.UseWallJumpMoveStats = false;
            }

            if (Movement.CurrentVelocity.y >= 0f)
            {
                WallJumpApexPoint = Mathf.InverseLerp(
                    MoveData.WallJumpDirection.y,
                    0f, Movement.CurrentVelocity.y
                    );
                if (WallJumpApexPoint > MoveData.ApexThreshold)
                {
                    if (!IsPastWallJumpApexThreshold)
                    {
                        IsPastWallJumpApexThreshold = true;
                        TimePastWallJumpApexThreshold = 0f;
                    }

                    if (IsPastWallJumpApexThreshold)
                    {
                        TimePastWallJumpApexThreshold += Time.fixedDeltaTime;
                        if (TimePastWallJumpApexThreshold > MoveData.ApexHangTime)
                        {
                            Movement.SetVelocityY(0f);
                        }
                        else
                        {
                            Movement.SetVelocityY(-0.01f);
                        }
                    }
                }

                else if (!IsWallJumpFastFalling)
                {
                    Movement.SetVelocityY(Movement.CurrentVelocity.y + (MoveData.WallJumpGravity * Time.fixedDeltaTime));

                    if (IsPastWallJumpApexThreshold)
                    {
                        IsPastWallJumpApexThreshold = false;
                    }
                }
            }

            else if (!IsWallJumpFastFalling)
            {
                Movement.SetVelocityY(Movement.CurrentVelocity.y + (MoveData.WallJumpGravity * Time.fixedDeltaTime));
            }

            else if (Movement.CurrentVelocity.y < 0f)
            {
                if (!IsWallJumpFalling)
                {
                    IsWallJumpFalling = true;
                }
            }
        }

        if (IsWallJumpFastFalling)
        {
            if (WallJumpFastFallTime >= MoveData.TimeForUpwardsCancel)
            {
                Movement.SetVelocityY(Movement.CurrentVelocity.y + (MoveData.WallJumpGravity * MoveData.WallJumpGravityOnReleaseMultiplier * Time.fixedDeltaTime));
            }
            else if (WallJumpFastFallTime < MoveData.TimeForUpwardsCancel)
            {
                Movement.SetVelocityY(
                    Mathf.Lerp(
                        WallJumpFastFallReleaseSpeed,
                        0f,
                        WallJumpFastFallTime / MoveData.TimeForUpwardsCancel
                        )
                    );
            }

            WallJumpFastFallTime += Time.fixedDeltaTime;
        }
    }

    #endregion
}