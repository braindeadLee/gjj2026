using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class Guest : MonoBehaviour
{
    Item_Mask mask;
    public GuestSO guestAssigned;
    RectTransform tr;
    Image im;

    public RectTransform maskPinRect;

    private void Awake()
    {
        tr = GetComponent<RectTransform>();
        im = GetComponent<Image>();
    }

    public void Initialize(GuestSO guestSO)
    {
        // --- SURVIVAL PATCH ---
        // If spawned into a disabled parent, Awake() is skipped. Force the fetch!
        if (im == null) im = GetComponent<Image>();
        if (tr == null) tr = GetComponent<RectTransform>();

        guestAssigned = guestSO;

        if (guestSO != null && guestSO.guestSprite != null)
        {
            im.sprite = guestSO.guestSprite;
            // DISABLED: Do not let raw PNG files override the Prefab's set size!
            // im.SetNativeSize(); 
        }
        
        tr.localScale = Vector3.one;
        tr.anchorMin = new Vector2(0.5f, 0.5f);
        tr.anchorMax = new Vector2(0.5f, 0.5f);
        tr.pivot = new Vector2(0.5f, 0.5f);

        Transform maskPin = transform.Find("MaskPin");
        if(maskPin != null)
        {
            maskPinRect = maskPin.GetComponent<RectTransform>();
        }
        else
        {
            maskPinRect = tr;
            Debug.Log("Can't find designated Mask Pin, defaulting to parent object for mask pinning");
        }
    }
}