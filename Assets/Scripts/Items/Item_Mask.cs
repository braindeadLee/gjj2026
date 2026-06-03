using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Mask : Item
{
    [Header("Attribute Zones")]
    [SerializeField] private Button maskAttributeZone;

    [Header("Data")]
    [SerializeField] [HideInInspector] private MaskSO maskAssigned;

    public override void Awake()
    {
        base.Awake();

        if (maskAttributeZone != null) maskAttributeZone.onClick.AddListener(maskAttributeZoneClicked);
        else Debug.LogWarning("MaskAttributeZone missing on " + gameObject.name + ". Please assign it in the Inspector!");
    }

    public void OnDisable()
    {
        if (maskAttributeZone != null) maskAttributeZone.onClick.RemoveListener(maskAttributeZoneClicked);
    }

    public void Initialize(MaskSO maskSO)
    {
        // --- GUEST FIX APPLIED TO MASKS ---
        // We do NOT call base.Initialize(maskSO) so we can skip SetNativeSize()!
        if (im == null) im = GetComponent<Image>();
        if (rt == null) rt = GetComponent<RectTransform>();

        maskAssigned = maskSO;

        if (maskSO != null && maskSO.itemSprite != null)
        {
            im.sprite = maskSO.itemSprite;
        }

        // --- NEW: Apply your custom Scale Offset from the Scriptable Object! ---
        if (maskSO != null) rt.localScale = maskSO.scaleOffset;
        else rt.localScale = Vector3.one;

        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
    }

    // --- RETAINED FIX: Explicit Override guarantees the exact button is turned on! ---
    public override void ToggleInspectable(bool value)
    {
        if (maskAttributeZone != null) maskAttributeZone.gameObject.SetActive(value);
    }

    [HideInInspector] 
    public void maskAttributeZoneClicked()
    {
        if(maskAssigned != null)
            GuestItemManager.Instance.SelectAttributeForInspection(new AttributeSO[] { maskAssigned.color, maskAssigned.quality, maskAssigned.theme }, this);
        else
            Debug.Log("No mask assigned to this item.");
    }

#if UNITY_EDITOR
    private void Reset()
    {
        maskAttributeZone = transform.Find("MaskAttributeZone")?.GetComponent<Button>();
    }
#endif
}