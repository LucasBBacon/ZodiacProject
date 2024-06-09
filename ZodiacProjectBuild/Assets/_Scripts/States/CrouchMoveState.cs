using UnityEngine;

public class CrouchMoveState : State
{
    [Header("Animation Clip")]
    public AnimationClip animClip;

    #region States

    [Header("States")]
    [SerializeField] CrouchIdleState crouchIdleState;
    [SerializeField] RunState runState;

    #endregion


    #region Callback Functions

    public override void Enter()
    {
        base.Enter();

        core.collisionSensors.SetAllColliderHeight(0.8f);
        Animator.Play(animClip.name);
    }

    public override void Do()
    {
        base.Do();
        
        if (UserInput.instance.MoveInput.x == 0)
            Set(crouchIdleState);

        else if (UserInput.instance.MoveInput.y != -1 && !core.collisionSensors.IsCeiling)
            Set(runState);
    }

    public override void Exit()
    {
        base.Exit();

        core.collisionSensors.SetAllColliderHeight(1.6f);
    }

    #endregion
}