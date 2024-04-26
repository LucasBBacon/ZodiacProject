using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    /// <summary>
    /// Current State.
    /// </summary>
    public State state;

    /// <summary>
    /// Attempts to set a new state.
    /// </summary>
    /// <param name="newState">New state to be set.</param>
    /// <param name="forceReset">Should new state set force the previous state to reset.</param>
    public void Set(State newState, bool forceReset = false)
    {
        if(state != newState || forceReset)
        {
            state?.Exit();
            state = newState;
            state.Initialise(this);
            state.Enter();
        }
    }

    /// <summary>
    /// Returns a list of all states in the active StateMachine branch.
    /// </summary>
    /// <param name="list">Current list of all states.</param>
    /// <returns>List of all states in active branch.</returns>
    public List<State> GetActiveStateBranch(List<State> list = null)
    {
        if(list == null)
            list = new List<State>();

        if(state == null)
            return list;
        else
        {
            list.Add(state);
            return state.stateMachine.GetActiveStateBranch(list);
        }
    }
}