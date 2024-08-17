using UnityEngine;

public class PlayerIdleState : PlayerState
{
    bool _isGrounded;
    bool _isCeiling;
    bool _isLedge;
    bool _isWall;

    public PlayerIdleState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();
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
            _isCeiling = CollisionSensors.IsCeiling;
            _isLedge = CollisionSensors.IsLedgeHorizontal;
            _isWall = CollisionSensors.IsWall;
        }
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (
            InputManager.instance.AttackInput
            && player.AttackState.CanAttack()
            )
        {
            Debug.Log("Entereing attacke");
            ChangeState(player.AttackState);
        }

        if (InputManager.instance.BlockInput)
        {
            ChangeState(player.BlockState);
        }

        if (Mathf.Abs(InputManager.instance.MoveInput.x) > MoveData.MoveThreshold)
        {
            ChangeState(player.RunState);
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
        
        else if (!CollisionSensors.IsGrounded)
        {
            ChangeState(player.AirborneState);
        }

        else if (InputManager.instance.DashInput && player.DashState.CanDash() && player.DashEnabled)
        {
            ChangeState(player.DashState);
        }

        if (
            InputManager.instance.AbilityUse[(int)AbilityInputs.First] &&
            (player.AbilityOneState.CanCheck() || player.AbilityOneState.CanAirCheck())
            )
        {
            ChangeState(player.AbilityOneState);
        }

        if (
            InputManager.instance.AbilityUse[(int)AbilityInputs.Second] &&
            (player.AbilityTwoState.CanCheck() || player.AbilityTwoState.CanAirCheck())
            )
        {
            ChangeState(player.AbilityTwoState);
        }

        if (
            InputManager.instance.AbilityUse[(int)AbilityInputs.Third] &&
            (player.AbilityThreeState.CanCheck() || player.AbilityThreeState.CanAirCheck())
            )
        {
            ChangeState(player.AbilityThreeState);
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
    }
}
