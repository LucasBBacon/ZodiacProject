using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();

        Animator.SetBool("isIdle", true);

        Movement?.SetVelocityZero();
    
        _player.TrailRenderer.emitting = false;
    }

    public override void StateExit()
    {
        base.StateExit();

        Animator.SetBool("isIdle", false);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (
            !_player.CollisionSensors.IsGrounded
            )
        {
            ChangeState(_player.AirborneState);
        }

        // if (
        //     UserInput.MoveInput != Vector2.zero &&
        //     !UserInput.RunIsHeld
        //     )
        // {
        //     ChangeState(_player.WalkState);
        // }

        else if (
            InputManager.MoveInput.x != 0 &&
            InputManager.MoveInput.y == 0
            )
        {
            ChangeState(_player.RunState);
        }

       else if (
            InputManager.JumpJustPressed
            )
        {
            _player.SpawnParticles(_player.JumpParticles);

            ChangeState(_player.JumpState);
        }

        else if (
            _player.JumpState.JumpBufferedOrCoyoteTimed()
        )
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

        if (InputManager.AttackInput && _player.AttackState.CanAttack())
        {
            ChangeState(_player.AttackState);
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

        Movement?.SetVelocityZero();

        Movement.Move
            (
                InputManager.MoveInput,
                MoveStats.MaxWalkSpeed,
                true
            );
    }
}
