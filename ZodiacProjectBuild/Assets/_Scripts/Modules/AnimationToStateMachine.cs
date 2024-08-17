using UnityEngine;

public class AnimationToStateMachine : MonoBehaviour
{
    public BirdEnemy birdEnemy;

    private void Awake()
    {
        birdEnemy = GetComponentInParent<BirdEnemy>();
    }

    private void TriggerAttack()
    {
        birdEnemy.MeleeAttackState.TriggerAttack();
    }

    private void FinishAttack()
    {
        birdEnemy.MeleeAttackState.FinishAttack();
        birdEnemy.SpecialAttackState.FinishAttack();
    }

    private void TriggerRangedAttack()
    {
        birdEnemy.SpecialAttackState.TriggerAttack();
    }
}
