using UnityEngine;
using System.Collections.Generic;
using System;


public class Manager : MonoBehaviour
{
    UIManager uiManager;
    GuestManager guestManager;

    [Header("Winning Conditions")]
    private List<int> guestLoadOrder;
    private int listLength;
    private int listCounter = 0;
    
    private int evilEntered = 0;

    private int goodKickedOut = 0;

    private float timeToDecide = 10f;

    [SerializeField] private int evilThreshhold = 1;

    [SerializeField] private int goodThreshhold = 3;
    private GuestData[] listOfGuests;

    [Header("Misc")]

    [SerializeField] private GameObject npcStartPosition;
    [SerializeField] private GameObject npcEndPosition;

public static Manager Instance {get; private set;}

    private void Awake()
    {
        if (Instance != null && Instance != this) 
            Destroy(this); 
        else 
            Instance = this; 
    }
    void Start()
    {
        uiManager = UIManager.Instance;
        guestManager = GuestManager.Instance;

        listOfGuests = guestManager.ListOfGuests;

        uiManager.introFinished.AddListener(GameplayLoop);

        StartCoroutine(uiManager.StartingMenu());
    }

    public void StartGameplay()
    {
        SetGuestListOrder(guestManager.ListOfGuests.Length);
        listCounter = 0;
        listLength = listOfGuests.Length;
    }

    private void SetGuestListOrder(int guestNum)
    {
        guestLoadOrder = new List<int>();
        for(int i = 0; i < guestNum; i++)
        {
            guestLoadOrder.Add(i);
        }

        Debug.Log("Original order: " + string.Join(", ", guestLoadOrder));
        guestLoadOrder.Shuffle();
        Debug.Log("Randomized order: " + string.Join(", ", guestLoadOrder));
    }

    private void GameplayLoop()
    {
        if(listCounter < listLength)
        {
            NextGuest(listCounter);
        }
    }
    private void NextGuest(int guestNum)
    {
        Debug.Log("Guest Number: " + listCounter);
        guestManager.SetGuest(listOfGuests[guestNum].CharacterSprite, listOfGuests[guestNum].MaskSprite);
        guestManager.EnterGuest(npcStartPosition.transform.position, npcEndPosition.transform.position, 1.5f);

    }

    System.Collections.IEnumerator Decisions(float prepTime)
    {
        yield return new WaitForSeconds(prepTime);

        uiManager.showDecideButton();


    }
}
