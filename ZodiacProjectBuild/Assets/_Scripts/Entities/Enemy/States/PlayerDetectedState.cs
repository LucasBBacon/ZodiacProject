using UnityEngine;

public class PlayerDetectedState : State
{
    #region Blackboard Variables
    
    protected bool IsPlayerInMinAgroRange;
    protected bool IsPlayerInMaxAgroRange;
    protected bool PerformLongRangeAction;
    protected bool PerformCloseRangeAction;
    protected bool IsDetectingLedge;
    
    #endregion

    public PlayerDetectedState(EnemyEntity entity, EntityStateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
    {
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        Movement.SetVelocityZero();
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    public override void StateChecks()
    {
        base.StateChecks();

        IsPlayerInMaxAgroRange = entity.CheckPlayerInMinAgroRange();
        IsPlayerInMinAgroRange = entity.CheckPlayerInMaxAgroRange();
        PerformCloseRangeAction = entity.CheckPlayerInCloseRangeAction();

        IsDetectingLedge = CollisionSensors.IsLedgeVertical;
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        Movement.SetVelocityZero();

        if (Time.time >= StartTime + EntityData.LongRangeActionTime)
        {
            PerformLongRangeAction = true;
        }
    }

    #endregion
}
