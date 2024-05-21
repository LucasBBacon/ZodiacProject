using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : MonoBehaviour
{
    public      bool    IsComplete          { get; protected set; }
    public      bool    IsAnimationFinished { get; protected set; }
    protected   float   startTime;
    public      float   StateTime                => Time.time - startTime;

    #region Blackboard variables

    protected   Core            core;
    protected   Rigidbody2D     Body          => core.body;
    protected   Animator        Animator      => core.animator;
    protected   Movement        Movement      => core.movement;
    protected   EntityData      Data          => core.data;

    #endregion

    #region StateMachine Wrappers

    /// <summary>
    /// Current StateMachine
    /// </summary>
    public      StateMachine    stateMachine;
    /// <summary>
    /// The StateMachine that called the current StateMachine
    /// </summary>
    protected   StateMachine    parent;
    /// <summary>
    /// Wrappers to avoid having to call machine.state and its functions
    /// </summary>
    public      State           state       => stateMachine.state;
    
    protected void Set(State newState, bool forceReset = false) => stateMachine.Set(newState, forceReset);

    #endregion


    /// <summary>
    /// Assigns the core and inialises the StateMachine for potential child states.
    /// </summary>
    /// <param name="_core">Core to be assigned.</param>
    public void SetCore(Core _core)
    {
        stateMachine = new StateMachine();
        core    = _core;
    }

    /// <summary>
    /// Prepares state for its next usage.
    /// </summary>
    /// <param name="_parent">Parent state of current state.</param>
    public void Initialise(StateMachine _parent)
    {
        parent      = _parent;
        IsComplete  = false;
        IsAnimationFinished = false;
        startTime   = Time.time;
    }

    #region Override Functions
    
    public virtual void Enter()     { }
    public virtual void Do()        { }
    public virtual void FixedDo()   { }
    public virtual void Exit()      { }
    public virtual void AnimationTrigger() { }
    public virtual void AnimationFinishedTrigger() => IsAnimationFinished = true;
    
    #endregion

    #region StateMachine Branching
    
    /// <summary>
    /// Calls all of the "Do" functions in the active branch.
    /// </summary>
    /// <remarks>
    /// Calls every node within the branch until the end leaf.
    /// </remarks>
    public void DoBranch()
    {
        Do();
        state?.DoBranch();
    }

    /// <summary>
    /// Calls all of the "FixedDo" functions in the active branch.
    /// </summary>
    /// <remarks>
    /// Calls every node within the branch until the end leaf.
    /// </remarks>
    public void FixedDoBranch()
    {
        FixedDo();
        state?.FixedDoBranch();
    }
    
    #endregion
}