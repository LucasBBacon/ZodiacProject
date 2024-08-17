using UnityEngine;

public class PlayerRunState : PlayerState
{
    bool _isGrounded;
    bool _isCeiling;
    bool _isLedge;
    bool _isWall;
    bool _isSlope;

    public PlayerRunState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        player.JumpState.ResetJumps();
    }

    public override void StateExit()
    {
        base.StateExit();

        if (player.SpeedParticles.isPlaying)
            player.SpeedParticles.Stop();
    }

    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSensors)
        {
            _isLedge = CollisionSensors.IsLedgeHorizontal;
            _isGrounded = CollisionSensors.IsGrounded;
            _isCeiling = CollisionSensors.IsCeiling;
            _isSlope = CollisionSensors.IsOnSlope;
            _isWall = CollisionSensors.IsWall;
        }
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (InputManager.instance.AttackInput && player.AttackState.CanAttack())
        {
            ChangeState(player.AttackState);
        }

        if (InputManager.instance.BlockInput)
        {
            ChangeState(player.BlockState);
        }

        if (Mathf.Abs(InputManager.instance.MoveInput.x) < MoveData.MoveThreshold)
        {
            ChangeState(player.IdleState);
        }

        else if (InputManager.instance.JumpJustPressed)
        {
            if (player.JumpState.CanJump())
            {
                ChangeState(player.JumpState);
            }
        }

        else if (player.JumpState.JumpBufferedOrCoyoteTimed())
        {
            ChangeState(player.JumpState);
        }
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();

        player.Move(
            MoveData.GroundAcceleration,
            MoveData.GroundDeceleration,
            InputManager.instance.MoveInput
            );
        
        // if (!_isSlope)
        // {
        //     Movement.SetVelocityX(MoveData.MaxRunSpeed * InputManager.instance.MoveInput.x);
        // }
        // else if (_isSlope && CollisionSensors.CanWalkOnSlope)
        // {
        //     Movement.SetVelocity(
        //         MoveData.MaxRunSpeed * CollisionSensors.SlopeNormalPerp.x * -InputManager.instance.MoveInput.x,
        //         MoveData.MaxRunSpeed * CollisionSensors.SlopeNormalPerp.y * -InputManager.instance.MoveInput.x
        //         );
        // }
    }

    #endregion


    #region Effects

    void HandleSpeedParticles()
    {
        // Debug.Log(Body.velocity.x);
        if (Mathf.Abs(Movement.CurrentVelocity.x) >= MoveData.MaxRunSpeed - 2f)
            {
                // Debug.Log("Speed!");
                if (!player.SpeedParticles.isPlaying)
                    player.SpeedParticles.Play();
            }
        else
            if (player.SpeedParticles.isPlaying)
                player.SpeedParticles.Stop();
    }

    void DashParticles()
    {
        float counter = 0f + Time.deltaTime;
        // Debug.Log(counter);

        if (counter >= player.DashParticleTime)
        {
            player.DashParticles.Play();
            counter = 0f;
        }
    }

    #endregion
}