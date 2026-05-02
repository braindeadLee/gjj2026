using UnityEngine;
using UnityEngine.UI;

public class Item_Mask : Item
{
    [Header("Attribute Zones")]
    [SerializeField] private Button maskAttributeZone;

    [Header("Data")]
    [SerializeField] [HideInInspector] private MaskSO maskAssigned;

    private bool isAttachedToGuest;

    public override void Awake()
    {
        base.Awake();

        if (maskAttributeZone != null)
        {
            maskAttributeZone.onClick.AddListener(maskAttributeZoneClicked);
        } 
        else
        {
            Debug.LogWarning("MaskAttributeZone missing on " + gameObject.name + ". Please assign it in the Inspector!");
        }
    }

    public override void Start()
    {
        base.Start();
    }

    public void OnDisable()
    {
        if (maskAttributeZone != null)
        {
            maskAttributeZone.onClick.RemoveListener(maskAttributeZoneClicked);
        }
    }

    public void Initialize(MaskSO maskSO)
    {
        base.Initialize(maskSO);
        maskAssigned = maskSO;
    }
[HideInInspector] 
    public void maskAttributeZoneClicked()
    {
        if(maskAssigned != null)
        {
            Debug.Log($"Mask's color: " + maskAssigned.color.displayName);
            Debug.Log($"Mask's quality: " + maskAssigned.quality.displayName);
            Debug.Log($"Mask's theme: " + maskAssigned.theme.displayName);
        } 
        else
        {
            Debug.Log("No mask assigned to this item.");
        }
    }

#if UNITY_EDITOR
    private void Reset()
    {
        //auto-fills designated button slot if name match
        maskAttributeZone = transform.Find("MaskAttributeZone")?.GetComponent<Button>();
    }
#endif
}