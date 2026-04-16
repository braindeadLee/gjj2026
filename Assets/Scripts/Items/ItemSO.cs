using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public ItemType itemType;
    public Sprite itemSprite;
    public Vector2 alignmentOffset;
    public List<AttributeSO> attributes;
}
