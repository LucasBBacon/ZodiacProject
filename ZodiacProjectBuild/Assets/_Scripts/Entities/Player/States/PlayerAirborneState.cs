using System;
using UnityEngine;

public class PlayerAirborneState : PlayerState
{
    #region Blackboard Variables

    public bool IsJumping { get; set; }
    public bool IsFalling { get; set; }
    public bool IsFastFalling { get; set; }
    public bool IsPastApexThreshold { get; set;}
    public float FastFallTime { get; set; }
    public float FastFallReleaseSpeed { get; set; }
    public float TimePastApexThreshold { get; set; }
    
    public bool IsWallSlideFalling { get; private set; }
    
    public float ApexPoint { get; set; }
    public bool IsWallSliding { get; private set; }
    public float WallJumpApexPoint { get; set; }

    #region New WallJump

    public bool IsWallJumping { get; private set; }
    public bool IsPastWallJumpApexThreshold { get; private set; }
    public float TimePastWallJumpApexThreshold { get; private set; }
    public bool IsWallJumpFastFalling { get; private set; }

    public float WallJumpTime { get; private set; }
    public float WallJumpFastFallTime { get; private set; }
    public bool UseWallJumpMoveStats { get; set; }
    public bool IsWallJumpFalling { get; private set; }
    public float WallJumpFastFallReleaseSpeed { get; private set; }

    #endregion

    #endregion

    public PlayerAirborneState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        Animator.SetBool("inAir", true);

        if (_player.LedgeClimbState.IsLedgeFalling)
            _player.Invoke("NotLedgeFalling", 0.3f);
    }

    public override void StateExit()
    {
        base.StateExit();

        Animator.SetBool("inAir", false);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (InputManager.AttackInput && _player.AttackState.CanAttack())
        {
            ChangeState(_player.AttackState);
        }

        if (
            _player.WallSlideState.ShouldWallSlide()
            )
        {
            ChangeState(_player.WallSlideState);
        }

        if (
            !CollisionSensors.IsGrounded &&
            CollisionSensors.IsLedge &&
            !_player.LedgeClimbState.IsLedgeFalling
            )
        {
            ResetJumpValues();
            ResetWallJumpValues();

            _player.LedgeClimbState.SetDetectedPosition(_player.transform.position);

            ChangeState(_player.LedgeClimbState);
        }

        if (
            InputManager.JumpJustPressed
            )
        {
            if (_player.JumpState.CanJump())
            {
                _player.SpawnParticles(_player.JumpParticles);

                ChangeState(_player.JumpState);
            }

            if (_player.JumpState.CanAirJump())
            {
                _player.SpawnParticles(_player.JumpParticles);

                ChangeState(_player.JumpState);
            }

            if (_player.WallJumpState.CanWallJumpDueToPostBufferTimer())
            {
                // _player.WallJumpState.DetermineWallJumpDirection(CollisionSensors.IsWallFront);

                ChangeState(_player.WallJumpState);
            }

            // Debug.Log(_player.JumpState.NumberOfJumpsUsed);
        }


        else if (
            _player.JumpState.JumpBufferedOrCoyoteTimed()
            )
        {    
            _player.SpawnParticles(_player.JumpParticles);

            ChangeState(_player.JumpState);
        }

        if (HasLanded())
        {
            float landTime = Time.time;
            _player.SpawnParticles(_player.LandParticles);
            
            ChangeState(_player.IdleState);
        }

        if (
            InputManager.DashInput &&
            _player.DashState.CanDash()
            )
        {
            ChangeState(_player.DashState);
        }

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

        _player.DashState.DashPhysics();
        _player.ChariotState.ChariotPhysics();     
        JumpPhysics();
        WallJumpPhysics();

        Animator.SetFloat("yVelocity", Movement.VerticalVelocity);
        Animator.SetFloat("xVelocity", Mathf.Abs(Movement.HorizontalVelocity));

        if (IsWallJumping && !CollisionSensors.IsWall)
            IsWallJumping = false;

        if (CollisionSensors.IsLedge)
        {
            _player.LedgeClimbState.SetDetectedPosition(_player.transform.position);
        }

        Movement.Move(InputManager.MoveInput, MoveStats.MaxRunSpeed);
    }

    #endregion


    #region Checks

    public bool HasLanded()
    {
        if (
            (
                IsJumping || IsFalling ||
                IsWallJumping ||  IsWallJumpFalling ||
                IsWallSliding || IsWallSlideFalling || 
                _player.DashState.IsDashFastFalling || 
                _player.ChariotState.IsChariotFastFalling
            ) &&
            CollisionSensors.IsGrounded &&
            Movement.VerticalVelocity <= 0f
            )
        {
            ResetJumpValues();
            ResetWallJumpValues();
            _player.DashState.ResetDashes();
            _player.ChariotState.ResetData();

            Movement.SetVerticalVelocity(Physics2D.gravity.y);

            _player.JumpState.ResetJumps();

            // Debug.Log("Has Landed!");

            _player.TrailRenderer.emitting = false;
            
            if (_player.DashState.IsDashFastFalling && CollisionSensors.IsGrounded)
            {
                _player.DashState.ResetDashValues();
                return true;
            }
            if (_player.ChariotState.IsChariotFastFalling && CollisionSensors.IsGrounded)
            {
                _player.ChariotState.ResetValues();
                return true;
            }

            _player.DashState.ResetDashValues();
            _player.ChariotState.ResetValues();

            Animator.SetTrigger(Player.LAND);            

            return true;
        }
        
        return false;
    }

    #endregion
    

    #region Functionality

    public void JumpPhysics()
    {
        if (IsJumping)
        {
            // hit head
            if (CollisionSensors.BumpedHead)
            {
                IsFastFalling = true;
            }

            if (Movement.VerticalVelocity >= 0f)
            {
                // apex controls
                ApexPoint = Mathf.InverseLerp
                    (
                        MoveStats.InitialJumpVelocity,
                        0f,
                        Movement.VerticalVelocity
                    );

                if (ApexPoint > MoveStats.ApexThreshold)
                {
                    if (!IsPastApexThreshold)
                    {
                        IsPastApexThreshold = true;
                        TimePastApexThreshold = 0f;
                    }

                    if (IsPastApexThreshold)
                    {
                        TimePastApexThreshold += Time.fixedDeltaTime;
                        if (TimePastApexThreshold < MoveStats.ApexHangTime)
                            Movement.SetVerticalVelocity(0f);
                        else
                            Movement.SetVerticalVelocity(-0.01f); // start moving downward
                    }
                }

                else if (!IsFastFalling)
                {
                    Movement.IncrementVerticalVelocity(MoveStats.Gravity * Time.fixedDeltaTime);

                    if (IsPastApexThreshold)
                    {
                        IsPastApexThreshold = false;
                    }
                } 
            }

            else if (!IsFastFalling)
                Movement.IncrementVerticalVelocity(MoveStats.Gravity * MoveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime);

            else if (Movement.VerticalVelocity < 0f)
                if (!IsFalling)
                    IsFalling = true;
        }

        // normal falling (no jumping)
        if (
            IsFalling &&
            !IsJumping &&
            !CollisionSensors.IsGrounded
            )
            Movement.IncrementVerticalVelocity(MoveStats.Gravity * Time.fixedDeltaTime);

        // handle released jump deceleartion
        if (IsFastFalling)
        {
            if (FastFallTime > MoveStats.TimeForUpwardsCancel)
                Movement.IncrementVerticalVelocity(MoveStats.Gravity * MoveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime);
            else if (FastFallTime < MoveStats.TimeForUpwardsCancel)
                Movement.SetVerticalVelocity
                    (
                        Mathf.Lerp
                            (
                                FastFallReleaseSpeed,
                                0f,
                                FastFallTime / MoveStats.TimeForUpwardsCancel
                            )
                    );
            
            FastFallTime += Time.fixedDeltaTime;
        }
    }

    public void InitiateJump()
    {
        Movement.SetVerticalVelocity(MoveStats.InitialJumpVelocity);

        ResetWallJumpValues();
        IsJumping = true;
        _player.JumpState.NumberOfJumpsUsed++;

        if (!_player.TrailRenderer.emitting)
        {
            _player.TrailRenderer.emitting = true;
        }

        if (_player.JumpState.JumpReleasedDuringBuffer)
        {
            IsFastFalling = true;
            FastFallReleaseSpeed = Movement.VerticalVelocity;
        }
    }

    public void ResetJumpValues()
    {
        IsJumping = false;
        IsFalling = false;
        IsFastFalling = false;
        IsPastApexThreshold = false;
        
        FastFallTime = 0f;
    }


    public void WallJumpPhysics()
    {
        if (IsWallJumping)
        {
            WallJumpTime += Time.fixedDeltaTime;
            if (WallJumpTime > MoveStats.TimeTillJumpApex)
            {
                UseWallJumpMoveStats = false;
            }

            if (CollisionSensors.BumpedHead)
            {
                IsWallJumpFastFalling = true;
                UseWallJumpMoveStats = false;
            }

            if (Movement.VerticalVelocity >= 0f)
            {
                _player.AirborneState.WallJumpApexPoint = Mathf.InverseLerp(MoveStats.WallJumpDirection.y, 0f, Movement.VerticalVelocity);
            
                if (_player.AirborneState.WallJumpApexPoint > MoveStats.ApexThreshold)
                {
                    if (!IsPastWallJumpApexThreshold)
                    {
                        IsPastWallJumpApexThreshold = true;
                        TimePastWallJumpApexThreshold = 0f;
                    }

                    if (IsPastWallJumpApexThreshold)
                    {
                        TimePastWallJumpApexThreshold += Time.fixedDeltaTime;
                        if (TimePastWallJumpApexThreshold < MoveStats.ApexHangTime)
                        {
                            Movement.SetVerticalVelocity(0f);
                        }
                        else
                        {
                            Movement.SetVerticalVelocity(-0.01f);
                        }
                    }
                }

                else if (!IsWallJumpFastFalling)
                {
                    Movement.IncrementVerticalVelocity(MoveStats.WallJumpGravity * Time.fixedDeltaTime);

                    if (IsPastWallJumpApexThreshold)
                    {
                        IsPastWallJumpApexThreshold = false;
                    }
                }
            }

            else if (!IsWallJumpFastFalling)
            {
                Movement.IncrementVerticalVelocity(MoveStats.WallJumpGravity * Time.fixedDeltaTime);
            }

            else if (Movement.VerticalVelocity < 0f)
            {
                if (!IsWallJumpFalling)
                {
                    IsWallJumpFalling = true;
                }
            }
        }

        if (IsWallJumpFastFalling)
        {
            if (WallJumpFastFallTime >= MoveStats.TimeForUpwardsCancel)
            {
                Movement.IncrementVerticalVelocity(MoveStats.WallJumpGravity * MoveStats.WallJumpGravityOnReleaseMultiplier * Time.fixedDeltaTime);
            }
            else if (WallJumpFastFallTime < MoveStats.TimeForUpwardsCancel)
            {
                Movement.SetVerticalVelocity(Mathf.Lerp(WallJumpFastFallReleaseSpeed, 0f, WallJumpFastFallTime / MoveStats.TimeForUpwardsCancel));
            }

            WallJumpFastFallTime += Time.fixedDeltaTime;
        }
    }

    public void InitiateWallJump(GameObject particlesToSpawn = null)
    {
        if (!IsWallJumping)
        {
            IsWallJumping = true;
            UseWallJumpMoveStats = true;
        }

        _player.WallSlideState.StopWallSliding();

        _player.AirborneState.ResetJumpValues();
        WallJumpTime = 0f;
        Movement.SetVerticalVelocity(MoveStats.InitialJumpVelocity);

        int dirMultiplier = 0;
        Vector2 hitDir =  CollisionSensors.LastWallHit.collider.ClosestPoint(CollisionSensors.mainCollider.bounds.center);
        
        if (hitDir.x > _player.transform.position.x)
        {
            dirMultiplier = -1;
        }
        else
        {
            dirMultiplier = 1;
        }

        Movement.HorizontalVelocity = Mathf.Abs(MoveStats.WallJumpDirection.x) * dirMultiplier;

        _player.TrailRenderer.emitting = true;
    }

    public void ResetWallJumpValues()
    {
        _player.WallSlideState.IsWallSlideFalling = false;
        UseWallJumpMoveStats = false;
        IsWallJumping = false;
        IsWallJumpFalling = false;
        IsWallJumpFastFalling = false;
        IsPastWallJumpApexThreshold = false;

        WallJumpFastFallTime = 0f;
        WallJumpTime = 0f;
    }

    public void WallJumpWasReleased()
    {
        if (
            !_player.WallSlideState.IsWallSliding
            && CollisionSensors.IsWall
            && IsWallJumping
            )
        {
            if (IsPastWallJumpApexThreshold)
            {
                IsPastWallJumpApexThreshold = false;
                IsWallJumpFastFalling = true;
                WallJumpFastFallTime = MoveStats.TimeForUpwardsCancel;

                Movement.SetVerticalVelocity(0f);
            }
            else
            {
                IsWallJumpFastFalling = true;
                WallJumpFastFallTime = Movement.VerticalVelocity;
            }
        }
    }

    #endregion
}

