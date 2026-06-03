using UnityEngine;

public class MicroCutsceneState : MicroState
{
    public MicroCutsceneState(MicroManager manager, MicroStateMachine stateMachine) : base(manager, stateMachine) { }

    public override void EnterState()
    {
        base.EnterState();
        
        // --- THE GOLDEN UI RULE ---
        // 1. Turn OFF everything that isn't the Cutscene
        if(manager.inspectionUIPanel != null) manager.inspectionUIPanel.SetActive(false);
        if(manager.dialoguePanel != null) manager.dialoguePanel.SetActive(false);

        // 2. Tell the manager to figure out if there's a cutscene today and play it
        manager.StartCutsceneForToday();
    }
}