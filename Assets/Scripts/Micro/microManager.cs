/*
Rules
1. Guest list comes in a predetermined orde
2. Only one guest can be present at a time
3. Only one mask can be present at a time
4. The rule book must be present

*/
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

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

    // Add these inside microManager.cs
    
    public void UI_Button_ApproveGuest()
    {
        // Only allow this if we are actually in the inspection phase!
        if (StateMachine.currentState == InspectionState)
        {
            Debug.Log("Player clicked Approve!");
            // TODO: Play approve stamp sound
            StateMachine.ChangeState(DecisionState);
        }
    }

    public void UI_Button_RejectGuest()
    {
        if (StateMachine.currentState == InspectionState)
        {
            Debug.Log("Player clicked Reject!");
            // TODO: Play reject stamp sound
            StateMachine.ChangeState(DecisionState);
        }
    }

    [Header("Data Architecture")]
    public CutsceneSO[] dailyCutscenes;
    public DialogueSO[] dailyDialogues;

    [Header("UI: Master Panels")]
    public GameObject inspectionUIPanel; // Put ALL your drag/drop table UI in here!

    [Header("UI: Cutscene")]
    public GameObject cutscenePanel;
    public UnityEngine.UI.Image cutsceneImageDisplay;
    private int currentCutsceneIndex = 0;

    [Header("UI: Dialogue & Instructions")]
    public GameObject dialoguePanel;
    public GameObject npcCharacterArt; // The GameObject holding the NPC Image
    public RectTransform npcOffscreenPos; // Where they wait
    public RectTransform npcOnscreenPos;  // Where they stand

    [Header("UI: Speech Bubbles")]
    public GameObject playerBubble;
    public TextMeshProUGUI playerText;
    
    public GameObject npcBubble;
    public TextMeshProUGUI npcText;
    
    public GameObject clickToContinueTriangle;
    private int currentDialogueIndex = 0;

    // --- CUTSCENE LOGIC ---
    public void AdvanceCutscene()
    {
        currentCutsceneIndex++;
        CutsceneSO todayCutscene = dailyCutscenes[currentDay];

        if (currentCutsceneIndex < todayCutscene.sequenceImages.Length)
        {
            cutsceneImageDisplay.sprite = todayCutscene.sequenceImages[currentCutsceneIndex];
        }
        else
        {
            // Cutscene is over! Fade out (Faking it instantly for MVP, you can add DOTween/Coroutine later)
            cutscenePanel.SetActive(false);
            StateMachine.ChangeState(InstructionsState);
        }
    }

    // --- DIALOGUE LOGIC ---
    public void AdvanceDialogue()
    {
        currentDialogueIndex++;
        DialogueSO todayDialogue = dailyDialogues[currentDay];

        if (currentDialogueIndex < todayDialogue.lines.Length)
        {
            DisplayDialogueLine(todayDialogue.lines[currentDialogueIndex]);
        }
        else
        {
            // Dialogue over! Clean up and start the game!
            dialoguePanel.SetActive(false);
            npcCharacterArt.SetActive(false); // Hide the NPC
            inspectionUIPanel.SetActive(true); // TURN THE GAME UI ON!
            
            StateMachine.ChangeState(NextGuestState);
        }
    }

    public void DisplayDialogueLine(DialogueLine line)
    {
        clickToContinueTriangle.SetActive(true);

        if (line.isPlayerTalking)
        {
            playerBubble.SetActive(true);
            npcBubble.SetActive(false);
            playerText.text = line.text;
        }
        else
        {
            playerBubble.SetActive(false);
            npcBubble.SetActive(true);
            npcText.text = line.text;
        }
    }
}