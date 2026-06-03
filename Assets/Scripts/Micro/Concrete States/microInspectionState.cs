using UnityEngine;

public class MicroInspectionState : MicroState
{
    public MicroInspectionState(MicroManager manager, MicroStateMachine stateMachine) : base(manager, stateMachine) { }

    public override void EnterState()
    {
        base.EnterState();
        GuestSO currentGuest = manager.guestListSO[manager.currentDay].guestList[manager.currentGuest].guest;
        if (currentGuest.conversation != null)
        {
            manager.PlayInGameDialogue(currentGuest.conversation.entryDialogue);
        }
    }

    public override void UpdateState()
    {
        base.UpdateState();
        
        // Locks in the proof and changes the text color to RED!
        if (manager.guestItemManager.CurrentInspectionStatus == InspectionStatus.MISMATCHED)
        {
            manager.isMismatchProven = true; 
            if (manager.mismatchFoundIndicatorText != null)
            {
                manager.mismatchFoundIndicatorText.color = manager.triggeredMismatchColor;
            }
        }
    }
}