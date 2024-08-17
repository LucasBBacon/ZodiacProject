using UnityEngine;

public class PlayerStunState : PlayerState
{
    public PlayerStunState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();

        Movement.SetVelocityX(0f);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (Time.time >= startTime + MoveData.StunTime)
        {
            ChangeState(player.IdleState);
        }
    }
}