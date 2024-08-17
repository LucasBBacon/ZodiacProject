using UnityEngine;

public class AttackState : State
{
    #region Blackboard Variables

    protected Transform AttackPosition;

    protected bool IsPlayerInMinAgroRange;

    #endregion

    public AttackState(EnemyEntity entity, EntityStateMachine stateMachine, string animBoolName, Transform attackPosition) : base(entity, stateMachine, animBoolName)
    {
        this.AttackPosition = attackPosition;
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        IsAnimationFinished = false;

        Movement?.SetVelocityX(0f);
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
    
        Movement?.SetVelocityX(0f);
    }

    public virtual void TriggerAttack() { }

    public virtual void FinishAttack()
    => IsAnimationFinished = true;

    #endregion
}