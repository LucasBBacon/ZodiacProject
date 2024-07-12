using UnityEngine;

public class PlayerWalkState : PlayerState
{
    public bool Turned;

    public PlayerWalkState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        Animator.SetBool("isWalking", true);
    }

    public override void StateExit()
    {
        base.StateExit();

        Animator.SetBool("isWalking", false);

        _player.Animator.speed = 1f;
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        _player.Animator.speed = Utilities.MappingUtil.Map
            (
                Mathf.Abs(Body.velocity.x), 
                0,
                MoveStats.MaxWalkSpeed,
                0.5f, 
                1f, 
                true
            );

        if (
            InputManager.MoveInput != Vector2.zero
            )
        {
            ChangeState(_player.RunState);
        }

        else if (
            InputManager.MoveInput == Vector2.zero
            )
        {
            _player.Animator.SetBool(Player.IS_WALKING, false);

            ChangeState(_player.IdleState);
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
        
        Movement.Move(InputManager.MoveInput, MoveStats.MaxWalkSpeed);
    }

    #endregion


    #region Effects

    public void HandleStopParticles()
    {
        if
        (
            CollisionSensors.IsGrounded &&
            Turned
            )
        {
            _player.SpawnParticles(_player.StopParticles, _player.transform.rotation);
            Turned = false;
        }
    }

    #endregion
}
