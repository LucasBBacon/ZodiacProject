public class PlayerHierophantState : PlayerAbilityState
{
    public PlayerHierophantState(
        Player player,
        PlayerStateMachine stateMachine,
        string animBoolName,
        AbilityInputs input
        ) : base(player, stateMachine, animBoolName, input)
    {

    }

    #region Callback Methods

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
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
    }

    public override void HeldBehaviour()
    {
        base.HeldBehaviour();
    }

    public override void ReleaseBehaviour()
    {
        base.ReleaseBehaviour();
    }

    public override bool CanCheck()
    {
        return base.CanCheck();
    }

    public override bool CanAirCheck()
    {
        return base.CanAirCheck();
    }

    #endregion
}
