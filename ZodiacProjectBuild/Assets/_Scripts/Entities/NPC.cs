using UnityEngine;

public class NPC : Core
{
    public PatrolState patrolState;
    public CollectState collectState;

    void Start()
    {
        SetupInstances();
        Set(patrolState);
    }

    private void Update()
    {
        if(state.IsComplete)
        {
            if(state == collectState)
            {
                Set(patrolState);
            }
        }

        if(state == patrolState)
        {
            collectState.CheckForTarget();
            if(collectState.target != null)
            {
                Set(collectState, true);
            }
        }
        state.DoBranch();
    }

    private void FixedUpdate()
    {
        state.FixedDoBranch();
    }
}
