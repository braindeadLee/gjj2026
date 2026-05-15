using UnityEngine;

public class MicroCutsceneState : MicroState
{
    public MicroCutsceneState(MicroManager manager, MicroStateMachine stateMachine) : base(manager, stateMachine) { }

    public override void EnterState()
    {
        base.EnterState();
        
        // 1. Lock the game UI so they can't play during the story
        manager.inspectionUIPanel.SetActive(false);

        // 2. Do we have a cutscene for today?
        if (manager.currentDay < manager.dailyCutscenes.Length && manager.dailyCutscenes[manager.currentDay].sequenceImages.Length > 0)
        {
            manager.cutscenePanel.SetActive(true);
            
            // TODO: In the future, add your alpha fade-in coroutine here!
            
            // Load the first image
            manager.cutsceneImageDisplay.sprite = manager.dailyCutscenes[manager.currentDay].sequenceImages[0];
            manager.cutsceneImageDisplay.color = Color.white; // Ensure it's visible
        }
        else
        {
            // No cutscene for today? Skip straight to instructions!
            stateMachine.ChangeState(manager.InstructionsState);
        }
    }
}