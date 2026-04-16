using UnityEngine;
using UnityEngine.UI;

public class Mask : Item
{
    private Button maskAttributeZone;

    public override void Awake()
    {
        base.Awake();

        maskAttributeZone = transform.Find("MaskAttributeZone").GetComponent<Button>();
    }

    public override void Start() => base.Start();
    
    public void maskAttributeZoneClicked()
    {
        // Handle the click event for the mask attribute zone
        Debug.Log("Mask attribute zone clicked!");
    }
}