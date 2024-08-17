using UnityEngine;

public class MeleeAttackState : AttackState
{
    public MeleeAttackState(EnemyEntity entity, EntityStateMachine stateMachine, string animBoolName, Transform attackPosition) : base(entity, stateMachine, animBoolName, attackPosition)
    {
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();

        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(
            AttackPosition.position,
            EntityData.AttackRadius,
            EntityData.PlayerMask
            );

        foreach (Collider2D detected in detectedObjects)
        {
            IDamageable damageable = detected.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.Damage(
                    new DamageData(
                        EntityData.AttackDamage,
                        entity.gameObject
                        )
                    );
            }

            IKnockbackable knockbackable = detected.GetComponent<IKnockbackable>();
            if (knockbackable != null)
            {
                knockbackable.Knockback(
                    new KnockbackData(
                        EntityData.KnockbackAngle,
                        EntityData.KnockbackStrength,
                        Movement.FacingDirection,
                        entity.gameObject
                        )
                    );
            }
        }

    }
}
