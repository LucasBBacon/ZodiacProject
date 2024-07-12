using UnityEngine;

public class MeleeAttackState : AttackState
{
    public MeleeAttackState(EnemyEntity entity, EntityStateMachine stateMachine, Transform attackPosition) : base(entity, stateMachine, attackPosition)
    {
    }
}
