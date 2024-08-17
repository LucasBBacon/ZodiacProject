using UnityEngine;

public class RangedAttackState : AttackState
{
    public RangedAttackState(EnemyEntity entity, EntityStateMachine stateMachine, string animBoolName, Transform attackPosition) : base(entity, stateMachine, animBoolName, attackPosition)
    {
    }
}
