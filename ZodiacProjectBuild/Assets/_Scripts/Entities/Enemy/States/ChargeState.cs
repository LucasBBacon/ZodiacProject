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

    public ChargeState(EnemyEntity entity, EntityStateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
    {
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        ChargeTimeOver = false;

        Movement.SetVelocityX(EntityData.ChargeSpeed * Movement.FacingDirection);
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
        IsDetectingWall = CollisionSensors.IsWallBird;
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        Movement.SetVelocityX(EntityData.ChargeSpeed * Movement.FacingDirection);

        if (Time.time >= StartTime + EntityData.ChargeTime)
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

public class HomingState : State
{
    #region Blackboard Variables

    protected bool ChargeTimeOver;

    protected bool IsPlayerInMinAgroRange;
    protected bool PerformCloseRangeAction;

    protected bool IsDetectingLedge;
    protected bool IsDetectingWall;

    #endregion

    public HomingState(EnemyEntity entity, EntityStateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
    {
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        ChargeTimeOver = false;

        Movement.SetVelocityX(EntityData.ChargeSpeed * Movement.FacingDirection);
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
        IsDetectingWall = CollisionSensors.IsWall;
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        Movement.SetVelocityX(EntityData.ChargeSpeed * Movement.FacingDirection);

        if (Time.time >= StartTime + EntityData.ChargeTime)
        {
            ChargeTimeOver = true;
        }
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
    }

    // public virtual Vector3 PreviousPlayerPos()
    // {

    // }

    #endregion
}