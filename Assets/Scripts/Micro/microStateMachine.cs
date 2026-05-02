using UnityEngine;

public class MicroStateMachine
{
    public MicroState currentState {get; set;}

    public void Initialize(MicroState startingState){
        currentState = startingState;
        currentState.EnterState();
    }

    public void ChangeState(MicroState newState){
        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }
}
