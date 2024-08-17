using UnityEngine;

public class PlayerAbilityTest : PlayerAbilityState
{
    

    public PlayerAbilityTest(Player player, PlayerStateMachine stateMachine, string animBoolName, AbilityInputs input) : base(player, stateMachine, animBoolName, input)
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

    public override void StateUpdate()
    {
        base.StateUpdate();
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
    }

    public override void ReleaseBehaviour()
    {
        base.ReleaseBehaviour();

        Body.AddForce(Vector2.up * 30f, ForceMode2D.Impulse);
        isAbilityDone = true;
    }


    #region Checks

    public override bool CanCheck()
    => true;
        

    public override bool CanAirCheck()
    => true;

    #endregion
}