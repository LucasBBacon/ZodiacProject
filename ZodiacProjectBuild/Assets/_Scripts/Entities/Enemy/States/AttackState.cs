using UnityEngine;

public class AttackState : State
{
    #region Blackboard Variables

    protected Transform AttackPosition;

    protected bool IsPlayerInMinAgroRange;

    #endregion

    public AttackState(EnemyEntity entity, EntityStateMachine stateMachine, Transform attackPosition) : base(entity, stateMachine)
    {
        this.AttackPosition = attackPosition;
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        IsAnimationFinished = false;

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
    }

    #endregion
}