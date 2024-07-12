using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState
{
    public bool IsExitingState { get; protected set; }
    public bool IsAnimationFinished { get; protected set; }
    protected float startTime;
    public float StateTime => Time.time - startTime;

    #region Blackboard variables

    protected Player _player;

    protected CollisionSensors CollisionSensors => _player.CollisionSensors;
    protected Animator Animator => _player.Animator;
    protected Movement Movement => _player.Movement;
    protected Rigidbody2D Body => Movement.Body;
    protected MovementData MoveStats => _player.MoveStats;

    #endregion

    #region StateMachine Wrappers

    /// <summary>
    /// Current StateMachine
    /// </summary>
    protected PlayerStateMachine _stateMachine;
    /// <summary>
    /// Wrappers to avoid having to call machine.state and its functions
    /// </summary>
    public PlayerState State => _stateMachine.CurrentState;
    
    protected void ChangeState(PlayerState newState, bool forceReset = false) 
    => _stateMachine.ChangeState(newState, forceReset);

    #endregion

    public PlayerState(Player player, PlayerStateMachine stateMachine)
    {
        _player = player;
        _stateMachine = stateMachine;
    }

    #region Override Functions
    
    public virtual void StateEnter()
    {
        Debug.Log("Enter " + this.GetType().Name);

        IsExitingState  = false;
        IsAnimationFinished = false;
        startTime   = Time.time;
    }
    public virtual void StateUpdate()
    {
        _player.CheckInput();
        
        _player.CheckForFalling();

        _player.JumpState.JumpTimers();
        _player.DashState.DashTimers();
        _player.ChariotState.ChariotTimers();
        _player.AttackState.AttackTimers();
        _player.WallJumpState.WallJumpTimers();

        //_player.CheckForWall();
    }
    public virtual void StateFixedUpdate()
    {
        CollisionSensors.CollisionChecks();
        Movement.ApplyVelocity();
    }
    public virtual void StateExit() 
    {
        IsExitingState = true;
    }
    public virtual void AnimationTrigger() {}
    public virtual void AnimationFinishedTrigger() => IsAnimationFinished = true;
    
    #endregion
}