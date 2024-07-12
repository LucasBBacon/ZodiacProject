public class EntityStateMachine
{
    public State CurrentState { get; private set; }
    public State PreviousState { get; private set; }

    public void Initalize(State startingState)
    {
        CurrentState = startingState;
        CurrentState.StateEnter();
    }

    public void ChangeState(State newState, bool forceReset = false)
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
