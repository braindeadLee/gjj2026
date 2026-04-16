using UnityEngine;

public class MicroCutsceneState : MicroState
{
    public MicroCutsceneState(MicroManager manager, MicroStateMachine stateMachine) : base(manager, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();

        stateMachine.ChangeState(manager.InstructionsState);
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
