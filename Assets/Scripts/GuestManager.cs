using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;

[Serializable] public struct GuestData
{
    public Sprite CharacterSprite;
    public Sprite MaskSprite;
    public bool IsImposter;
    public int specialVoiceline;
    public string title;
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
    private Vector3 startingPosition;
    private Quaternion startingRotation;
    private Vector3 startingScale;

    private Color startingColor;
    private SpriteRenderer characterSpriteRenderer;
    private SpriteRenderer maskSpriteRenderer;

    [HideInInspector] public UnityEvent guestExited;

    public static GuestManager Instance {get; private set;}

    private void Awake()
    {
        if (Instance != null && Instance != this) 
            Destroy(this); 
        else 
            Instance = this; 
    }

    private void Start()
    {
        startingPosition = Guest.transform.position;
        startingRotation = Guest.transform.rotation;
        startingScale = Guest.transform.localScale;
        startingColor = Character.GetComponent<SpriteRenderer>().color;

        characterSpriteRenderer = Character.GetComponent<SpriteRenderer>();
        maskSpriteRenderer = Mask.GetComponent<SpriteRenderer>();
    }

    public void SetGuest(Sprite CharacterSprite, Sprite MaskSprite)
    {
        Guest.transform.position = startingPosition;
        Guest.transform.rotation = startingRotation;
        Guest.transform.localScale = startingScale;

        maskSpriteRenderer.color = startingColor;
        characterSpriteRenderer.color = startingColor;

        Character.GetComponent<SpriteRenderer>().sprite = CharacterSprite;
        Mask.GetComponent<SpriteRenderer>().sprite = MaskSprite;
    }

    public void EnterGuest(Vector2 startingPosition, Vector2 endPosition, float transitionTime)
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

    public IEnumerator LetInGuest(float enterTime)
    {
        float enterTimer = 0f;

        Vector3 finishedScale = new Vector3(3.5f,3.5f,3.5f);

        Color finishedOpacity = Color.clear;

        while (enterTimer < enterTime)
        {
            float t = enterTimer / enterTime;

            float tSquared = t * t;
            Guest.transform.localScale = Vector3.Lerp(startingScale, finishedScale, tSquared);
            maskSpriteRenderer.color = Color.Lerp(startingColor, finishedOpacity, t);
            characterSpriteRenderer.color = Color.Lerp(startingColor, finishedOpacity, t);

            enterTimer += Time.deltaTime;
            yield return null;

            guestExited?.Invoke();
        }
    }

    public IEnumerator KickOutGuest(float exitTime)
    {
        float exitTimer = 0f;

        Vector3 finishedScale = new Vector3(0.01f, 0.01f, 0.01f);
        Color finishedOpacity = Color.clear;
        Quaternion finishedRot = Quaternion.Euler(0,0,-150);

        while(exitTimer < exitTime)
        {
            float t = exitTimer / exitTime;

            Guest.transform.localScale = Vector3.Lerp(startingScale, finishedScale, t/2);
            Guest.transform.rotation = Quaternion.Lerp(startingRotation, finishedRot, t);
            maskSpriteRenderer.color = Color.Lerp(startingColor, finishedOpacity, t);
            characterSpriteRenderer.color = Color.Lerp(startingColor, finishedOpacity, t);

            exitTimer += Time.deltaTime;
            yield return null;

            guestExited?.Invoke();

            exitTimer += Time.deltaTime;
            yield return null;
        }
    }
}
