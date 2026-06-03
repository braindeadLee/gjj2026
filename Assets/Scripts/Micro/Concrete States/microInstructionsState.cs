using System.Collections;
using UnityEngine;

public class MicroInstructionsState : MicroState
{
    public MicroInstructionsState(MicroManager manager, MicroStateMachine stateMachine) : base(manager, stateMachine) { }

    public override void EnterState()
    {
        base.EnterState();
        
        // 0. GUARANTEE BUBBLES ARE DEAD ON ARRIVAL
        manager.playerBubble.SetActive(false);
        manager.npcBubble.SetActive(false);
        
        // --- THE GOLDEN UI RULE ---
        if(manager.cutscenePanel != null) manager.cutscenePanel.SetActive(false);
        if(manager.inspectionUIPanel != null) manager.inspectionUIPanel.SetActive(false);
        if(manager.decisionPanel != null) manager.decisionPanel.SetActive(false); 

        // Hand control over to a coroutine so we can enforce pacing!
        manager.StartCoroutine(InstructionsSequence());
    }

    private IEnumerator InstructionsSequence()
    {
        // 1. Fade from black
        manager.cutsceneFadeOverlay.gameObject.SetActive(true);
        manager.StartCoroutine(manager.FadeOverlayRoutine(1f, 0f, 1.5f));

        // 2. Setup the Scroll of Rules
        if (!manager.hasSpawnedSORForToday)
        {
            manager.guestItemManager.SetupSOR(manager.currentDay);
            manager.hasSpawnedSORForToday = true;
        }

        // 3. Turn ON the Dialogue Panel and slide the NPC in
        manager.dialoguePanel.SetActive(true);
        manager.npcCharacterArtImage.gameObject.SetActive(true);
        
        manager.guestItemManager.TeleportUIElement(manager.npcCharacterArtImage.gameObject, manager.npcOffscreenPos);
        
        // PAUSE execution until the NPC finishes safely walking in!
        yield return manager.StartCoroutine(manager.guestItemManager.HopUIElement(
            manager.npcCharacterArtImage.gameObject, 
            manager.npcOnscreenPos, 
            1.5f, 4, 30f));

        // 4. THE BREATHING ROOM: Let them stand there for 2 seconds
        yield return new WaitForSeconds(2.0f);

        // 5. Trigger the new string parser!
        manager.StartInstructionsForToday();
    }
}