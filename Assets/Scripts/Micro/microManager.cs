using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class MicroManager : MonoBehaviour
{
    [Header("Debugging")]
    public bool debugSkipCutscenes = false;
    public bool debugSkipInstructions = false;

    #region State Machine Variables
    public MicroStateMachine StateMachine { get; private set; }
    public MicroCutsceneState CutsceneState {get; private set; }
    public MicroInstructionsState InstructionsState {get; private set; }
    public MicroNextGuestState NextGuestState {get; private set; }
    public MicroInspectionState InspectionState {get; private set; }
    public MicroDecisionState DecisionState {get; private set; }
    public GuestItemManager guestItemManager {get; set;}
    #endregion

    #region Items and Guests
    [SerializeField] public GuestlistSO[] guestListSO;
    public int currentDay = 0;
    public int currentGuest = 0;
    public GameObject activeGuest {get; set;}
    public MaskSO activeMask {get; set;}
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
        if(playerBubble != null) playerBubbleStartPos = playerBubble.GetComponent<RectTransform>().anchoredPosition;
        if(npcBubble != null) npcBubbleStartPos = npcBubble.GetComponent<RectTransform>().anchoredPosition;

        if (GuestItemManager.Instance == null) Debug.LogError("GuestItemManager instance not found!");
        else guestItemManager = GuestItemManager.Instance;
            
        if (HasCutsceneForToday()) StateMachine.Initialize(CutsceneState);
        else if (HasInstructionsForToday()) StateMachine.Initialize(InstructionsState);
        else StateMachine.Initialize(NextGuestState);
    }

    public bool HasCutsceneForToday()
    {
        if (debugSkipCutscenes) return false;
        foreach (CutsceneSO cutscene in dailyCutscenes)
        {
            if (cutscene != null && cutscene.DayToShow == currentDay && cutscene.sequenceImages.Length > 0) return true;
        }
        return false;
    }

    public bool HasInstructionsForToday()
    {
        if (debugSkipInstructions) return false;
        foreach (InstructionsSO iso in dailyInstructions)
        {
            if (iso != null && iso.DayToDisplay == currentDay && !string.IsNullOrEmpty(iso.conversationText)) return true;
        }
        return false;
    }

    private void Update() => StateMachine.currentState.UpdateState();
    private void FixedUpdate() => StateMachine.currentState.FixedUpdateState();

    public void UI_Button_ApproveGuest()
    {
        if (StateMachine.currentState == InspectionState)
        {
            lastDecisionWasApprove = true;
            StateMachine.ChangeState(DecisionState);
        }
    }

    public void UI_Button_RejectGuest()
    {
        if (StateMachine.currentState == InspectionState)
        {
            lastDecisionWasApprove = false;
            StateMachine.ChangeState(DecisionState);
        }
    }

    [Header("Data Architecture")]
    public CutsceneSO[] dailyCutscenes;
    public DialogueSO[] dailyDialogues;

    [Header("UI: Master Panels")]
    public GameObject inspectionUIPanel; 
    public GameObject decisionPanel;     

    [Header("UI: Shift Banners & Fading")]
    public CanvasGroup inspectionUICanvasGroup; 
    public CanvasGroup decisionUICanvasGroup; 
    
    [HideInInspector] public bool lastDecisionWasApprove = false; 
    [HideInInspector] public bool isMismatchProven = false; 
    [HideInInspector] public bool hasSpawnedSORForToday = false; 
    [HideInInspector] public bool isDialogueRunning = false;
    public GameObject shiftBannerPanel;
    public CanvasGroup shiftBannerCanvasGroup;
    
    // --- NEW: Custom Banner Sprites! ---
    public UnityEngine.UI.Image shiftBannerImage; 
    public Sprite beginShiftSprite;
    public Sprite shiftEndedSprite;
    public Sprite jobKeptSprite;
    public Sprite firedSprite;
    
    public TextMeshProUGUI dayText;

    [Header("UI: Cutscene")]
    public GameObject cutscenePanel;
    public UnityEngine.UI.Image cutsceneImageDisplay;
    private Coroutine dialogueCoroutine;

    [Header("UI: Cutscene Polish")]
    public CanvasGroup cutsceneFadeOverlay; 
    public GameObject cutsceneIndicator;    
    public CanvasGroup indicatorCanvasGroup; 
    public float cutsceneSlideDelay = 2.0f; 
    
    private bool canAdvanceCutscene = false;
    private Coroutine flashCoroutine;
    
    private CutsceneSO activeCutscene; 
    private int currentCutsceneIndex = 0;

    [Header("UI: Dialogue & Instructions")]
    public InstructionsSO[] dailyInstructions;
    public GameObject dialoguePanel;
    public UnityEngine.UI.Image npcCharacterArtImage; 
    public Sprite defaultNPCArt;                      
    public RectTransform npcOffscreenPos;
    public RectTransform npcOnscreenPos;

    [Header("UI: Speech Bubbles")]
    public GameObject playerBubble;
    public TextMeshProUGUI playerText;
    private Vector2 playerBubbleStartPos;
    
    public GameObject npcBubble;
    public TextMeshProUGUI npcText;
    private Vector2 npcBubbleStartPos;
    public Vector2 instructionsBubbleOffset = new Vector2(0, 0); 
    public Vector2 gameplayBubbleOffset = new Vector2(-20, 20);
    public GameObject clickToContinueTriangle;

    public struct ParsedLine { public bool isPlayer; public string text; }
    private System.Collections.Generic.List<ParsedLine> parsedDialogue = new();
    private int currentDialogueIndex = 0;
    private bool isAnimatingBubble = false;

    public void StartCutsceneForToday()
    {
        activeCutscene = null;
        foreach (CutsceneSO cutscene in dailyCutscenes)
        {
            if (cutscene != null && cutscene.DayToShow == currentDay) { activeCutscene = cutscene; break; }
        }

        if (activeCutscene != null && activeCutscene.sequenceImages.Length > 0)
        {
            cutscenePanel.SetActive(true);
            currentCutsceneIndex = 0;
            cutsceneImageDisplay.sprite = activeCutscene.sequenceImages[0];
            
            cutsceneFadeOverlay.alpha = 1f; 
            cutsceneFadeOverlay.gameObject.SetActive(true);
            
            StartCoroutine(FadeOverlayRoutine(1f, 0f, 1.5f)); 
            StartCoroutine(CutsceneSlideRoutine()); 
        }
        else StateMachine.ChangeState(InstructionsState);
    }

    public void AdvanceCutscene()
    {
        if (!canAdvanceCutscene) return;
        currentCutsceneIndex++;

        if (activeCutscene != null && currentCutsceneIndex < activeCutscene.sequenceImages.Length)
        {
            cutsceneImageDisplay.sprite = activeCutscene.sequenceImages[currentCutsceneIndex];
            StartCoroutine(CutsceneSlideRoutine()); 
        }
        else StartCoroutine(EndCutsceneRoutine());
    }

    private System.Collections.IEnumerator CutsceneSlideRoutine()
    {
        canAdvanceCutscene = false;
        cutsceneIndicator.SetActive(false);
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);

        yield return new WaitForSeconds(cutsceneSlideDelay);

        canAdvanceCutscene = true;
        cutsceneIndicator.SetActive(true);
        flashCoroutine = StartCoroutine(FlashIndicatorRoutine());
    }

    private System.Collections.IEnumerator FlashIndicatorRoutine()
    {
        while (true)
        {
            indicatorCanvasGroup.alpha = Mathf.PingPong(Time.time * 1.5f, 2f);
            yield return null;
        }
    }

    private System.Collections.IEnumerator EndCutsceneRoutine()
    {
        canAdvanceCutscene = false;
        cutsceneIndicator.SetActive(false);
        
        cutsceneFadeOverlay.gameObject.SetActive(true);
        yield return StartCoroutine(FadeOverlayRoutine(0f, 1f, 2f)); 

        cutscenePanel.SetActive(false);
        activeCutscene = null;

        if (HasInstructionsForToday()) StateMachine.ChangeState(InstructionsState);
        else StateMachine.ChangeState(NextGuestState);
    }

    public System.Collections.IEnumerator FadeOverlayRoutine(float startAlpha, float endAlpha, float duration)
    {
        float timer = 0f;
        cutsceneFadeOverlay.alpha = startAlpha;

        while (timer < duration)
        {
            cutsceneFadeOverlay.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        
        cutsceneFadeOverlay.alpha = endAlpha;
    }

    public void StartInstructionsForToday()
    {
        if (defaultNPCArt != null) npcCharacterArtImage.sprite = defaultNPCArt;

        InstructionsSO todayInstructions = null;
        foreach (InstructionsSO iso in dailyInstructions)
        {
            if (iso != null && iso.DayToDisplay == currentDay) { todayInstructions = iso; break; }
        }

        parsedDialogue.Clear();
        if (todayInstructions != null && !string.IsNullOrEmpty(todayInstructions.conversationText))
        {
            string[] lines = todayInstructions.conversationText.Split('\n');
            bool isPlayerSpeaking = false;

            foreach (string line in lines)
            {
                string cleanLine = line.Trim();
                if (string.IsNullOrEmpty(cleanLine)) continue;

                if (cleanLine.StartsWith("-P-")) { isPlayerSpeaking = true; continue; }
                if (cleanLine.StartsWith("-N-")) { isPlayerSpeaking = false; continue; }

                parsedDialogue.Add(new ParsedLine { isPlayer = isPlayerSpeaking, text = cleanLine });
            }
        }

        currentDialogueIndex = -1; 
        
        npcBubble.transform.SetParent(npcCharacterArtImage.transform, false);
        npcBubble.GetComponent<RectTransform>().anchoredPosition = instructionsBubbleOffset;
        npcBubbleStartPos = instructionsBubbleOffset;
        
        playerBubble.SetActive(false);
        npcBubble.SetActive(false);
        AdvanceDialogue();
    }

    public void AdvanceDialogue()
    {
        if (isAnimatingBubble) return; 
        currentDialogueIndex++;

        if (currentDialogueIndex < parsedDialogue.Count)
        {
            ParsedLine line = parsedDialogue[currentDialogueIndex];
            
            if (line.isPlayer) StartCoroutine(AnimateBubbleRoutine(playerBubble, playerText, line.text, playerBubbleStartPos));
            else StartCoroutine(AnimateBubbleRoutine(npcBubble, npcText, line.text, npcBubbleStartPos));
        }
        else StartCoroutine(EndInstructionsRoutine());
    }

    public void PlayInGameDialogue(DialogueSO dialogue)
    {
        if (dialogue == null || string.IsNullOrEmpty(dialogue.conversationText)) return;
        if (dialogueCoroutine != null) StopCoroutine(dialogueCoroutine);

        parsedDialogue.Clear();
        string[] lines = dialogue.conversationText.Split('\n');
        bool isPlayerSpeaking = false; 

        foreach (string line in lines)
        {
            string cleanLine = line.Trim();
            if (string.IsNullOrEmpty(cleanLine)) continue;

            if (cleanLine.StartsWith("-P-")) { isPlayerSpeaking = true; continue; }
            if (cleanLine.StartsWith("-N-")) { isPlayerSpeaking = false; continue; }

            parsedDialogue.Add(new ParsedLine { isPlayer = isPlayerSpeaking, text = cleanLine });
        }

        if (playerBubble != null) 
        {
            playerBubble.transform.SetParent(inspectionUIPanel.transform, false);
            playerBubbleStartPos = playerBubble.GetComponent<RectTransform>().anchoredPosition; 
        }

        if (activeGuest != null && npcBubble != null)
        {
            npcBubble.transform.SetParent(activeGuest.transform, false);
            npcBubble.GetComponent<RectTransform>().anchoredPosition = gameplayBubbleOffset; 
            npcBubbleStartPos = gameplayBubbleOffset; 
        }

        isDialogueRunning = true; 
        dialogueCoroutine = StartCoroutine(AutoAdvanceDialogueRoutine());
    }

    public void StopDialogueAndRescueBubbles()
    {
        isDialogueRunning = false; 
        if (dialogueCoroutine != null) StopCoroutine(dialogueCoroutine);

        if (playerBubble != null)
        {
            playerBubble.transform.SetParent(dialoguePanel.transform, false);
            playerBubble.SetActive(false);
        }
        if (npcBubble != null)
        {
            npcBubble.transform.SetParent(dialoguePanel.transform, false);
            npcBubble.SetActive(false);
        }
        isAnimatingBubble = false;
    }

    private System.Collections.IEnumerator AutoAdvanceDialogueRoutine()
    {
        for (int i = 0; i < parsedDialogue.Count; i++)
        {
            ParsedLine line = parsedDialogue[i];
            
            if (line.isPlayer) yield return StartCoroutine(AnimateBubbleRoutine(playerBubble, playerText, line.text, playerBubbleStartPos));
            else yield return StartCoroutine(AnimateBubbleRoutine(npcBubble, npcText, line.text, npcBubbleStartPos));

            yield return new WaitForSeconds(2.5f);
        }

        CanvasGroup pCg = playerBubble.GetComponent<CanvasGroup>();
        CanvasGroup nCg = npcBubble.GetComponent<CanvasGroup>();
        
        float t = 0;
        while(t < 0.5f)
        {
            if (playerBubble.activeSelf) pCg.alpha = Mathf.Lerp(1f, 0f, t / 0.5f);
            if (npcBubble.activeSelf) nCg.alpha = Mathf.Lerp(1f, 0f, t / 0.5f);
            t += Time.deltaTime;
            yield return null;
        }
        
        StopDialogueAndRescueBubbles(); 
    }

    private System.Collections.IEnumerator AnimateBubbleRoutine(GameObject activeBubble, TextMeshProUGUI textComp, string newText, Vector2 targetPos)
    {
        isAnimatingBubble = true;
        clickToContinueTriangle.SetActive(false);

        RectTransform rt = activeBubble.GetComponent<RectTransform>();
        CanvasGroup cg = activeBubble.GetComponent<CanvasGroup>();
        
        float duration = 0.35f; 
        float pushDistance = 65f; 

        if (activeBubble.activeSelf)
        {
            GameObject ghost = Instantiate(activeBubble, activeBubble.transform.parent);
            RectTransform ghostRt = ghost.GetComponent<RectTransform>();
            CanvasGroup ghostCg = ghost.GetComponent<CanvasGroup>();
            ghostCg.blocksRaycasts = false; 
            
            textComp.text = newText;
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
            
            rt.anchoredPosition = targetPos + new Vector2(0, -pushDistance);
            cg.alpha = 0f;

            float t = 0;
            while(t < duration)
            {
                float progress = t / duration;
                float ease = progress * progress * (3f - 2f * progress); 

                ghostRt.anchoredPosition = Vector2.Lerp(targetPos, targetPos + new Vector2(0, pushDistance), ease);
                ghostCg.alpha = Mathf.Lerp(1f, 0f, ease);

                rt.anchoredPosition = Vector2.Lerp(targetPos + new Vector2(0, -pushDistance), targetPos, ease);
                cg.alpha = Mathf.Lerp(0f, 1f, ease);

                t += Time.deltaTime;
                yield return null;
            }
            Destroy(ghost); 
        }
        else
        {
            textComp.text = newText;
            activeBubble.SetActive(true);

            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
            
            rt.anchoredPosition = targetPos + new Vector2(0, -pushDistance);
            cg.alpha = 0f;

            float t = 0;
            while(t < duration)
            {
                float progress = t / duration;
                float ease = progress * progress * (3f - 2f * progress);

                rt.anchoredPosition = Vector2.Lerp(targetPos + new Vector2(0, -pushDistance), targetPos, ease);
                cg.alpha = Mathf.Lerp(0f, 1f, ease);

                t += Time.deltaTime;
                yield return null;
            }
        }

        rt.anchoredPosition = targetPos;
        cg.alpha = 1f;

        clickToContinueTriangle.SetActive(true);
        isAnimatingBubble = false;
    }

    private System.Collections.IEnumerator EndInstructionsRoutine()
    {
        isAnimatingBubble = true;
        clickToContinueTriangle.SetActive(false);

        CanvasGroup pCg = playerBubble.GetComponent<CanvasGroup>();
        CanvasGroup nCg = npcBubble.GetComponent<CanvasGroup>();
        
        float t = 0;
        while(t < 0.5f)
        {
            if (playerBubble.activeSelf) pCg.alpha = Mathf.Lerp(1f, 0f, t / 0.5f);
            if (npcBubble.activeSelf) nCg.alpha = Mathf.Lerp(1f, 0f, t / 0.5f);
            t += Time.deltaTime;
            yield return null;
        }
        playerBubble.SetActive(false);
        npcBubble.SetActive(false);
        npcBubble.transform.SetParent(dialoguePanel.transform, false);
        
        yield return StartCoroutine(guestItemManager.HopUIElement(
            npcCharacterArtImage.gameObject, 
            npcOffscreenPos, 
            1.5f, 4, 30f));

        yield return new WaitForSeconds(2.0f);

        dialoguePanel.SetActive(false);
        npcCharacterArtImage.gameObject.SetActive(false);
        isAnimatingBubble = false;
        StateMachine.ChangeState(NextGuestState);
    }

    public System.Collections.IEnumerator FadeUIRoutine(CanvasGroup cg, float startAlpha, float endAlpha, float duration)
    {
        if (cg == null) yield break;
        float timer = 0f;
        cg.alpha = startAlpha;

        while (timer < duration)
        {
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        cg.alpha = endAlpha;
    }

    [Header("UI: Gameplay Rules & Lives")]
    public int currentLives = 4;
    public UnityEngine.UI.Image livesImageDisplay;
    public Sprite[] livesSprites; 
    public TextMeshProUGUI mismatchFoundIndicatorText; 
    public Color defaultMismatchColor = new Color(0.4f, 0.2f, 0.1f, 1f); 
    public Color triggeredMismatchColor = Color.red; 
    public SorSO[] dailyRules; 

    public void LoseLife()
    {
        currentLives--;
        if (currentLives > 0)
        {
            livesImageDisplay.sprite = livesSprites[currentLives - 1]; 
            Debug.Log("MISTAKE MADE! Lives remaining: " + currentLives);
        }
        else
        {
            livesImageDisplay.gameObject.SetActive(false);
            Debug.Log("GAME OVER! PLAYER RAN OUT OF LIVES!");
            StartCoroutine(GameOverSequence()); 
        }
    }

    private System.Collections.IEnumerator GameOverSequence()
    {
        if (inspectionUICanvasGroup != null) inspectionUICanvasGroup.alpha = 0f;
        if (decisionUICanvasGroup != null) decisionUICanvasGroup.alpha = 0f;
        if (inspectionUIPanel != null) inspectionUIPanel.SetActive(false);

        shiftBannerPanel.SetActive(true);
        
        // Hide the banner image so the "Verdict..." text appears alone!
        if (shiftBannerImage != null) shiftBannerImage.gameObject.SetActive(false); 
        dayText.gameObject.SetActive(true);
        dayText.text = "Verdict...";
        
        shiftBannerCanvasGroup.alpha = 0f;
        yield return StartCoroutine(FadeUIRoutine(shiftBannerCanvasGroup, 0f, 1f, 1.0f));
        yield return new WaitForSeconds(2.0f);
        
        // Turn the banner back ON and apply the Fired Sprite!
        if (shiftBannerImage != null)
        {
            shiftBannerImage.gameObject.SetActive(true);
            shiftBannerImage.sprite = firedSprite;
        }
        dayText.gameObject.SetActive(false); 
        
        yield return new WaitForSeconds(3.0f);
    }
}