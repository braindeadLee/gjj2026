using UnityEngine;

[CreateAssetMenu(fileName = "New Conversation", menuName = "Story/Conversation")]
public class ConversationSO : ScriptableObject
{
    public DialogueSO entryDialogue;
    public DialogueSO acceptDialogue;
    public DialogueSO rejectValidDialogue;
    public DialogueSO rejectInvalidDialogue;
}