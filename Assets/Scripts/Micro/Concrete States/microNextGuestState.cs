using System.Collections;
using UnityEngine;

public class MicroNextGuestState : MicroState
{
    private MaskSO currentMask;
    private GuestSO currentGuest;

    public MicroNextGuestState(MicroManager manager, MicroStateMachine stateMachine) : base(manager, stateMachine) { }

    public override void EnterState()
    {
        base.EnterState();
        manager.isMismatchProven = false;
        if (manager.decisionPanel != null) manager.decisionPanel.SetActive(false);

        if (manager.mismatchFoundIndicatorText != null) manager.mismatchFoundIndicatorText.color = manager.defaultMismatchColor; 

        // --- END OF DAY LOGIC ---
        if (manager.currentGuest >= manager.guestListSO[manager.currentDay].guestList.Length)
        {
            manager.StartCoroutine(EndOfShiftRoutine());
            return;
        }

        currentMask = manager.guestListSO[manager.currentDay].guestList[manager.currentGuest].mask;
        currentGuest = manager.guestListSO[manager.currentDay].guestList[manager.currentGuest].guest;
        manager.activeGuest = manager.guestItemManager.SetupGuest(currentGuest, currentMask);
        
        manager.guestItemManager.TeleportUIElement(manager.activeGuest, manager.guestItemManager.guestSpawnPosition);
        manager.StartCoroutine(ShiftAndGuestRoutine());
    }

    private IEnumerator ShiftAndGuestRoutine()
    {
        // 1. Shift Banner Logic
        if (manager.currentGuest == 0)
        {
            if (!manager.hasSpawnedSORForToday)
            {
                manager.guestItemManager.SetupSOR(manager.currentDay);
                manager.hasSpawnedSORForToday = true;
            }

            if (manager.cutsceneFadeOverlay != null && manager.cutsceneFadeOverlay.alpha > 0f)
            {
                manager.cutsceneFadeOverlay.gameObject.SetActive(true);
                manager.StartCoroutine(manager.FadeOverlayRoutine(1f, 0f, 1.5f));
            }

            if (manager.inspectionUICanvasGroup != null)
            {
                manager.inspectionUICanvasGroup.alpha = 0f;
                manager.inspectionUIPanel.SetActive(true);
            }

            manager.shiftBannerPanel.SetActive(true);
            
            // Apply the "Begin Shift" Sprite and keep the Day Number Text active!
            if (manager.shiftBannerImage != null)
            {
                manager.shiftBannerImage.gameObject.SetActive(true);
                manager.shiftBannerImage.sprite = manager.beginShiftSprite;
            }
            manager.dayText.gameObject.SetActive(true);
            manager.dayText.text = "Day " + (manager.currentDay + 1); 
            
            yield return manager.StartCoroutine(manager.FadeUIRoutine(manager.shiftBannerCanvasGroup, 0f, 1f, 1.0f));
            yield return new WaitForSeconds(2.0f);
            yield return manager.StartCoroutine(manager.FadeUIRoutine(manager.shiftBannerCanvasGroup, 1f, 0f, 1.0f));
            
            manager.shiftBannerPanel.SetActive(false);
            yield return new WaitForSeconds(1.0f);
        }

        // 2. Guest hops in from the shadows!
        yield return manager.StartCoroutine(manager.guestItemManager.ShadowHopUIElement(
            manager.activeGuest, 
            manager.guestItemManager.guestStandPosition, 
            3f, 6, 30f));

        // 3. Desk Fades In
        if (manager.currentGuest == 0)
        {
            if (manager.decisionPanel != null) manager.decisionPanel.SetActive(true);
            if (manager.decisionUICanvasGroup != null) manager.decisionUICanvasGroup.alpha = 0f;

            manager.inspectionUICanvasGroup.interactable = false;
            
            manager.StartCoroutine(manager.FadeUIRoutine(manager.inspectionUICanvasGroup, 0f, 1f, 1.0f));
            if (manager.decisionUICanvasGroup != null)
            {
                manager.StartCoroutine(manager.FadeUIRoutine(manager.decisionUICanvasGroup, 0f, 1f, 1.0f));
            }
            
            manager.StartCoroutine(UnlockDeskAfterFade(1.0f));
        }
        else
        {
            if (manager.decisionPanel != null) manager.decisionPanel.SetActive(true);
        }

        stateMachine.ChangeState(manager.InspectionState);
    }

    private IEnumerator UnlockDeskAfterFade(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (manager.inspectionUICanvasGroup != null)
            manager.inspectionUICanvasGroup.interactable = true;
    }

    // End of Shift Cinematic Sequence
    private IEnumerator EndOfShiftRoutine()
    {
        // 1. Hide Desk completely
        if (manager.inspectionUIPanel != null) manager.inspectionUIPanel.SetActive(false);
        if (manager.inspectionUICanvasGroup != null) manager.inspectionUICanvasGroup.alpha = 0f;

        // 2. Display "Shift Finished" Banner (No text overlay)
        manager.shiftBannerPanel.SetActive(true);
        if (manager.shiftBannerImage != null)
        {
            manager.shiftBannerImage.gameObject.SetActive(true);
            manager.shiftBannerImage.sprite = manager.shiftEndedSprite;
        }
        manager.dayText.gameObject.SetActive(false); 
        
        yield return manager.StartCoroutine(manager.FadeUIRoutine(manager.shiftBannerCanvasGroup, 0f, 1f, 1.0f));
        yield return new WaitForSeconds(2.0f);
        yield return manager.StartCoroutine(manager.FadeUIRoutine(manager.shiftBannerCanvasGroup, 1f, 0f, 1.0f));

        // 3. Evaluate if game is over or next day begins
        if (manager.currentDay + 1 >= manager.guestListSO.Length)
        {
            // --- END OF GAME SEQUENCE ---
            
            // Turn OFF the banner image so the text can appear alone
            if (manager.shiftBannerImage != null) manager.shiftBannerImage.gameObject.SetActive(false);
            manager.dayText.gameObject.SetActive(true);
            manager.dayText.text = "Verdict...";
            
            yield return manager.StartCoroutine(manager.FadeUIRoutine(manager.shiftBannerCanvasGroup, 0f, 1f, 1.0f));
            yield return new WaitForSeconds(2.0f);
            
            // Turn the image back ON and swap to Win/Loss sprites!
            if (manager.shiftBannerImage != null) manager.shiftBannerImage.gameObject.SetActive(true);
            manager.dayText.gameObject.SetActive(false);
            
            if (manager.currentLives > 0) 
            {
                if (manager.shiftBannerImage != null) manager.shiftBannerImage.sprite = manager.jobKeptSprite;
            }
            else 
            {
                if (manager.shiftBannerImage != null) manager.shiftBannerImage.sprite = manager.firedSprite;
            }

            yield return new WaitForSeconds(3.0f);
            
            yield return manager.StartCoroutine(manager.FadeUIRoutine(manager.shiftBannerCanvasGroup, 1f, 0f, 1.0f));
            manager.shiftBannerPanel.SetActive(false);
        }
        else
        {
            // PROCEED TO NEXT DAY
            manager.shiftBannerPanel.SetActive(false);
            manager.currentGuest = 0;
            manager.currentDay++; 
            manager.hasSpawnedSORForToday = false;

            if (manager.HasCutsceneForToday()) stateMachine.ChangeState(manager.CutsceneState);
            else if (manager.HasInstructionsForToday()) stateMachine.ChangeState(manager.InstructionsState);
            else stateMachine.ChangeState(manager.NextGuestState);
        }
    }
}