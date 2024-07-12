using UnityEngine;

public class BirdSpecialAttackState : SpecialAttackState
{
    BirdEnemy birdEnemy;
    GameObject forceField;
    
    public BirdSpecialAttackState(EnemyEntity entity, EntityStateMachine stateMachine, Transform attackPosition, BirdEnemy birdEnemy, GameObject forceField) : base(entity, stateMachine, attackPosition)
    {
        this.birdEnemy = birdEnemy;
        this.forceField = forceField;
    }

    public override void StateEnter()
    {
        base.StateEnter();

        forceField.SetActive(false);
    }

    public override void StateExit()
    {
        base.StateExit();

        forceField.SetActive(false);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
    }
}
