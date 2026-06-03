using System.Collections;
using UnityEngine;

public class MicroDecisionState : MicroState
{
    public MicroDecisionState(MicroManager manager, MicroStateMachine stateMachine) : base(manager, stateMachine) { }

    public override void EnterState()
    {
        base.EnterState();
        manager.guestItemManager.ClearInspectionSelection();
        manager.StartCoroutine(HandleGuestDeparture());
    }

    private IEnumerator HandleGuestDeparture()
    {
        manager.guestItemManager.TransferMaskBackToGuest(); 
        
        MaskSO mask = manager.guestListSO[manager.currentDay].guestList[manager.currentGuest].mask;
        SorSO rules = manager.dailyRules[manager.currentDay]; 

        bool actualMismatch = false;
        if (rules.color != null && rules.color.Length > 0 && System.Array.IndexOf(rules.color, mask.color) == -1) actualMismatch = true;
        if (rules.quality != null && rules.quality.Length > 0 && System.Array.IndexOf(rules.quality, mask.quality) == -1) actualMismatch = true;
        if (rules.theme != null && rules.theme.Length > 0 && System.Array.IndexOf(rules.theme, mask.theme) == -1) actualMismatch = true;

        // FIX 1: Grade using the locked-in memory instead of the live status!
        bool foundMismatch = manager.isMismatchProven; 

        if (manager.lastDecisionWasApprove)
        {
            if (actualMismatch) manager.LoseLife(); 
        }
        else
        {
            if (!foundMismatch) manager.LoseLife(); 
        }

        // --- 2. DETERMINE DIALOGUE REACTION ---
        GuestSO currentGuest = manager.guestListSO[manager.currentDay].guestList[manager.currentGuest].guest;
        if (currentGuest.conversation != null)
        {
            DialogueSO reaction = manager.lastDecisionWasApprove ? currentGuest.conversation.acceptDialogue : 
                (foundMismatch ? currentGuest.conversation.rejectValidDialogue : currentGuest.conversation.rejectInvalidDialogue);

            manager.PlayInGameDialogue(reaction);
            
            // FIX: Wait endlessly until the manager announces the dialogue is completely finished!
            yield return new WaitWhile(() => manager.isDialogueRunning);
            yield return new WaitForSeconds(0.5f); // Tiny buffer so they don't run away instantly
        }

        // --- 3. HIDE UI & FADE OUT ---
        bool isLastGuest = manager.currentGuest >= manager.guestListSO[manager.currentDay].guestList.Length - 1;

        if (manager.decisionPanel != null) manager.decisionPanel.SetActive(false); 

        if (isLastGuest)
        {
            if (manager.inspectionUICanvasGroup != null)
            {
                manager.inspectionUICanvasGroup.interactable = false;
                manager.StartCoroutine(manager.FadeUIRoutine(manager.inspectionUICanvasGroup, 1f, 0f, 2f));
            }
            if (manager.decisionUICanvasGroup != null) 
            {
                manager.decisionUICanvasGroup.interactable = false;
                manager.StartCoroutine(manager.FadeUIRoutine(manager.decisionUICanvasGroup, 1f, 0f, 2f));
            }
        }

        manager.StopDialogueAndRescueBubbles();

        // --- 4. WALK AWAY ---
        if (manager.lastDecisionWasApprove)
        {
            // The NEW cinematic "Let In" animation! (Scales up & fades without moving away)
            yield return manager.StartCoroutine(manager.guestItemManager.ScaleAndFadeRoutine(manager.activeGuest, 2.5f));
        }
        else
        {
            // The "Walk Forward" Kick Out animation! (Scales up, fades to black, and walks offscreen)
            yield return manager.StartCoroutine(manager.guestItemManager.WalkForwardAndOutRoutine(manager.activeGuest, 2.5f, 5, 30f));
        }

        Object.Destroy(manager.activeGuest);
        manager.activeGuest = null;
        manager.currentGuest++;

        if (isLastGuest)
        {
            manager.inspectionUIPanel.SetActive(false); 
            yield return new WaitForSeconds(1.0f); 
        }

        stateMachine.ChangeState(manager.NextGuestState);
    }
}