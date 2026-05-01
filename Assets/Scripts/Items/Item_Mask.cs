using UnityEngine;
using UnityEngine.UI;

public class Item_Mask : Item
{
    private Button maskAttributeZone;
    [SerializeField] private MaskSO maskAssigned;

    private bool isAttachedToGuest;

    public override void Awake()
    {
        base.Awake();

        maskAttributeZone = transform.Find("MaskAttributeZone").GetComponent<Button>();

        if (maskAttributeZone != null)
        {
            maskAttributeZone.enabled = true;
            maskAttributeZone.onClick.AddListener(maskAttributeZoneClicked);
        } else
        {
            Debug.LogWarning("MaskAttributeZone button not found on " + gameObject.name);
        }
    }

    public override void Start()
    {
        base.Start();
    }

    public void OnDisable()
    {
        maskAttributeZone.onClick.RemoveListener(maskAttributeZoneClicked);
    }

    public void Initialize(MaskSO maskSO)
    {
        base.Initialize(maskSO);
        maskAssigned = maskSO;
    }

    public void maskAttributeZoneClicked()
    {
        if(maskAssigned != null)
        {
            Debug.Log($"Mask's color: " + maskAssigned.color.displayName);
            Debug.Log($"Mask's quality: " + maskAssigned.quality.displayName);
            Debug.Log($"Mask's theme: " + maskAssigned.theme.displayName);
        } else
        {
            Debug.Log("No mask assigned to this item.");
        }
    }
}