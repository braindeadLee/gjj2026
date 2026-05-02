using System.Collections.Generic;
using TMPro;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GuestItemManager : MonoBehaviour
{
    public static GuestItemManager Instance;
    [SerializeField] private Canvas canvas;
    public bool inspectionMode = false;
    
    //all items, regardless of sub-type, must obey rules of Inspection/Dragging mode
    #region Items
    private List<GameObject> activeItems = new();
    [SerializeField] private RectTransform itemsCanvas;
    #endregion

    #region Masks
    [Header("Masks")]
    [SerializeField] private GameObject _maskPrefab;
    [SerializeField] private MaskSO[] maskSOArray;
    [SerializeField] private RectTransform tablePanel;

    //legacy, need to rework or delete
    // private List<GameObject> activeMasks = new();
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

    #endregion

    #region Inspection
    private AttributeSO currentInspectionAttributeA;
    private AttributeSO currentInspectionAttributeB;
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

    //All "animations" should notify when they're done via these events, so that we can chain them together and/or trigger other actions when they finish
    public UnityEvent teleportUIDoneEvent;
    public UnityEvent moveUIDoneEvent;
    public UnityEvent scaleUIDoneEvent;
    public UnityEvent colorUIDoneEvent;

    #endregion


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        inspectionMode = false;

        if (debugMode)
        {
            debugPanel.gameObject.SetActive(true);
            debugInspectionModeText.text = "Inspection Mode: " + (inspectionMode ? "ON" : "OFF");
        }
        else
        {
            debugPanel.gameObject.SetActive(false);
        }
        ToggleInspectionMode(false);
    }

    private void OnEnable()
    {
        if (debugToggleInspectionButton != null)
            debugToggleInspectionButton.onClick.AddListener(ToggleInspectionMode);
        if (debugSpawnMaskButton != null)
            debugSpawnMaskButton.onClick.AddListener(SpawnMask);
        if (debugSpawnGuestButton != null)
            debugSpawnGuestButton.onClick.AddListener(SpawnGuest);
        if (debugTransferMaskButton != null)        
            debugTransferMaskButton.onClick.AddListener(TransferMask);
    }
    private void OnDisable()
    {
        if (debugToggleInspectionButton != null)
            debugToggleInspectionButton.onClick.RemoveListener(ToggleInspectionMode);
        if (debugSpawnMaskButton != null)
            debugSpawnMaskButton.onClick.RemoveListener(SpawnMask);
        if (debugSpawnGuestButton != null)
            debugSpawnGuestButton.onClick.RemoveListener(SpawnGuest);
        if (debugTransferMaskButton != null)
            debugTransferMaskButton.onClick.RemoveListener(TransferMask);
    }
    #region Items
    //Enforce rules of inspection/dragging mode on all items, called whenever those modes are toggled
    private void ToggleInspectionMode(bool value)
    {
        inspectionMode = value;
        debugInspectionModeText.text = "Inspection Mode: " + (inspectionMode ? "ON" : "OFF");

        foreach (GameObject item in activeItems)
        {
            if (item.GetComponent<RectTransform>().IsChildOf(itemsCanvas))
            {
                item.GetComponent<Item>().ToggleInspectable(value);
            }
        }

        foreach (GameObject item in activeItems)
        {
            if (item.GetComponent<RectTransform>().IsChildOf(tablePanel))
            {
                item.GetComponent<Item>().ToggleDraggable(!value);
            }
        }
    }

    private void ToggleInspectionMode()
    {
        ToggleInspectionMode(!inspectionMode);
    }

    #endregion

    #region Masks
        public GameObject SetupMask(MaskSO maskSO, RectTransform parentPanel = null){
            
            if(parentPanel == null) parentPanel = tablePanel;
        
            GameObject newMask = Instantiate(_maskPrefab, parentPanel);
            Item_Mask maskComponent = newMask.GetComponent<Item_Mask>();
            maskComponent.Initialize(maskSO);
            //change later
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

            }
        }
    }

    #endregion

    #region Guests

    public GameObject SetupGuest(GuestSO guestSO, MaskSO maskSO){
        GameObject newGuest = Instantiate(_guestPrefab, guestsPanel);

        Guest guestComponent = newGuest.GetComponent<Guest>();
        guestComponent.Initialize(guestSO);

        GameObject newMask = SetupMask(maskSO, guestsPanel);
        activeItems.Add(newMask);

        newMask.GetComponent<Item_Mask>().ToggleDraggable(false);
        newMask.GetComponent<Item_Mask>().ToggleInspectable(inspectionMode);

        RectTransform maskRect = newMask.GetComponent<RectTransform>();
        RectTransform maskPinRect = newGuest.GetComponent<Guest>().maskPinRect;

            if (maskPinRect == null)
                Debug.Log("no maskPinRect");

            maskRect.SetParent(maskPinRect, false);
            maskRect.anchorMin = new Vector2(0.5f,0.5f);
            maskRect.anchorMax = new Vector2(0.5f,0.5f);
            maskRect.pivot = new Vector2(0.5f, 0.5f);

            maskRect.anchoredPosition = maskSO.alignmentOffset;

        activeGuest = newGuest;
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

    #region Debugging

    public void SpawnMask()
    //Probably good idea to implement input parameters via text box or something
    {
        if (maskSOArray.Length > 0)
        {
            MaskSO randomMaskSO = maskSOArray[Random.Range(0, maskSOArray.Length)];
            GameObject newMask = SetupMask(randomMaskSO);
            activeItems.Add(newMask);
        }
        else
        {
            Debug.LogWarning("No MaskSOs assigned to GuestItemManager.");
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
        else
        {
            Debug.LogWarning("No GuestSOs assigned to GuestItemManager.");
        }
    }
    #endregion

    #region animations
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
    /// Moves a UI element to a destination position over a specified time period.
    //function assumes both destinationTarget and movee.GetComponent<RectTransform>() are using the same coordinate space (e.g. both are children of the same canvas)
    public System.Collections.IEnumerator MoveUIElement(GameObject movee, RectTransform destinationTarget, float timeInSeconds)
    {
        float timer = 0f;
        RectTransform moveeRect = movee.GetComponent<RectTransform>();
        Vector2 startPos = moveeRect.anchoredPosition;

        while(timer < timeInSeconds)
        {
            Vector2 targetPos = destinationTarget.anchoredPosition;

            moveeRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, timer/timeInSeconds);

            timer += Time.deltaTime;
            yield return null;
        }
        moveUIDoneEvent.Invoke();
    }

    public System.Collections.IEnumerator ScaleUIElement(GameObject scalee, Vector3 endScale, float timeInSeconds)
    {
        float timer = 0f;
        RectTransform rect = scalee.GetComponent<RectTransform>();
        Vector3 startScale = rect.anchoredPosition;

        while(timer < timeInSeconds)
        {
            rect.localScale = Vector3.Lerp(startScale, endScale, timer/timeInSeconds);

            timer += Time.deltaTime;
            yield return null;
        }

        scaleUIDoneEvent.Invoke();
    }

    public System.Collections.IEnumerator ColorUIElement(GameObject coloree, Color endColor, float timeInSeconds)
    {
        float timer = 0f;
        Image im = coloree.GetComponent<Image>();
        Color startColor = im.color;

        while(timer < timeInSeconds)
        {
            im.color = Color.Lerp(startColor, endColor, timer/timeInSeconds);

            timer += Time.deltaTime;
            yield return null;
        }

        colorUIDoneEvent.Invoke();
    }



    #endregion
}
