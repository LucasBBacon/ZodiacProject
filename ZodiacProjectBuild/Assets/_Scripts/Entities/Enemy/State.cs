using UnityEngine;

public class State
{
    public bool IsExitingState { get; protected set; }
    public bool IsAnimationFinished { get; protected set; }
    public float StartTime { get; protected set; }
    public float StateTime => Time.time - StartTime;

    protected EnemyEntity entity;
    protected EntityData EntityData => entity.EntityData;
    protected Animator Animator => entity.Animator;
    protected Movement Movement => entity.Movement;
    protected Rigidbody2D Body => Movement.Body;
    protected CollisionSensors CollisionSensors => entity.CollisionSensors;
    
    protected EntityStateMachine stateMachine;
    public State CurrentState => stateMachine.CurrentState;

    protected void ChangeState(State newState, bool forceReset = false)
    => stateMachine.ChangeState(newState, forceReset);

    public State(EnemyEntity entity, EntityStateMachine stateMachine)
    {
        this.entity = entity;
        this.stateMachine = stateMachine;
    }

    public virtual void StateEnter()
    {
        // Debug.Log("Enter " + this.GetType().Name);

        IsExitingState = false;
        StartTime = Time.time;
        IsAnimationFinished = false;

        StateChecks();
    }

    public virtual void StateExit()
    {
        IsExitingState = true;
    }

    public virtual void StateUpdate() { }

    public virtual void StateFixedUpdate()
    {
        StateChecks();
    }

    public virtual void StateChecks()
    {
        
    }

    public virtual void AnimationTrigger() {}
    public virtual void AnimationFinishedTrigger() => IsAnimationFinished = true;
}
