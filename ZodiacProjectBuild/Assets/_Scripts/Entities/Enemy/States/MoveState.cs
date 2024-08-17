using UnityEngine;

public class MoveState : State
{
    #region Blackboard Variables

    protected bool IsDetectingWall;
    protected bool IsDetectingLedge;
    protected bool IsPlayerInMinAgroRange;

    #endregion

    public MoveState(EnemyEntity entity, EntityStateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
    {
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        Movement?.SetVelocityX(EntityData.MovementSpeed * Movement.FacingDirection);
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    public override void StateChecks()
    {
        base.StateChecks();

        IsDetectingLedge = CollisionSensors.IsLedgeVertical;
        IsDetectingWall = CollisionSensors.IsWallBird;

        IsPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        Movement?.SetVelocityX(EntityData.MovementSpeed * Movement.FacingDirection);
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
        CollisionSensors.CollisionChecks();
    }

    #endregion
}
