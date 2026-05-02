/*
Rules
1. Guest list comes in a predetermined orde
2. Only one guest can be present at a time
3. Only one mask can be present at a time
4. The rule book must be present

*/
using Unity.VisualScripting;
using UnityEngine;

public class MicroManager : MonoBehaviour
{

    #region State Machine Variables

    public MicroStateMachine StateMachine { get; private set; }
    
    public MicroCutsceneState CutsceneState {get; private set; }
    public MicroInstructionsState InstructionsState {get; private set; }
    public MicroNextGuestState NextGuestState {get; private set; }
    public MicroInspectionState InspectionState {get; private set; }
    public MicroDecisionState DecisionState {get; private set; }
    public GuestItemManager guestItemManager {get; set;}

    #endregion

    #region UI
    // [SerializeField] private Canvas Table;
    // [SerializeField] private Canvas InspectionCanvas;
    // [SerializeField] private Canvas BackgroundCanvas;

    #endregion

    #region Items and Guests

    [SerializeField] public GuestlistSO[] guestListSO;
    public int currentDay = 0;
    // [SerializeField] protected
    // private int currentDay = 0;
    public int currentGuest = 0;
    public GameObject activeGuest {get; set;}
    public MaskSO activeMask {get; set;}
    // private RulebookSO rulebook;
    // private bool 

    #endregion

    private void Awake()
    {
        StateMachine = new MicroStateMachine();
        CutsceneState = new MicroCutsceneState(this, StateMachine);
        InstructionsState = new MicroInstructionsState(this, StateMachine);
        NextGuestState = new MicroNextGuestState(this, StateMachine);
        InspectionState = new MicroInspectionState(this, StateMachine);
        DecisionState = new MicroDecisionState(this, StateMachine);
    }

    private void Start()
    {
        if (GuestItemManager.Instance == null)
        {
            Debug.LogError("GuestItemManager instance not found!");
        }
        else
        {
            guestItemManager = GuestItemManager.Instance;
        }
        GameObject sorForTheDay = guestItemManager.SetupSOR(0);

        StateMachine.Initialize(CutsceneState);
    }

    private void Update()
    {
        StateMachine.currentState.UpdateState();
    }

    private void FixedUpdate()
    {
        StateMachine.currentState.FixedUpdateState();
    }
}