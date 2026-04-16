using System.Collections.Generic;
using UnityEngine.EventSystems;

using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    
    private Transform tr;
    private Image im;

    public List<InspectionZone> inspectionZones;
    public bool isDraggable;
    public RectTransform _dragRectTransform;
    public Canvas canvas;
    public ItemSO itemAssigned;

    public virtual void Awake()
    {
        tr = GetComponent<Transform>();
        im = GetComponent<Image>();

        if (_dragRectTransform == null)
        {
            _dragRectTransform = transform.parent.GetComponent<RectTransform>();
        }

        if (canvas == null)
        {
            Transform testCanvasTransform = transform.parent;
            while (testCanvasTransform != null)
            {
                canvas = testCanvasTransform.GetComponent<Canvas>();
                if (canvas != null)
                {
                    break;
                }
                testCanvasTransform = testCanvasTransform.parent;
            }
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (isDraggable)
        {
            _dragRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
            //eventData.delta is the movement of the mouse since the last event, so we add it to the current position of the RectTransform to move it accordingly.
        }
    }
        

    public void OnPointerDown(PointerEventData eventData)
    {
        _dragRectTransform.SetAsLastSibling();
        //This line ensures that the dragged window is rendered on top of other UI elements by moving it to the end of the sibling hierarchy in the UI canvas.
    }


    public virtual void Start()
    {
        im.sprite = itemAssigned.itemSprite;
        
    }
}
