using System.Collections.Generic;
using UnityEngine.EventSystems;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Item : MonoBehaviour, IDragHandler, IPointerDownHandler, IBeginDragHandler, IEndDragHandler
{
    public GuestItemManager guestItemManager;
    protected Transform tr;
    protected Image im;
    // public List<InspectionZone> inspectionZones;
    public RectTransform _dragRectTransform;
    public Canvas canvas;

    public CanvasGroup canvasGroup;

    private bool canDrag;
    private bool canInspect;
    // public ItemSO itemAssigned;

    public virtual void Awake()
    {
        tr = GetComponent<Transform>();
        im = GetComponent<Image>();
        im.SetNativeSize();

        if (_dragRectTransform == null)
            _dragRectTransform = transform.parent.GetComponent<RectTransform>();
        
        if (canvasGroup == null)
            canvasGroup = _dragRectTransform.GetComponent<CanvasGroup>();
        

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
        if (itemSO != null)
        {
            im.sprite = itemSO.itemSprite;
            im.SetNativeSize();
        }
        RectTransform rt = GetComponent<RectTransform>();
        rt.localScale = Vector3.one;
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
    }
    
    #region Dragging Logic
    public void ToggleDraggable(bool value) => canDrag = value;

    public void ToggleInspectable (bool value)
    {
        foreach (Button button in GetComponentsInChildren<Button>())
        {
                button.enabled = value;
                button.image.enabled = value;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        {
            _dragRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }
        
    public void OnPointerDown(PointerEventData eventData)
    {
        _dragRectTransform.SetAsLastSibling();
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
