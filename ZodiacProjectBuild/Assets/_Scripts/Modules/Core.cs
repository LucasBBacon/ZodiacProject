using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Core : MonoBehaviour
{
    #region Blackboard instances

    public  Rigidbody2D         body;
    public  Animator            animator;
    public  CollisionSensors    collisionSensors;
    public  Movement            movement;
    public  EntityData          data;

    #endregion

    /// <summary>
    /// Current StateMachine.
    /// </summary>
    public StateMachine stateMachine;
    /// <summary>
    /// Wrappers to avoid having to call machine.state and its functions
    /// </summary>
    public State        state           => stateMachine.state;
    protected void Set(State newState, bool forceReset = false) 
    => stateMachine.Set(newState, forceReset);

    /// <summary>
    /// Assigns this Core to all of the states found found in the scene hieararchy for the current game object.
    /// </summary>
    public void SetupInstances()
    {
        stateMachine = new StateMachine();

        State[] allChildStates = GetComponentsInChildren<State>();
        foreach(State state in allChildStates)
            state.SetCore(this);
    }

    #region Debug Functions

    /// <summary>
    /// Prints out all of the active states in the tree.
    /// </summary>
    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        if(Application.isPlaying)
        {
            List<State> states = stateMachine.GetActiveStateBranch();
            UnityEditor.Handles.Label(transform.position, gameObject.name + " Active States: " + string.Join(" > ", states));
        }
        #endif
    }

    #endregion
}