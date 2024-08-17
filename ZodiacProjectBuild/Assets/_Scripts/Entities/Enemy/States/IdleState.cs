using UnityEngine;

public class IdleState : State
{
    #region Blackboard Variables

    protected float IdleTime;
    protected bool IdleTimeOver;
    protected bool FlipAfterIdle;

    protected bool IsPlayerInMinAgroRange;

    #endregion

    public IdleState(EnemyEntity entity, EntityStateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
    {
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        Movement?.SetVelocityX(0f);

        IdleTimeOver = false;

        SetRandomIdleTime();
    }

    public override void StateExit()
    {
        base.StateExit();
        
        // Debug.Log("Flip " + FlipAfterIdle);

        if (FlipAfterIdle)
        {
            Movement?.Turn();
            FlipAfterIdle = false;
        }
    }

    public override void StateChecks()
    {
        base.StateChecks();

        IsPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        Movement?.SetVelocityX(0f);

        if (Time.time >= StartTime + IdleTime)
            IdleTimeOver = true;
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
    }

    #endregion


    #region Functionality

    public void SetFlipAfterIdle(bool flip)
    => FlipAfterIdle = flip;

    void SetRandomIdleTime()
    => IdleTime = Random.Range
        (
            EntityData.MinIdleTime,
            EntityData.MaxIdleTime
        );

    #endregion
}
