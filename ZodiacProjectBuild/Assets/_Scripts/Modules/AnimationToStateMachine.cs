using UnityEngine;

public class AnimationToStateMachine : MonoBehaviour
{
    public BirdEnemy birdEnemy;

    private void Awake()
    {
        birdEnemy = GetComponentInParent<BirdEnemy>();
    }

    private void TriggerAttack()
    => birdEnemy.MeleeAttackState.AnimationTrigger();

    private void FinishAttack()
    => birdEnemy.MeleeAttackState.AnimationFinishedTrigger();
}
