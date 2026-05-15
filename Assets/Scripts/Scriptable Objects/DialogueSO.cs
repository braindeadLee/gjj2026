using UnityEngine;

[System.Serializable]
public struct DialogueLine
{
    public bool isPlayerTalking;
    [TextArea(3, 5)] public string text;
}

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Story/Dialogue")]
public class DialogueSO : ScriptableObject
{
    public DialogueLine[] lines;
}