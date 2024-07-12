using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine
{
    /// <summary>
    /// Current State.
    /// </summary>
    public PlayerState CurrentState { get; private set; }
    public PlayerState PreviousState { get; private set; }

    public void InitalizeState(PlayerState startingState)
    {
        CurrentState = startingState;
        CurrentState.StateEnter();
    }

    /// <summary>
    /// Attempts to set a new state.
    /// </summary>
    /// <param name="newState">New state to be set.</param>
    /// <param name="forceReset">Should new state set force the previous state to reset.</param>
    public void ChangeState(PlayerState newState, bool forceReset = false)
    {
        if(CurrentState != newState || forceReset)
        {
            CurrentState?.StateExit();
            PreviousState = CurrentState;

            CurrentState = newState;
            CurrentState.StateEnter();
        }
    }
}