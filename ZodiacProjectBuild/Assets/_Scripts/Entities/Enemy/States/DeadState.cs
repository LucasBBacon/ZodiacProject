using UnityEngine;

public class DeadState : State
{
    public DeadState(EnemyEntity entity, EntityStateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();

        GameObject.Instantiate(EntityData.DeathEntity, entity.transform.position, EntityData.DeathEntity.transform.rotation);
        GameObject.Instantiate(EntityData.DeathParticles, entity.transform.position, EntityData.DeathParticles.transform.rotation);
    
        entity.gameObject.SetActive(false);
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

    public override void StateChecks()
    {
        base.StateChecks();
    }
}
