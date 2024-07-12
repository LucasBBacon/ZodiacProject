using UnityEngine;

public class BirdDamageState : DamageState
{
    BirdEnemy enemy;

    public BirdDamageState(EnemyEntity entity, EntityStateMachine stateMachine, BirdEnemy enemy) : base(entity, stateMachine)
    {
        this.enemy = enemy;
    }

    public override void StateEnter()
    {
        base.StateEnter();

        Animator.SetBool(BirdEnemy.DAMAGE, true);
    }

    public override void StateExit()
    {
        base.StateExit();

        Animator.SetBool(BirdEnemy.DAMAGE, false);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        
        if (
            DamageTimeOver
            )
        {
            if (
            IsPlayerInMinAgroRange
            )
            {
                ChangeState(enemy.PlayerDetectedState);
            }

            else
            {
                ChangeState(enemy.LookForPlayerState);
            }
        }
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
    }
}
