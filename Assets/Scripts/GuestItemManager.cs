using System.Collections.Generic;
using TMPro;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GuestItemManager : MonoBehaviour
{
    public static GuestItemManager Instance;
    [SerializeField] private Canvas canvas;
    public bool inspectionMode = false;

    #region Masks
    [Header("Masks")]
    [SerializeField] private GameObject _maskPrefab;
    [SerializeField] private MaskSO[] maskSOArray;
    [SerializeField] private RectTransform tablePanel;

    private List<GameObject> activeMasks = new List<GameObject>();
    #endregion

    #region Guests
    [Header("Guests")]
    [SerializeField] private GameObject _guestPrefab;
    [SerializeField] private GuestSO[] guestSOArray;
    [SerializeField] private RectTransform guestsPanel;

    // private List<GameObject> activeGuests = new List<GameObject>();
    private GameObject activeGuest;
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

    #region Masks
        public GameObject SetupMask(MaskSO maskSO, RectTransform parentPanel = null){
            if(parentPanel == null)
        {
            parentPanel = tablePanel;
        }
            GameObject newMask = Instantiate(_maskPrefab, parentPanel);
            Item_Mask maskComponent = newMask.GetComponent<Item_Mask>();
            maskComponent.Initialize(maskSO);
            //change later
            maskComponent.ToggleDraggable(true);
            maskComponent.ToggleInspectable(false);
            return newMask;
        }

    #endregion

    #region Guests

        public GameObject SetupGuest(GuestSO guestSO, MaskSO maskSO){
            GameObject newGuest = Instantiate(_guestPrefab, guestsPanel);

            Guest guestComponent = newGuest.GetComponent<Guest>();
            guestComponent.Initialize(guestSO);

            GameObject newMask = SetupMask(maskSO, guestsPanel);
            newMask.GetComponent<Item_Mask>().ToggleDraggable(false);
            newMask.GetComponent<Item_Mask>().ToggleInspectable(false);

            RectTransform maskRect = newMask.GetComponent<RectTransform>();
            RectTransform maskPinRect = newGuest.GetComponent<Guest>().maskPinRect;

                if (maskPinRect == null)
                    Debug.Log("no maskPinRect");

                maskRect.SetParent(maskPinRect, false);
                maskRect.anchorMin = new Vector2(0.5f,0.5f);
                maskRect.anchorMax = new Vector2(0.5f,0.5f);
                maskRect.pivot = new Vector2(0.5f, 0.5f);

                maskRect.anchoredPosition = maskSO.alignmentOffset;
            
            return newGuest;
        }

    #endregion
    #region Debugging
    private void ToggleInspectionMode(bool value)
    {
        inspectionMode = value;
        debugInspectionModeText.text = "Inspection Mode: " + (inspectionMode ? "ON" : "OFF");

        foreach (GameObject mask in activeMasks)
        {
            if (mask.GetComponent<RectTransform>().IsChildOf(tablePanel))
            {
                mask.GetComponent<Item_Mask>().ToggleDraggable(!value);
                mask.GetComponent<Item_Mask>().ToggleInspectable(value);
            }
        }
    }

    private void ToggleInspectionMode()
    {
        ToggleInspectionMode(!inspectionMode);
    }

    public void SpawnMask()
    //Probably good idea to implement input parameters via text box or something
    {
        if (maskSOArray.Length > 0)
        {
            MaskSO randomMaskSO = maskSOArray[Random.Range(0, maskSOArray.Length)];
            GameObject newMask = SetupMask(randomMaskSO);
            activeMasks.Add(newMask);
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
                activeMasks.Add(maskTransferee.GameObject());

                TeleportUIElement(maskTransferee.gameObject, 0f, 0f);

            }
        }
    }
    #endregion

    #region animations
    public void TeleportUIElement(GameObject teleportee, float xPos, float yPos)
    {
        RectTransform rect = teleportee.GetComponent<RectTransform>();

        rect.anchoredPosition = new Vector2(xPos, yPos);
    }

    public System.Collections.IEnumerator MoveUIElement(GameObject movee, float xPos, float yPos, float timeInSeconds)
    {
        float timer = 0f;
        RectTransform rect = movee.GetComponent<RectTransform>();
        Vector2 startPos = rect.anchoredPosition;
        Vector2 targetPos = new Vector2(xPos, yPos);

        while(timer < timeInSeconds)
        {
            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, timer/timeInSeconds);

            timer += Time.deltaTime;
            yield return null;
        }
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
    }

    #endregion
}
