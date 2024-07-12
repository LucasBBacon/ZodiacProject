using UnityEngine;

public class LookForPlayerState : State
{
    #region Blackboard Variables

    protected bool TurnImmediately;
    protected bool IsPlayerInMinAgroRange;
    protected bool IsAllTurnsDone;
    protected bool IsAllTurnsTimeDone;

    protected float LastTurnTime;

    protected int AmountOfTurnsDone;    

    #endregion

    public LookForPlayerState(EnemyEntity entity, EntityStateMachine stateMachine) : base(entity, stateMachine)
    {
    }

    #region Callback Functionalitites

    public override void StateEnter()
    {
        base.StateEnter();

        IsAllTurnsDone = false;
        IsAllTurnsTimeDone = false;

        LastTurnTime = StartTime;
        AmountOfTurnsDone = 0;

        Movement?.SetHorizontalVelocity(0f);
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    public override void StateChecks()
    {
        base.StateChecks();
    
        IsPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        Movement?.SetHorizontalVelocity(0f);

        if (
            TurnImmediately
            )
        {
			Movement?.Turn();
			
            LastTurnTime = Time.time;
			AmountOfTurnsDone++;
			TurnImmediately = false;
		} 
        
        else if (
            Time.time >= LastTurnTime + EntityData.TimeBetweenTurns && 
            !IsAllTurnsDone
        )
        {
			Movement?.Turn();
			LastTurnTime = Time.time;
			AmountOfTurnsDone++;
		}

		if (
            AmountOfTurnsDone >= EntityData.AmountOfTurns
            )
        {
			IsAllTurnsDone = true;
		}

		if (
            Time.time >= LastTurnTime + EntityData.TimeBetweenTurns &&
            IsAllTurnsDone
            )
        {
			IsAllTurnsTimeDone = true;
		}
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
    }

    #endregion


    #region Functionality

    public void SetTurnImmediately(bool flip)
    => TurnImmediately = flip;

    #endregion
}