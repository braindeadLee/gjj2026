using UnityEngine;

public class MicroState : MonoBehaviour
{
    protected MicroManager manager;
    protected MicroStateMachine stateMachine;

    public MicroState(MicroManager manager, MicroStateMachine stateMachine){
        this.manager = manager;
        this.stateMachine = stateMachine;
    }

    public virtual void EnterState(){}
    public virtual void ExitState(){}
    public virtual void UpdateState(){}
    public virtual void FixedUpdateState(){}

}
