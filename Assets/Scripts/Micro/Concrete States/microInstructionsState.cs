using UnityEngine;

public class MicroInstructionsState : MicroState
{
    public MicroInstructionsState(MicroManager manager, MicroStateMachine stateMachine) : base(manager, stateMachine) { }

    public override void EnterState()
    {
        base.EnterState();
        
        // 1. Setup the Scroll of Rules
        manager.guestItemManager.SetupSOR(manager.currentDay);

        // 2. Slide the NPC in
        manager.dialoguePanel.SetActive(true);
        manager.npcCharacterArt.SetActive(true);
        
        // Using your existing animation system to slide them in!
        manager.guestItemManager.TeleportUIElement(manager.npcCharacterArt, manager.npcOffscreenPos);
        manager.StartCoroutine(manager.guestItemManager.MoveUIElement(manager.npcCharacterArt, manager.npcOnscreenPos, 1.5f));

        // 3. Do we have dialogue today?
        if (manager.currentDay < manager.dailyDialogues.Length && manager.dailyDialogues[manager.currentDay].lines.Length > 0)
        {
            // Show the first line!
            manager.DisplayDialogueLine(manager.dailyDialogues[manager.currentDay].lines[0]);
        }
        else
        {
            // No dialogue? Turn UI on and start game immediately.
            manager.dialoguePanel.SetActive(false);
            manager.inspectionUIPanel.SetActive(true);
            stateMachine.ChangeState(manager.NextGuestState);
        }
    }
}