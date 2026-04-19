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
        guestAssigned = guestSO;
        im.sprite = guestAssigned.guestSprite;

        if (guestSO != null)
        {
            im.sprite = guestSO.guestSprite;
            im.SetNativeSize();
        }
        RectTransform rt = GetComponent<RectTransform>();
        rt.localScale = Vector3.one;
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);

        Transform maskPin = transform.Find("MaskPin");
        if(maskPin != null)
        {
            maskPinRect = maskPin.GetComponent<RectTransform>();
        }
        else
        {
            maskPinRect = this.GetComponent<RectTransform>();
            Debug.Log("Can't find designated Mask Pin, defaulting to parent object for mask pinning");
        }
    }
}
