using UnityEngine;

public class MicroInspectionState : MicroState
{
    public MicroInspectionState(MicroManager manager, MicroStateMachine stateMachine) : base(manager, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("Entered Inspection State");
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
