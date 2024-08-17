using UnityEngine;

public class SpecialAttackState : AttackState
{
    protected GameObject projectile;
    protected AreaEffector2D forceField;

    public SpecialAttackState(EnemyEntity entity, EntityStateMachine stateMachine, string animBoolName, Transform attackPosition) : base(entity, stateMachine, animBoolName, attackPosition)
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

    public override void TriggerAttack()
    {
        base.TriggerAttack();
    
        projectile = GameObject.Instantiate(
            EntityData.Projectile,
            (Vector2)AttackPosition.position + new Vector2(EntityData.ProjectileOffset.x * Movement.FacingDirection, EntityData.ProjectileOffset.y),
            AttackPosition.rotation
            );
        forceField = projectile.GetComponent<AreaEffector2D>();
        forceField.forceMagnitude *= Movement.FacingDirection;
    }

    public override void FinishAttack()
    {
        base.FinishAttack();
    }
}

public class DivingAttackState : AttackState
{
    public DivingAttackState(EnemyEntity entity, EntityStateMachine stateMachine, string animBoolName, Transform attackPosition) : base(entity, stateMachine, animBoolName, attackPosition)
    {
    }
}