public class FlyingBirdMoveState : MoveState
{
    FlyingBirdEnemy enemy;

    public FlyingBirdMoveState(EnemyEntity entity, EntityStateMachine stateMachine, FlyingBirdEnemy enemy, string animBoolName) : base(entity, stateMachine, animBoolName)
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
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
    }
}