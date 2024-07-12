using UnityEngine;

public class PlayerAnimationToStateMachine : MonoBehaviour
{
    Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    public void TriggerAttack()
    => player.AttackState.AnimationTrigger();

    public void FinishAttack()
    => player.AttackState.AnimationFinishedTrigger();
}
