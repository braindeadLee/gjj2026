using UnityEngine;

public class MicroInstructionsState : MicroState
{
    public MicroInstructionsState(MicroManager manager, MicroStateMachine stateMachine) : base(manager, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        stateMachine.ChangeState(manager.NextGuestState);
    }
    public override void ExitState()
    {
        base.ExitState();
    }
    public override void UpdateState()
    {
        base.UpdateState();
    }
    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }
}
