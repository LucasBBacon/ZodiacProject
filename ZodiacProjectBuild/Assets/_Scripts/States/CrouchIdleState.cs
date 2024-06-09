using UnityEngine;

public class CrouchIdleState : State
{
    [Header("Animation Clip")]
    public AnimationClip animClip;

    #region States

    [Header("States")]
    [SerializeField] IdleState idleState;
    [SerializeField] CrouchMoveState crouchMoveState;

    #endregion


    #region Unity Callback Methods

    public override void Enter()
    {
        base.Enter();

        core.collisionSensors.SetAllColliderHeight(0.8f);
        Animator.Play(animClip.name);
    }

    public override void Do()
    {
        base.Do();
        
        if (UserInput.instance.MoveInput.x != 0)
            Set(crouchMoveState);

        else if (!core.collisionSensors.IsCeiling && UserInput.instance.MoveInput.y != -1)
            Set(idleState);
    }

    public override void Exit()
    {
        base.Exit();

        core.collisionSensors.SetAllColliderHeight(1.6f);
    }

    #endregion
}