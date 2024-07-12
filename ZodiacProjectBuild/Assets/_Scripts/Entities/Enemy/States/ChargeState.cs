using UnityEngine;

public class ChargeState : State
{
    #region Blackboard Variables

    protected bool ChargeTimeOver;

    protected bool IsPlayerInMinAgroRange;
    protected bool PerformCloseRangeAction;

    protected bool IsDetectingLedge;
    protected bool IsDetectingWall;

    #endregion

    public ChargeState(EnemyEntity entity, EntityStateMachine stateMachine) : base(entity, stateMachine)
    {
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        ChargeTimeOver = false;

        Movement?.SetHorizontalVelocity(EntityData.ChargeSpeed * Movement.FacingDirection);
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    public override void StateChecks()
    {
        base.StateChecks();

        IsPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
        PerformCloseRangeAction = entity.CheckPlayerInCloseRangeAction();
        
        IsDetectingLedge = CollisionSensors.IsLedgeVertical;
        IsDetectingWall = CollisionSensors.IsWallFront;

    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        Movement?.SetHorizontalVelocity(EntityData.ChargeSpeed * Movement.FacingDirection);

        if (
            Time.time >= StartTime + EntityData.ChargeTime
            )
        {
            ChargeTimeOver = true;
        }
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
    }

    #endregion
}
