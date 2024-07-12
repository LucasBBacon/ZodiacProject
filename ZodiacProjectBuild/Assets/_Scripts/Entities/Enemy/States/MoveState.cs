using UnityEngine;

public class MoveState : State
{
    #region Blackboard Variables

    protected bool IsDetectingWall;
    protected bool IsDetectingLedge;
    protected bool IsPlayerInMinAgroRange;

    #endregion

    public MoveState(EnemyEntity entity, EntityStateMachine stateMachine) : base(entity, stateMachine)
    {
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        Movement?.SetHorizontalVelocity(EntityData.MovementSpeed * Movement.FacingDirection);
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    public override void StateChecks()
    {
        base.StateChecks();

        IsDetectingLedge = CollisionSensors.IsLedgeVertical;
        IsDetectingWall = CollisionSensors.IsWallFront;

        IsPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        Movement?.SetHorizontalVelocity(EntityData.MovementSpeed * Movement.FacingDirection);
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
    }

    #endregion
}
