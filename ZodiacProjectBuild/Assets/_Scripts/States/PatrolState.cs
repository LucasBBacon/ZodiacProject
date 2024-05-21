using UnityEngine;

public class PatrolState : State 
{
    public NavigateState navigateState;
    public IdleState idleState;
    public Transform anchor1;
    public Transform anchor2;

    void GoToNextDestination()
    {
        Vector2 a1 = (Vector2)anchor1.position;
        Vector2 a2 = (Vector2)anchor2.position;

        navigateState.destination = navigateState.destination == a1 ? a2 : a1;

        Set(navigateState, true);
    }

    public override void Enter()
    {
        base.Enter();
        GoToNextDestination();
    }

    public override void Do()
    {
        base.Do();

        if(stateMachine.state == navigateState)
        {
            if(navigateState.IsComplete)
            {
                Set(idleState, true);
                Body.velocity = new Vector2(0, Body.velocity.y);
            }
        }

        else
        {
            if(stateMachine.state.StateTime > 1)
            {
                GoToNextDestination();
            }
        }
    }
}
