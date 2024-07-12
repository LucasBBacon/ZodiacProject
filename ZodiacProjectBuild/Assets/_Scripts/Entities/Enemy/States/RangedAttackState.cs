using UnityEngine;

public class RangedAttackState : AttackState
{
    public RangedAttackState(EnemyEntity entity, EntityStateMachine stateMachine, Transform attackPosition) : base(entity, stateMachine, attackPosition)
    {
    }
}
