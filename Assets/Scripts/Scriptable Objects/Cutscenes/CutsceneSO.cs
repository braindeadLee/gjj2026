using UnityEngine;

[CreateAssetMenu(fileName = "New Cutscene", menuName = "Story/Cutscene")]
public class CutsceneSO : ScriptableObject
{
    public Sprite[] sequenceImages;
    public int DayToShow;
}