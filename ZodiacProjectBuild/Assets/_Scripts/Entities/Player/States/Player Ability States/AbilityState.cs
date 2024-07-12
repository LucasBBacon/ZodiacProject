public class AbilityState : PlayerState
{
    public bool IsAbilityDone { get; protected set; }

    public AbilityState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        IsAbilityDone = false;
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        // if (IsAbilityDone)
        // {
        //     if (CollisionSensors.IsGrounded && Movement?.VerticalVelocity < 0.01f)
        //     {
        //         ChangeState(_player.IdleState);
        //     }
        //     else
        //     {
        //         ChangeState(_player.AirborneState);
        //     }
        // }
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
    }

    public virtual bool CanCheck() => false;

    public virtual bool CanAirCheck() => false;

    public virtual void ResetData() { }
    
    public virtual void ResetValues() { }


    #endregion
}
