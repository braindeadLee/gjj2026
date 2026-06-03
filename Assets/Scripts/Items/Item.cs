using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Item : MonoBehaviour, IDragHandler, IPointerDownHandler, IBeginDragHandler, IEndDragHandler
{
    [HideInInspector] public GuestItemManager guestItemManager;
    protected Transform tr;
    protected Image im;
    [HideInInspector] public RectTransform rt;
    [HideInInspector] public Canvas canvas;
    [HideInInspector] public CanvasGroup canvasGroup;

    private bool canDrag = false;

    public virtual void Awake()
    {
        tr = GetComponent<Transform>();
        im = GetComponent<Image>();
        rt = GetComponent<RectTransform>();
        
        if (canvasGroup == null)
            canvasGroup = rt.GetComponent<CanvasGroup>();

        if (canvas == null)
        {
            Transform testCanvasTransform = transform.parent;
            while (testCanvasTransform != null)
            {
                canvas = testCanvasTransform.GetComponent<Canvas>();
                if (canvas != null) break;
                testCanvasTransform = testCanvasTransform.parent;
            }
        }
    }

    public virtual void Start()
    {
        if (guestItemManager == null)
        {
            guestItemManager = GuestItemManager.Instance;
        }
    }

    public virtual void Initialize(ItemSO itemSO)
    {
        if (im == null) im = GetComponent<Image>();
        if (rt == null) rt = GetComponent<RectTransform>();

        if (itemSO != null && itemSO.itemSprite != null)
        {
            im.sprite = itemSO.itemSprite;
            im.SetNativeSize(); 
        }

        rt.localScale = Vector3.one;
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
    }
    
    #region Dragging & Inspection Logic
    public void ToggleDraggable(bool value) => canDrag = value;

    // NEW: Made Virtual! Sub-classes will override this for guaranteed accuracy.
    public virtual void ToggleInspectable (bool value)
    {
        foreach (Button button in GetComponentsInChildren<Button>(true))
        {
            button.gameObject.SetActive(value);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        rt.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
        
    public void OnPointerDown(PointerEventData eventData)
    {
        rt.SetAsLastSibling();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!canDrag) return;
        canvasGroup.alpha = 0.8f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(!canDrag) return;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true; 
    }
    #endregion
}