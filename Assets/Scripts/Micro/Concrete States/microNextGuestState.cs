using System;
using UnityEngine;

public class MicroNextGuestState : MicroState
{
    private MaskSO currentMask;
    private GuestSO currentGuest;
    public MicroNextGuestState(MicroManager manager, MicroStateMachine stateMachine) : base(manager, stateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("Entered Next Guest State");

        if(manager.guestListSO[manager.currentDay].guestList.Length > 0){
            if(manager.currentGuest >= manager.guestListSO[manager.currentDay].guestList.Length){
                Debug.Log("No more guests for the day, moving to inspection state.");
                //insert end of day stuff
                ExitState();
                return;
            }
            currentMask = manager.guestListSO[manager.currentDay].guestList[manager.currentGuest].mask;
            currentGuest = manager.guestListSO[manager.currentDay].guestList[manager.currentGuest].guest;

            manager.activeGuest = manager.guestItemManager.SetupGuest(currentGuest, currentMask);

            //starting position is guest spawn position, then move to guest stand position
            manager.guestItemManager.TeleportUIElement(manager.activeGuest, manager.guestItemManager.guestSpawnPosition);
            manager.StartCoroutine((manager.guestItemManager.MoveUIElement(manager.activeGuest, manager.guestItemManager.guestStandPosition, 3f)));
            manager.guestItemManager.moveUIDoneEvent.AddListener(() => manager.StateMachine.ChangeState(manager.InspectionState));

            manager.currentGuest++;
        } else {
            Debug.LogError("No guests found for the current day!");
        }
    }
    public override void ExitState()
    {
        base.ExitState();
        manager.guestItemManager.moveUIDoneEvent.RemoveAllListeners();
    }
    public override void UpdateState()
    {
        base.UpdateState();
    }
    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }
}