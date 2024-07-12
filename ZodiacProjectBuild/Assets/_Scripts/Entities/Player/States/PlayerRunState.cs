using UnityEngine;

public class PlayerRunState : PlayerState
{
    public PlayerRunState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        Animator.SetBool("isRunning", true);
    }

    public override void StateExit()
    {
        base.StateExit();

        if (_player.SpeedParticles.isPlaying)
            _player.SpeedParticles.Stop();

        Animator.SetBool("isRunning", false);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (InputManager.AttackInput && _player.AttackState.CanAttack())
        {
            ChangeState(_player.AttackState);
        }

        if (
            InputManager.MoveInput == Vector2.zero
            || Mathf.Abs(InputManager.MoveInput.y) > 0.1f
            )
        {
            ChangeState(_player.IdleState);
        }

        // else if (
        //     UserInput.MoveInput != Vector2.zero &&
        //     !UserInput.RunIsHeld
        //     )
        // {
        //     ChangeState(_player.WalkState);
        // }

        else if (
            InputManager.JumpJustPressed
            )
        {
            if (_player.JumpState.CanJump())
            {
                _player.SpawnParticles(_player.JumpParticles);

                ChangeState(_player.JumpState);
            }
        }

        else if (_player.JumpState.JumpBufferedOrCoyoteTimed())
        {
            _player.SpawnParticles(_player.JumpParticles);

            ChangeState(_player.JumpState);
        }

        // if (
        //     InputManager.DashInput &&
        //     (_player.DashState.CanDash() || _player.DashState.CanAirDash())
        //     )
        // {
        //     ChangeState(_player.DashState);
        // }

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


        DashParticles();
        HandleSpeedParticles();
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();

        Movement.Move(InputManager.MoveInput, MoveStats.MaxRunSpeed, true);
    }

    #endregion


    #region Effects

    void HandleSpeedParticles()
    {
        // Debug.Log(Body.velocity.x);
        if (Mathf.Abs(Movement.HorizontalVelocity) >= MoveStats.MaxRunSpeed - 2f)
            {
                // Debug.Log("Speed!");
                if (!_player.SpeedParticles.isPlaying)
                    _player.SpeedParticles.Play();
            }
        else
            if (_player.SpeedParticles.isPlaying)
                _player.SpeedParticles.Stop();
    }

    void DashParticles()
    {
        float counter = 0f + Time.deltaTime;
        // Debug.Log(counter);

        if (counter >= _player.DashParticleTime)
        {
            _player.DashParticles.Play();
            counter = 0f;
        }
    }

    #endregion
}