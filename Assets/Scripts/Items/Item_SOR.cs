using UnityEngine;
using UnityEngine.UI;

public class Item_SOR : Item
{
    [Header("Attribute Zones")]
    [SerializeField] private Button colorAttributeZone;
    [SerializeField] private Button qualityAttributeZone;
    [SerializeField] private Button themeAttributeZone;
    
    [Header("Data")]
    [SerializeField] [HideInInspector] private SorSO scrollAssigned;

    public override void Awake()
    {
        base.Awake();

        if (colorAttributeZone != null) colorAttributeZone.onClick.AddListener(ColorAttributeZoneClicked);
        else Debug.LogWarning("ColorAttributeZone missing on " + gameObject.name);

        if (qualityAttributeZone != null) qualityAttributeZone.onClick.AddListener(QualityAttributeZoneClicked);
        else Debug.LogWarning("QualityAttributeZone missing on " + gameObject.name);

        if (themeAttributeZone != null) themeAttributeZone.onClick.AddListener(ThemeAttributeZoneClicked);
        else Debug.LogWarning("ThemeAttributeZone missing on " + gameObject.name);
    }

    public void OnDisable()
    {
        if (colorAttributeZone != null) colorAttributeZone.onClick.RemoveListener(ColorAttributeZoneClicked);
        if (qualityAttributeZone != null) qualityAttributeZone.onClick.RemoveListener(QualityAttributeZoneClicked);
        if (themeAttributeZone != null) themeAttributeZone.onClick.RemoveListener(ThemeAttributeZoneClicked);
    }

    public void Initialize(SorSO scrollSO)
    {
        base.Initialize(scrollSO);
        scrollAssigned = scrollSO;
    }

    // --- NEW: Explicit Override guarantees the exact buttons are turned on! ---
    public override void ToggleInspectable(bool value)
    {
        if (colorAttributeZone != null) colorAttributeZone.gameObject.SetActive(value);
        if (qualityAttributeZone != null) qualityAttributeZone.gameObject.SetActive(value);
        if (themeAttributeZone != null) themeAttributeZone.gameObject.SetActive(value);
    }

    public void ColorAttributeZoneClicked()
    {
        if(scrollAssigned != null) GuestItemManager.Instance.SelectAttributeForInspection(scrollAssigned.color, this);
        else Debug.Log("No color rule assigned to this scroll.");
    }

    public void QualityAttributeZoneClicked()
    {
        if(scrollAssigned != null) GuestItemManager.Instance.SelectAttributeForInspection(scrollAssigned.quality, this);
        else Debug.Log("No quality rule assigned to this scroll.");
    }

    public void ThemeAttributeZoneClicked()
    {
        if(scrollAssigned != null) GuestItemManager.Instance.SelectAttributeForInspection(scrollAssigned.theme, this);
        else Debug.Log("No theme rule assigned to this scroll.");
    }

    private void Reset()
    {
        colorAttributeZone = transform.Find("ColorAttributeZone")?.GetComponent<Button>();
        qualityAttributeZone = transform.Find("QualityAttributeZone")?.GetComponent<Button>();
        themeAttributeZone = transform.Find("ThemeAttributeZone")?.GetComponent<Button>();
    }
}