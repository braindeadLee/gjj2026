using UnityEngine;

[CreateAssetMenu(fileName = "Guest", menuName = "Scriptable Objects/Guest")]
public class GuestSO : ScriptableObject
{
    public string guestName;
    public Sprite guestSprite;
    public bool isLegit;
    public bool willFight;
    // TextAsset dialogue;
}
