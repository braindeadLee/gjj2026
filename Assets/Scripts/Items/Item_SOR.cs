using UnityEngine;
using UnityEngine.UI;

//SOR = Scroll of Rules
public class Item_SOR : Item
{
    [Header("Attribute Zones")]
    [SerializeField] private Button colorAttributeZone;
    [SerializeField] private Button qualityAttributeZone;
    [SerializeField] private Button themeAttributeZone;
    
    [Header("Data")]
    [SerializeField] [HideInInspector] private SorSO scrollAssigned;

    private bool isAttachedToGuest;

    public override void Awake()
    {
        base.Awake();

        if (colorAttributeZone != null)
            colorAttributeZone.onClick.AddListener(ColorAttributeZoneClicked);
        else
            Debug.LogWarning("ColorAttributeZone missing on " + gameObject.name);

        if (qualityAttributeZone != null)
            qualityAttributeZone.onClick.AddListener(QualityAttributeZoneClicked);
        else
            Debug.LogWarning("QualityAttributeZone missing on " + gameObject.name);

        if (themeAttributeZone != null)
            themeAttributeZone.onClick.AddListener(ThemeAttributeZoneClicked);
        else
            Debug.LogWarning("ThemeAttributeZone missing on " + gameObject.name);
    }

    public override void Start()
    {
        base.Start();
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

    public void ColorAttributeZoneClicked()
    {
        if(scrollAssigned != null)

            Debug.Log($"Scroll's color: " + scrollAssigned.color[0].displayName);
        else
            Debug.Log("No color rule assigned to this scroll.");
    }

    public void QualityAttributeZoneClicked()
    {
        if(scrollAssigned != null)
            Debug.Log($"Scroll's quality: " + scrollAssigned.quality[0].displayName);
        else
            Debug.Log("No quality rule assigned to this scroll.");
    }

    public void ThemeAttributeZoneClicked()
    {
        if(scrollAssigned != null)
            Debug.Log($"Scroll's theme: " + scrollAssigned.theme[0].displayName);
        else
            Debug.Log("No theme rule assigned to this scroll.");
    }
    private void Reset()
    {
        //auto-fills designated button slot if name match
        colorAttributeZone = transform.Find("ColorAttributeZone")?.GetComponent<Button>();
        qualityAttributeZone = transform.Find("QualityAttributeZone")?.GetComponent<Button>();
        themeAttributeZone = transform.Find("ThemeAttributeZone")?.GetComponent<Button>();
    }
}