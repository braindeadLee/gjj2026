using UnityEngine;

[CreateAssetMenu(fileName = "MaskSO", menuName = "Scriptable Objects/MaskSO")]
public class MaskSO : ItemSO
{
    public AttributeSO color;
    public AttributeSO quality;
    public AttributeSO theme;

    [Header("Transform Adjustments")]
    [Tooltip("Manual scale adjustment for this specific mask. Default is 1, 1, 1.")]
    public Vector3 scaleOffset = Vector3.one;
}