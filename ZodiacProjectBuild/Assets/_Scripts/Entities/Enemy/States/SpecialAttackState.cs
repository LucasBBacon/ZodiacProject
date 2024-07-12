using UnityEngine;

public class SpecialAttackState : AttackState
{
    public SpecialAttackState(EnemyEntity entity, EntityStateMachine stateMachine, Transform attackPosition) : base(entity, stateMachine, attackPosition)
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
}
