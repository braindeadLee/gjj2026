using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[Serializable] public struct GuestData
{
    public Sprite CharacterSprite;
    public Sprite MaskSprite;
    public bool IsImposter;
    public int specialVoiceline;
}
public class GuestManager : MonoBehaviour
{
    [Header("Guest Data")]
    [SerializeField] public GuestData[] ListOfGuests;

    [Header("Guest Objects")]
    [SerializeField] GameObject Guest;
    [SerializeField] GameObject Character;
    [SerializeField] GameObject Mask;

    float timerIndex = 0;

    public static GuestManager Instance {get; private set;}

    private void Awake()
    {
        if (Instance != null && Instance != this) 
            Destroy(this); 
        else 
            Instance = this; 
    }

    void SetGuest(Sprite CharacterSprite, Sprite MaskSprite)
    {
        Character.GetComponent<SpriteRenderer>().sprite = CharacterSprite;
        Mask.GetComponent<SpriteRenderer>().sprite = MaskSprite;
        
    }

    void EnterGuest(Vector2 startingPosition, Vector2 endPosition, float transitionTime)
    {
        Guest.transform.position = startingPosition;
        StartCoroutine(GuestIsWalking(startingPosition, endPosition, transitionTime));
    }

    public IEnumerator GuestIsWalking(Vector2 startingPosition, Vector2 endPosition, float transitionTime)
    {
        timerIndex = 0;

        while(timerIndex < transitionTime)
        {
            Guest.transform.position = Vector2.Lerp(startingPosition, endPosition, timerIndex / transitionTime);

            timerIndex += Time.deltaTime;
            
            yield return null;
        }
    }
}
