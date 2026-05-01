using UnityEngine;

public enum ItemType{
    MASK,
    RULE,
    LETTER
}

//Add more as they come
public enum RuleType{
    COLOR,
    QUALITY,
    THEME,
    INSIGNIA
}

public enum CursorMode{
    NORMAL,
    INSPECTING
}

// [System.Serializable]
// public struct ItemAttribute{
//     public RuleType ruleType;
//     public string actualValue;
// }

[System.Serializable]
public struct InspectionZone{
    public BoxCollider2D collider;
    public AttributeSO attributeValue;
}

[System.Serializable]
public struct GuestMaskPair
{
    public MaskSO mask;
    public GuestSO guest;
}