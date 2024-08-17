using UnityEngine;

public class BirdIdleState : IdleState
{
    BirdEnemy enemy;
    float _restingTime;

    public BirdIdleState(EnemyEntity entity, EntityStateMachine stateMachine, BirdEnemy enemy, string animBoolName) : base(entity, stateMachine, animBoolName)
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

        //Debug.Log(_restingTime);

        if (_restingTime > 0)
            _restingTime -= Time.deltaTime;

        if (_restingTime <= 0)
        {
            if (
                IsPlayerInMinAgroRange
                )
            {
                ChangeState(enemy.PlayerDetectedState);
            }

        
            else if (
                IdleTimeOver
                )
            {
                ChangeState(enemy.MoveState);
            }
        }
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
    }

    public void SetIsResting(float restTime)
    => _restingTime = restTime;
}
