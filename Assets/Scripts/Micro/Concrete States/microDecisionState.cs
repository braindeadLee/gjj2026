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
        // 1. Force the mask back onto the guest's face if it was left on the table
        // (Fake it: just teleport it and parent it back immediately)
        manager.guestItemManager.TransferMaskBackToGuest(); 

        // 2. Animate the guest walking off-screen (Assuming -1500f is off-screen left)
        manager.StartCoroutine(manager.guestItemManager.MoveUIElement(manager.activeGuest, -1500f, 0f, 2f));

        // Wait for them to leave
        yield return new WaitForSeconds(2.0f);

        // 3. Delete the guest object completely
        Object.Destroy(manager.activeGuest);
        manager.activeGuest = null;

        // 4. Increment the guest index
        manager.currentGuest++;

        // 5. BOOM. LOOP CLOSES. Call the next guest.
        stateMachine.ChangeState(manager.NextGuestState);
    }
}