using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem; 

public class GuestItemManager : MonoBehaviour
{
    public static GuestItemManager Instance;
    [SerializeField] private Canvas canvas;
    public bool inspectionMode = false;
    
    #region Items
    private List<GameObject> activeItems = new();
    [SerializeField] private RectTransform itemsCanvas;
    #endregion

    #region Masks
    [Header("Masks")]
    [SerializeField] private GameObject _maskPrefab;
    [SerializeField] private MaskSO[] maskSOArray;
    [SerializeField] private RectTransform tablePanel;
    private GameObject activeMask;
    #endregion

    #region Guests
    [Header("Guests")]
    [SerializeField] private GameObject _guestPrefab;
    [SerializeField] private GuestSO[] guestSOArray;
    [SerializeField] private RectTransform guestsPanel;
    private GameObject activeGuest;
    #endregion

    #region Scroll of Rules
    [SerializeField] private GameObject _sorPrefab;
    [SerializeField] private SorSO[] sorSOArray;
    private GameObject activeSOR;
    #endregion

    #region Inspection
    [Header("Cursors")]
    public Texture2D defaultCursor;
    public Texture2D defaultClickCursor;
    public Texture2D inspectCursor;
    public Texture2D inspectClickCursor;
    public Vector2 cursorHotspot = Vector2.zero; 

    private List<AttributeSO> firstSelectedAttributes;
    private List<AttributeSO> secondSelectedAttributes;

    private InspectionStatus inspectionStatus;
    private Item firstSelectedItem;
    private Item secondSelectedItem;
    public InspectionStatus CurrentInspectionStatus => inspectionStatus; 
    #endregion

    #region Debugging
    [Header("Debugging")]
    [SerializeField] private Button debugToggleInspectionButton;
    [SerializeField] private Button debugSpawnMaskButton;
    [SerializeField] private Button debugSpawnGuestButton;
    [SerializeField] private Button debugTransferMaskButton;
    [SerializeField] private TextMeshProUGUI debugInspectionModeText;
    [SerializeField] private Image debugPanel;
    [SerializeField] private bool debugMode = false;
    #endregion

    #region Animations
    [SerializeField] public RectTransform guestSpawnPosition;
    [SerializeField] public RectTransform guestStandPosition;

    public UnityEvent teleportUIDoneEvent;
    public UnityEvent moveUIDoneEvent;
    public UnityEvent scaleUIDoneEvent;
    public UnityEvent colorUIDoneEvent;
    #endregion

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        inspectionMode = false;
        if (debugMode)
        {
            debugPanel.gameObject.SetActive(true);
            if (debugInspectionModeText != null) debugInspectionModeText.text = "Inspection Mode: " + (inspectionMode ? "ON" : "OFF");
        }
        else debugPanel.gameObject.SetActive(false);

        ToggleInspectionMode(false);
    }

    private void Update()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Texture2D clickTex = inspectionMode ? inspectClickCursor : defaultClickCursor;
            if (clickTex != null) Cursor.SetCursor(clickTex, cursorHotspot, UnityEngine.CursorMode.Auto);
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            Texture2D releaseTex = inspectionMode ? inspectCursor : defaultCursor;
            if (releaseTex != null) Cursor.SetCursor(releaseTex, cursorHotspot, UnityEngine.CursorMode.Auto);
        }
    }

    private void OnEnable()
    {
        if (debugToggleInspectionButton != null) debugToggleInspectionButton.onClick.AddListener(ToggleInspectionMode);
        if (debugSpawnMaskButton != null) debugSpawnMaskButton.onClick.AddListener(SpawnMask);
        if (debugSpawnGuestButton != null) debugSpawnGuestButton.onClick.AddListener(SpawnGuest);
        if (debugTransferMaskButton != null) debugTransferMaskButton.onClick.AddListener(TransferMask);
    }

    private void OnDisable()
    {
        if (debugToggleInspectionButton != null) debugToggleInspectionButton.onClick.RemoveListener(ToggleInspectionMode);
        if (debugSpawnMaskButton != null) debugSpawnMaskButton.onClick.RemoveListener(SpawnMask);
        if (debugSpawnGuestButton != null) debugSpawnGuestButton.onClick.RemoveListener(SpawnGuest);
        if (debugTransferMaskButton != null) debugTransferMaskButton.onClick.RemoveListener(TransferMask);
    }

    #region Items
    public void ToggleInspectionMode(bool value)
    {
        inspectionMode = value;
        if (debugInspectionModeText != null) 
            debugInspectionModeText.text = "Inspection Mode: " + (inspectionMode ? "ON" : "OFF");

        if (defaultCursor != null && inspectCursor != null)
        {
            Cursor.SetCursor(value ? inspectCursor : defaultCursor, cursorHotspot, UnityEngine.CursorMode.Auto);
        }

        activeItems.RemoveAll(item => item == null);

        foreach (GameObject obj in activeItems)
        {
            Item itemComponent = obj.GetComponent<Item>();
            if (itemComponent != null)
            {
                itemComponent.ToggleDraggable(!value);
                itemComponent.ToggleInspectable(value);
            }
        }
    }

    public void ToggleInspectionMode() => ToggleInspectionMode(!inspectionMode);
    #endregion

    #region Masks
    public GameObject SetupMask(MaskSO maskSO, RectTransform parentPanel = null)
    {
        if(parentPanel == null) parentPanel = tablePanel;
        GameObject newMask = Instantiate(_maskPrefab, parentPanel);
        Item_Mask maskComponent = newMask.GetComponent<Item_Mask>();
        maskComponent.Initialize(maskSO);
        maskComponent.ToggleDraggable(true);
        maskComponent.ToggleInspectable(false);
        activeMask = newMask;
        return newMask;
    }

    public void TransferMask()
    {
        if(activeGuest != null)
        {
            Guest currentGuest = activeGuest.GetComponent<Guest>();
            Item_Mask maskTransferee = currentGuest.maskPinRect.GetComponentInChildren<Item_Mask>();
            if(maskTransferee != null)
            {
                maskTransferee.transform.SetParent(tablePanel, true);
                maskTransferee.ToggleDraggable(true);
                maskTransferee.ToggleInspectable(false);
                maskTransferee.rt.SetAsLastSibling();

                TeleportUIElement(maskTransferee.gameObject, 0f, 0f);

                if (!activeItems.Contains(maskTransferee.gameObject)) activeItems.Add(maskTransferee.gameObject);
            }
        }
    }

    public void TransferMaskBackToGuest()
    {
        if (activeGuest != null)
        {
            Item_Mask maskOnTable = tablePanel.GetComponentInChildren<Item_Mask>();
            if (maskOnTable != null)
            {
                Guest guestComp = activeGuest.GetComponent<Guest>();
                maskOnTable.rt.SetParent(guestComp.maskPinRect, false);
                maskOnTable.rt.anchoredPosition = Vector2.zero; 
                maskOnTable.ToggleDraggable(false);
                maskOnTable.ToggleInspectable(false);
            }
        }
    }
    #endregion

    #region Guests
    public GameObject SetupGuest(GuestSO guestSO, MaskSO maskSO)
    {
        GameObject newGuest = Instantiate(_guestPrefab, guestsPanel);
        Guest guestComponent = newGuest.GetComponent<Guest>();
        guestComponent.Initialize(guestSO);

        GameObject newMask = SetupMask(maskSO, guestsPanel);
        activeItems.Add(newMask);

        newMask.GetComponent<Item_Mask>().ToggleDraggable(false);
        newMask.GetComponent<Item_Mask>().ToggleInspectable(inspectionMode);

        RectTransform maskRect = newMask.GetComponent<RectTransform>();
        RectTransform maskPinRect = newGuest.GetComponent<Guest>().maskPinRect;

        maskRect.SetParent(maskPinRect, false);
        maskRect.anchorMin = new Vector2(0.5f,0.5f);
        maskRect.anchorMax = new Vector2(0.5f,0.5f);
        maskRect.pivot = new Vector2(0.5f, 0.5f);
        maskRect.anchoredPosition = maskSO.alignmentOffset;

        activeGuest = newGuest;

        // Allow all images (including buttons) to start at 0 alpha so they hop in seamlessly
        foreach (UnityEngine.UI.Image img in newGuest.GetComponentsInChildren<UnityEngine.UI.Image>())
        {
            Color c = img.color;
            c.a = 0f;
            img.color = c;
        }
        
        return newGuest;
    }
    #endregion

    #region Scroll of Rules
    public GameObject SetupSOR(SorSO sorSO, RectTransform parentPanel = null)
    {
        if(parentPanel == null) parentPanel = tablePanel;
        GameObject newSOR = Instantiate(_sorPrefab, parentPanel);
        Item_SOR sorComponent = newSOR.GetComponent<Item_SOR>();
        sorComponent.Initialize(sorSO);
        sorComponent.ToggleDraggable(!inspectionMode);
        sorComponent.ToggleInspectable(inspectionMode);

        activeItems.Add(newSOR);
        return newSOR;
    }

    public GameObject SetupSOR(int index, RectTransform parentPanel = null)
    {
        if(sorSOArray.Length > index)
        {
            SorSO sorSO = sorSOArray[index];
            return SetupSOR(sorSO, parentPanel);
        }
        else
        {
            Debug.LogWarning("Index out of range for sorSOArray in GuestItemManager.");
            return null;
        }
    }
    #endregion

    #region Inspection Logic
    public void SelectAttributeForInspection(AttributeSO[] attributeData, Item sourceItem)
    {
        if (firstSelectedAttributes == null)
        {
            firstSelectedAttributes =  new List<AttributeSO>(attributeData);
            firstSelectedItem = sourceItem;
            return;
        }

        if (sourceItem == firstSelectedItem)
        {
            Debug.LogWarning("You cannot compare an item to itself!");
            ClearInspectionSelection(); 
            return;
        }

        secondSelectedAttributes = new List<AttributeSO>(attributeData);
        secondSelectedItem = sourceItem;
        EvaluateInspection();
    }

    public void SelectAttributeForInspection(AttributeSO attributeData, Item sourceItem)
    {
        SelectAttributeForInspection(new AttributeSO[] { attributeData }, sourceItem);
    }

    private void EvaluateInspection()
    {
        inspectionStatus = InspectionStatus.UNRELATED; 

        foreach(AttributeSO firstAttr in firstSelectedAttributes)
        {
            foreach(AttributeSO secondAttr in secondSelectedAttributes)
            {
                if (firstAttr.category == secondAttr.category)
                {
                    if (firstAttr == secondAttr) inspectionStatus = InspectionStatus.MATCHED;
                    else inspectionStatus = InspectionStatus.MISMATCHED;  
                }
            }
        }
        ClearInspectionSelection();
    }

    public void ClearInspectionSelection()
    {
        firstSelectedAttributes = null;
        secondSelectedAttributes = null;
        firstSelectedItem = null;
        secondSelectedItem = null;
    }
    #endregion

    #region Debugging
    public void SpawnMask()
    {
        if (maskSOArray.Length > 0)
        {
            MaskSO randomMaskSO = maskSOArray[Random.Range(0, maskSOArray.Length)];
            GameObject newMask = SetupMask(randomMaskSO);
            activeItems.Add(newMask);
        }
    }

    public void SpawnGuest()
    {
        GameObject newGuest;
        if (guestSOArray.Length > 0)
        {
            GuestSO randomGuestSO = guestSOArray[Random.Range(0, guestSOArray.Length)];
            MaskSO randomMaskSO = maskSOArray[Random.Range(0, maskSOArray.Length)];
            newGuest = SetupGuest(randomGuestSO, randomMaskSO);
            activeGuest = newGuest;

            TeleportUIElement(newGuest, -500f, 0f);
            StartCoroutine(MoveUIElement(newGuest, 0f, 0f, 3f));
        }
    }
    #endregion

    #region Animations
    public void TeleportUIElement(GameObject teleportee, float xPos, float yPos)
    {
        RectTransform rect = teleportee.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(xPos, yPos);
        teleportUIDoneEvent.Invoke();
    }

    public void TeleportUIElement(GameObject teleportee, RectTransform destinationTarget)
    {
        RectTransform rect = teleportee.GetComponent<RectTransform>();
        rect.anchoredPosition = destinationTarget.anchoredPosition;
        teleportUIDoneEvent.Invoke();
    }

    public System.Collections.IEnumerator MoveUIElement(GameObject movee, float xPos, float yPos, float timeInSeconds)
    {
        float timer = 0f;
        RectTransform moveeRect = movee.GetComponent<RectTransform>();
        Vector2 startPos = moveeRect.anchoredPosition;
        Vector2 targetPos = new(xPos, yPos);

        while(timer < timeInSeconds)
        {
            moveeRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, timer/timeInSeconds);
            timer += Time.deltaTime;
            yield return null;
        }
        moveUIDoneEvent.Invoke();
    }

    public System.Collections.IEnumerator HopUIElement(GameObject movee, float xPos, float yPos, float timeInSeconds, int hops = 4, float hopHeight = 30f)
    {
        float timer = 0f;
        RectTransform moveeRect = movee.GetComponent<RectTransform>();
        Vector2 startPos = moveeRect.anchoredPosition;
        Vector2 targetPos = new Vector2(xPos, yPos);

        while(timer < timeInSeconds)
        {
            float progress = timer / timeInSeconds;
            Vector2 basePos = Vector2.Lerp(startPos, targetPos, progress);
            float hopOffset = Mathf.Abs(Mathf.Sin(progress * Mathf.PI * hops)) * hopHeight;
            moveeRect.anchoredPosition = basePos + new Vector2(0, hopOffset);

            timer += Time.deltaTime;
            yield return null;
        }
        moveeRect.anchoredPosition = targetPos;
        moveUIDoneEvent.Invoke();
    }

    public System.Collections.IEnumerator HopUIElement(GameObject movee, RectTransform destinationTarget, float timeInSeconds, int hops = 4, float hopHeight = 30f)
    {
        float timer = 0f;
        RectTransform moveeRect = movee.GetComponent<RectTransform>();
        Vector2 startPos = moveeRect.anchoredPosition;

        while(timer < timeInSeconds)
        {
            Vector2 targetPos = destinationTarget.anchoredPosition;
            float progress = timer / timeInSeconds;

            Vector2 basePos = Vector2.Lerp(startPos, targetPos, progress);
            float hopOffset = Mathf.Abs(Mathf.Sin(progress * Mathf.PI * hops)) * hopHeight;

            moveeRect.anchoredPosition = basePos + new Vector2(0, hopOffset);

            timer += Time.deltaTime;
            yield return null;
        }
        
        moveeRect.anchoredPosition = destinationTarget.anchoredPosition;
        moveUIDoneEvent.Invoke();
    }

    public System.Collections.IEnumerator ShadowHopUIElement(GameObject movee, RectTransform destinationTarget, float timeInSeconds, int hops = 4, float hopHeight = 30f)
    {
        float timer = 0f;
        RectTransform moveeRect = movee.GetComponent<RectTransform>();
        
        // Let ALL images (including buttons) fade naturally!
        UnityEngine.UI.Image[] allImages = movee.GetComponentsInChildren<UnityEngine.UI.Image>();
        
        Vector2 startPos = moveeRect.anchoredPosition;
        Vector2 targetPos = destinationTarget.anchoredPosition;

        while(timer < timeInSeconds)
        {
            float progress = timer / timeInSeconds;
            
            Vector2 basePos = Vector2.Lerp(startPos, targetPos, progress);
            float hopOffset = Mathf.Abs(Mathf.Sin(progress * Mathf.PI * hops)) * hopHeight;
            moveeRect.anchoredPosition = basePos + new Vector2(0, hopOffset);

            foreach(var img in allImages)
            {
                if (progress < 0.2f) img.color = Color.Lerp(new Color(0, 0, 0, 0), Color.black, progress / 0.2f);
                else 
                {
                    float colorProgress = (progress - 0.2f) / 0.8f;
                    img.color = Color.Lerp(Color.black, Color.white, colorProgress);
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }
        
        moveeRect.anchoredPosition = targetPos;
        foreach(var img in allImages) img.color = Color.white;
        moveUIDoneEvent.Invoke();
    }

    public System.Collections.IEnumerator WalkForwardAndOutRoutine(GameObject movee, float timeInSeconds, int hops = 4, float hopHeight = 30f)
    {
        float timer = 0f;
        RectTransform moveeRect = movee.GetComponent<RectTransform>();
        
        UnityEngine.UI.Image[] allImages = movee.GetComponentsInChildren<UnityEngine.UI.Image>();

        Vector2 startPos = moveeRect.anchoredPosition;
        Vector2 targetPos = startPos + new Vector2(0, -1000f); 
        Vector3 startScale = moveeRect.localScale;
        Vector3 targetScale = startScale * 1.5f;

        while(timer < timeInSeconds)
        {
            float progress = timer / timeInSeconds;
            
            Vector2 basePos = Vector2.Lerp(startPos, targetPos, progress);
            float hopOffset = Mathf.Abs(Mathf.Sin(progress * Mathf.PI * hops)) * hopHeight;
            moveeRect.anchoredPosition = basePos + new Vector2(0, hopOffset);

            moveeRect.localScale = Vector3.Lerp(startScale, targetScale, progress);

            foreach(var img in allImages)
            {
                if (progress < 0.5f) img.color = Color.Lerp(Color.white, Color.black, progress / 0.5f);
                else
                {
                    float alphaProgress = (progress - 0.5f) / 0.5f;
                    img.color = Color.Lerp(Color.black, new Color(0, 0, 0, 0), alphaProgress);
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }
        
        moveeRect.anchoredPosition = targetPos;
        moveUIDoneEvent.Invoke();
    }

    public System.Collections.IEnumerator ScaleAndFadeRoutine(GameObject movee, float timeInSeconds)
    {
        float timer = 0f;
        RectTransform moveeRect = movee.GetComponent<RectTransform>();
        
        UnityEngine.UI.Image[] allImages = movee.GetComponentsInChildren<UnityEngine.UI.Image>();

        Vector3 startScale = moveeRect.localScale;
        Vector3 targetScale = startScale * 1.5f;

        while(timer < timeInSeconds)
        {
            float progress = timer / timeInSeconds;
            moveeRect.localScale = Vector3.Lerp(startScale, targetScale, progress);

            foreach(var img in allImages)
            {
                if (progress < 0.5f) img.color = Color.Lerp(Color.white, Color.black, progress / 0.5f);
                else
                {
                    float alphaProgress = (progress - 0.5f) / 0.5f;
                    img.color = Color.Lerp(Color.black, new Color(0, 0, 0, 0), alphaProgress);
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }
        moveUIDoneEvent.Invoke();
    }
    #endregion
}