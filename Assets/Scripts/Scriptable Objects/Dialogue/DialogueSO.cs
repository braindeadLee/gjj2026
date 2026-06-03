using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Story/Dialogue")]
public class DialogueSO : ScriptableObject
{
    [TextArea(10, 20)]
    [Tooltip("Use -P- for Player, -N- for NPC. Separate lines with Enter.")]
    public string conversationText;
}