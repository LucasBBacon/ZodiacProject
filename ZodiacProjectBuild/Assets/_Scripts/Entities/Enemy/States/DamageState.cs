using UnityEngine;

public class DamageState : State
{
    protected bool DamageTimeOver;
    protected bool IsPlayerInMinAgroRange;

    public DamageState(EnemyEntity entity, EntityStateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();

        Movement.SetVelocityZero();

        DamageTimeOver = false;
        entity.ChangeToDamageState = false;
    }

    public override void StateChecks()
    {
        base.StateChecks();

        IsPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (Time.time >= StartTime + 0.25f)
            DamageTimeOver = true;
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
    }
}