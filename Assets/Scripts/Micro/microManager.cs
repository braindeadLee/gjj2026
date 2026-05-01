/*
Rules
1. Guest list comes in a predetermined orde
2. Only one guest can be present at a time
3. Only one mask can be present at a time
4. The rule book must be present

*/
using UnityEngine;

public class MicroManager : MonoBehaviour
{

    #region State Machine Variables

    public MicroStateMachine StateMachine { get; set; }
    
    public MicroCutsceneState CutsceneState {get; set; }
    public MicroInstructionsState InstructionsState {get; set; }
    public MicroNextGuestState NextGuestState {get; set; }
    public MicroInspectionState InspectionState {get; set; }
    public MicroDecisionState DecisionState {get; set; }

    #endregion

    #region UI
    [SerializeField] private Canvas Table;
    [SerializeField] private Canvas InspectionCanvas;
    [SerializeField] private Canvas BackgroundCanvas;

    #endregion

    #region Items and Guests



    #endregion

    private void Awake()
    {
        StateMachine = new MicroStateMachine();
        CutsceneState = new MicroCutsceneState(this, StateMachine);
        NextGuestState = new MicroNextGuestState(this, StateMachine);
        InspectionState = new MicroInspectionState(this, StateMachine);
        DecisionState = new MicroDecisionState(this, StateMachine);
    }

    private void Start()
    {
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