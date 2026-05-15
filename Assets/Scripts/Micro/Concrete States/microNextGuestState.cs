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

        // 1. Check if the day is over
        if (manager.currentGuest >= manager.guestListSO[manager.currentDay].guestList.Length)
        {
            Debug.Log("Day is over! No more guests.");
            
            // THE FIX: Did we just beat the final day of the entire game?
            if (manager.currentDay + 1 >= manager.guestListSO.Length)
            {
                Debug.Log("🎉 YOU BEAT THE GAME! 🎉");
                // MVP Survival trick: Just reset to Day 1 so the game doesn't crash!
                manager.currentDay = 0; 
                manager.currentGuest = 0;
                stateMachine.ChangeState(manager.CutsceneState);
                return;
            }

            // Otherwise, go to tomorrow normally
            manager.currentGuest = 0;
            manager.currentDay++; 
            
            stateMachine.ChangeState(manager.CutsceneState);
            return;
        }

        // 2. Spawn the guest
        currentMask = manager.guestListSO[manager.currentDay].guestList[manager.currentGuest].mask;
        currentGuest = manager.guestListSO[manager.currentDay].guestList[manager.currentGuest].guest;

        manager.activeGuest = manager.guestItemManager.SetupGuest(currentGuest, currentMask);
        
        // 3. Start the arrival sequence
        manager.StartCoroutine(HandleGuestArrival());
    }

    private IEnumerator HandleGuestArrival()
    {
        // Tell the manager to animate the guest walking in (Assuming 3 seconds)
        // Ensure you have a target Vector2 in your GuestItemManager!
        manager.guestItemManager.TeleportUIElement(manager.activeGuest, manager.guestItemManager.guestSpawnPosition);
            manager.StartCoroutine(manager.guestItemManager.MoveUIElement(manager.activeGuest, manager.guestItemManager.guestStandPosition, 3f));

        // Wait for the exact duration of the walking animation
        yield return new WaitForSeconds(3.0f);

        // Hand control over to the player!
        stateMachine.ChangeState(manager.InspectionState);
    }
}