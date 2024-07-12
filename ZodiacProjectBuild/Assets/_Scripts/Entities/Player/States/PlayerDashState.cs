using System.Collections;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    #region Blackboard Variables

    public Vector2 DashDirection;

    public bool IsDashing;
    public bool IsDashAttacking;
    public bool IsDashRefilling;
    public bool IsAirDashing;

    public int NumberOfDashesLeft;
    public int NumberOfDashesUsed;

    public float DashTimer;
    public float DashBufferTimer;
    public float WallJumpPostBufferTimer;
    public float DashOnGroundTimer;

    public int DashDirectionMult;

    public float DashFastFallTime { get; private set; }
    public float DashFastFallReleaseSpeed { get; private set; }
    public bool IsDashFastFalling { get; private set; }

    #endregion

    public PlayerDashState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        // _player.Sleep(MoveStats.DashSleepTime);

        // Vector2 lastDashDirection;

        // if (UserInput.MoveInput != Vector2.zero)
        //     lastDashDirection =  UserInput.MoveInput;
        // else
        //     lastDashDirection = Movement.IsFacingRight ? Vector2.right : Vector2.left;

        // IsDashing = true;
        
        // _player.AirborneState.IsJumping = false;

        // _player.StartCoroutine(StartDash(lastDashDirection));

        // if (
        //     CollisionSensors.IsGrounded
        //     )
        // {
        //     ChangeState(_player.IdleState);
        // }
        // else
        // {
        //     ChangeState(_player.AirborneState);
        // }

        InitiateDash();
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        //Debug.Log(IsDashing + ", " + CollisionSensors.IsGrounded + ", " + IsExitingState);

        if (_player.WallSlideState.ShouldWallSlide())
        {
            ChangeState(_player.AirborneState);
        }

        else if (
            !IsDashing &&
            !CollisionSensors.IsGrounded &&
            !IsExitingState
            )
        {
            ChangeState(_player.AirborneState);
        }

        else if (
            !IsDashing &&
            CollisionSensors.IsGrounded &&
            !IsExitingState
            )
        {
            ChangeState(_player.IdleState);
        }

        else if (InputManager.JumpJustPressed)
        {
            if (_player.JumpState.CanJump())
            {
                _player.SpawnParticles(_player.JumpParticles);
                ChangeState(_player.JumpState);
            }
        }
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();

        DashPhysics();
    }

    #endregion


    #region Check Functions

    public bool CanDash()
    {
        if (
            CollisionSensors.IsGrounded &&
            DashOnGroundTimer < 0 &&
            !IsDashing &&
            NumberOfDashesUsed < MoveStats.NumberOfDashes
            )
        {
            //Debug.Log("Can Dash");
            return true;
        }
        else
        {
            //Debug.Log("No Dash");
            return false;
        }
    }

    #endregion


    #region Functionality

    public void ResetDashes()
    => NumberOfDashesUsed = 0;

    public void ResetDashValues()
    {
        IsDashFastFalling = false;
        DashOnGroundTimer = -0.01f;
    }

    public void DashTimers()
    {
        if (CollisionSensors.IsGrounded)
            DashOnGroundTimer -= Time.deltaTime;
    }


    public void InitiateDash()
    {
        NumberOfDashesUsed++;
        IsDashing = true;
        DashTimer = 0f;
        DashOnGroundTimer = MoveStats.TimeBetweenDashesGround;

        DashDirection = Movement.IsFacingRight ? Vector2.right : Vector2.left;

        _player.AirborneState.ResetJumpValues();
        _player.AirborneState.ResetWallJumpValues();
    }

    public void DashPhysics()
    {
        if (IsDashing)
        {
            DashTimer += Time.fixedDeltaTime;
            
            if (DashTimer >= MoveStats.DashTime)
            {
                if (CollisionSensors.IsGrounded)
                {
                    ResetDashes();
                }
                
                IsAirDashing = false;
                IsDashing = false;

                if (
                    !_player.AirborneState.IsJumping
                    && !_player.AirborneState.IsWallJumping
                    )
                {
                    DashFastFallTime = 0f;
                    DashFastFallReleaseSpeed = Movement.VerticalVelocity;

                    if (!CollisionSensors.IsGrounded)
                    {
                        IsDashFastFalling = true;
                    }
                }

                return;
            }

            Movement.HorizontalVelocity = MoveStats.DashSpeed * CollisionSensors.SlopeNormalPerp.x * -DashDirection.x;
            Movement.VerticalVelocity = MoveStats.DashSpeed * CollisionSensors.SlopeNormalPerp.y;

            if (DashDirection.y != 0f)
            {
                Movement.SetVerticalVelocity(MoveStats.DashSpeed * DashDirection.y);
            }
        }
        // dash cut time
        else if (IsDashFastFalling)
        {
            if (Movement.VerticalVelocity > 0f)
            {
                if (DashFastFallTime < MoveStats.DashTimeForUpwardsCancel)
                {
                    Movement.SetVerticalVelocity
                        (
                            Mathf.Lerp
                                (
                                    DashFastFallReleaseSpeed,
                                    0f,
                                    DashFastFallTime / MoveStats.DashTimeForUpwardsCancel
                                )
                        );
                }

                else if (DashFastFallTime >= MoveStats.DashTimeForUpwardsCancel)
                {
                    Movement.IncrementVerticalVelocity
                        (
                            MoveStats.Gravity * MoveStats.DashGravityOnReleaseMultiplier * Time.fixedDeltaTime
                        );
                }

                DashFastFallTime += Time.fixedDeltaTime;
            }
            else
            {
                Movement.IncrementVerticalVelocity
                    (
                        MoveStats.Gravity * MoveStats.DashGravityOnReleaseMultiplier * Time.fixedDeltaTime
                    );
            }
        }
    }

    /*
    IEnumerator StartDash(Vector2 direction)
    {
        DashBufferTimer = 0f;

        startTime = Time.time;

        NumberOfDashesLeft--;
        IsDashAttacking = true;

        Movement.SetVerticalVelocity(false, 0f, 0f);

        while (Time.time - startTime <= MoveStats.DashAttackTime)
        {
            DashParticles();

            Movement.SetVelocity(MoveStats.DashSpeed, direction.normalized);

            yield return null;
        }

        startTime = Time.time;

        IsDashAttacking = false;

        Body.velocity = direction.normalized * MoveStats.DashEndSpeed;

        while (Time.time - startTime <= MoveStats.DashEndTime)
            yield return null;

        IsDashing = false;
    }

    IEnumerator RefillDash(int amount)
    {
        IsDashRefilling = true;

        yield return new WaitForSeconds(MoveStats.DashRefillTime);

        IsDashRefilling = false;

        NumberOfDashesLeft = Mathf.Min(MoveStats.DashAmount, NumberOfDashesLeft + amount);
    }
    */

    #endregion


    #region Effects

    void DashParticles()
    {
        float counter = 0f + Time.deltaTime;
        Debug.Log(counter);

        if (counter >= _player.DashParticleTime)
        {
            _player.DashParticles.Play();
            counter = 0f;
        }
    }

    #endregion
}
