using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    #region Blackboard Variables

    public bool IsWallSliding { get; private set; }

    bool _isWallLedge;
    bool _isGrounded;
    bool _isLedge;
    bool _isWall;

    #endregion

    public PlayerWallSlideState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        if (player.WallSlideParticles.isPlaying)
        {
            player.WallSlideParticles.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        player.WallSlideParticles.gameObject.SetActive(true);
        player.WallSlideParticles.Play();

        if (player.MoveData.ResetJumpsOnWallSlide)
        {
            Debug.Log("Resetting");
            player.JumpState.ResetJumps();
        }

        player.AirborneState.ResetJumpValues();
        player.AirborneState.ResetWallJumpValues();

        player.AirborneState.IsWallSlideFalling = false;
        IsWallSliding = true;

        if (player.MoveData.ResetJumpsOnWallSlide)
            player.JumpState.ResetJumps();
    }

    public override void StateExit()
    {
        base.StateExit();

        player.WallSlideParticles.Stop();
    }

    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSensors)
        {
            _isLedge = CollisionSensors.IsLedgeHorizontal;
            _isWallLedge = CollisionSensors.IsWallLedge;
            _isGrounded = CollisionSensors.IsGrounded;
            _isWall = CollisionSensors.IsWall;
        }

        if (_isWallLedge && !_isLedge)
        {
            player.LedgeClimbState.SetDetectedPosition(player.transform.position);
        }
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (ShouldStopWallSliding())
        {
            player.AirborneState.IsWallSlideFalling = true;
            StopWallSliding();

            ChangeState(player.AirborneState);
        }

        else if (player.AirborneState.HasLanded())
        {
            ChangeState(player.LandState);
        }

        else if (
            InputManager.instance.JumpJustPressed
            && player.WallJumpState.CanWallJumpDueToPostBufferTimer()
            && player.WallJumpEnabled
            )
        {
            player.WallJumpState.UseWallJumpMoveStats = true;
            ChangeState(player.WallJumpState);
        }

        else if (CollisionSensors.IsGrounded)
        {
            ChangeState(player.IdleState);
        }
        // else if (!_isWall || (InputManager.instance.MoveInput.x != Movement.FacingDirection))
        // {
        //     ChangeState(player.AirborneState);
        // }

        else if (_isWallLedge && !_isLedge)
        {
            ChangeState(player.LedgeClimbState);
        }
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();

        Movement.SetVelocityY(
            Mathf.Lerp(
                Movement.CurrentVelocity.y,
                -MoveData.WallSlideVelocity,
                MoveData.WallSlideDecelerationSpeed * Time.fixedDeltaTime
                )
            );

        if (player.WallJumpState.UseWallJumpMoveStats)
            player.Move(
                MoveData.WallJumpMoveAcceleration,
                MoveData.WallJumpMoveDeceleration,
                InputManager.instance.MoveInput
                );

        MoveCheck();
    }

    #endregion


    #region Checks

    public bool ShouldWallSlide()
    {
        //Debug.Log(CollisionSensors.IsWall + ", " + !CollisionSensors.IsGrounded + ", " + !player.DashState.IsDashing);
        if (
            CollisionSensors.IsWall
            && !CollisionSensors.IsGrounded
            && !player.DashState.IsDashing
            )
        {
            if (
                Movement.CurrentVelocity.y < 0f &&
                !IsWallSliding
                )
            {
                return true;
            }
        }

        return false;
    }

    public bool ShouldStopWallSliding()
    {
        if (
            IsWallSliding
            && !CollisionSensors.IsWall
            && !CollisionSensors.IsGrounded
            && !player.AirborneState.IsWallSlideFalling
            )
        {
            return true;
        }

        return false;
    }

    #endregion


    #region Functionality

    void MoveCheck()
    {
        if (CollisionSensors.WallHit.collider != null)
        {
            Vector2 hitPosition = CollisionSensors.WallHit.collider.ClosestPoint(player.transform.position);

            if (InputManager.instance.MoveInput.x > 0 && hitPosition.x > player.transform.position.x)
            {
                player.Move(
                    MoveData.AirAcceleration,
                    MoveData.AirDeceleration,
                    Vector2.zero
                    );
            }

            else if (InputManager.instance.MoveInput.x < 0 && hitPosition.x < player.transform.position.x)
            {
                player.Move(
                    MoveData.AirAcceleration,
                    MoveData.AirDeceleration,
                    Vector2.zero
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
    }

    public void StopWallSliding()
    {
        if (IsWallSliding)
        {
            player.JumpState.AmountOfJumpsLeft--;

            IsWallSliding = false;
        }
    }

    #endregion
}