using UnityEngine;

public class BirdDamageState : DamageState
{
    BirdEnemy enemy;

    public BirdDamageState(EnemyEntity entity, EntityStateMachine stateMachine, BirdEnemy enemy, string animBoolName) : base(entity, stateMachine, animBoolName)
    {
        this.enemy = enemy;
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
