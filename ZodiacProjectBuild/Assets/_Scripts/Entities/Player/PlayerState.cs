using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState
{
    public bool IsExitingState { get; protected set; }
    public bool IsAnimationFinished { get; protected set; }
    protected float startTime;
    protected float StateTime => Time.time - startTime;
    string animBoolName;

    #region Blackboard variables

    protected Player player;

    protected CollisionSensors CollisionSensors => player.CollisionSensors;
    protected Animator Animator => player.Animator;
    protected Movement Movement => player.Movement;
    protected Rigidbody2D Body => Movement.Body;
    protected SOMovementData MoveData => player.MoveData;

    #endregion

    #region StateMachine Wrappers

    /// <summary>
    /// Current StateMachine
    /// </summary>
    protected PlayerStateMachine stateMachine;
    /// <summary>
    /// Wrappers to avoid having to call machine.state and its functions
    /// </summary>
    public PlayerState State => stateMachine.CurrentState;
    
    protected void ChangeState(PlayerState newState, bool forceReset = false) 
    => stateMachine.ChangeState(newState, forceReset);

    #endregion

    public PlayerState(Player player, PlayerStateMachine stateMachine, string animBoolName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    #region Override Functions
    
    public virtual void StateEnter()
    {
        //Debug.Log("Enter " + this.GetType().Name);
        DoChecks();
        Animator.SetBool(animBoolName, true);
        startTime = Time.time;
        IsExitingState  = false;
        IsAnimationFinished = false;

    }
    public virtual void StateUpdate()
    {
        // player.JumpInputChecks();
        player.JumpState.JumpTimers();
        player.WallJumpState.WallJumpTimers();
        player.JumpInputChecks();
        player.WallJumpState.WallJumpChecks();
        player.AirborneState.CheckForFalling();
        player.ChariotState.ChariotTimers();
        player.AttackState.AttackTimers();
        if (player.AbilityOneState != null) player.AbilityOneState.UpdateAbilityTimer();
        if (player.AbilityTwoState != null) player.AbilityTwoState.UpdateAbilityTimer();
        if (player.AbilityThreeState != null) player.AbilityThreeState.UpdateAbilityTimer();
    }
    public virtual void StateFixedUpdate() { 
        DoChecks();
        CollisionSensors.CollisionChecks();
        player.ApplyVelocity();
    }
    public virtual void StateExit() 
    {
        Animator.SetBool(animBoolName, false);
        IsExitingState = true;
    }
    public virtual void DoChecks() {
        CollisionSensors.SlopeCheck();
    }
    public virtual void AnimationTrigger() { }
    public virtual void AnimationFinishedTrigger()
    => IsAnimationFinished = true;
    
    #endregion
    
}