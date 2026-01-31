using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor.Build;


public class Manager : MonoBehaviour
{
    UIManager uiManager;
    GuestManager guestManager;

    [Header("Winning Conditions")]
    private List<int> guestLoadOrder;
    private int listLength;
    private int listCounter = 0;
    

    [Header("Game Rules")]
    [SerializeField] private float timeToDecide = 10f;
    private float timerToDecide = 0f;
    private bool decisionMade = false;

    [SerializeField] private int evilThreshhold = 1;
    private int evilCounter = 0;

    [SerializeField] private int goodThreshhold = 3;
    private int goodCounter = 0;
    private GuestData[] listOfGuests;

    [Header("Misc")]

    [SerializeField] private GameObject npcStartPosition;
    [SerializeField] private GameObject npcEndPosition;
    [SerializeField] private GameObject npcBlastedPosition;

    private int currentGuestNum = 0;


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
        uiManager.endingFinished.AddListener(StartGameplay);

        StartGameplay();
    }

    public void StartGameplay()
    {
        StartCoroutine(uiManager.StartingMenu());

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

    private System.Collections.IEnumerator StartShift()
    {

        yield return null;
    }

    private void GameplayLoop()
    {
        if(listCounter < listLength)
        {
            currentGuestNum = guestLoadOrder[listCounter];
            StartCoroutine(NextGuest(currentGuestNum));

            listCounter++;
        } else
        {
            if(goodCounter >= goodThreshhold)
                StartCoroutine(uiManager.EndLoopSequence(1f, "You've been fired!", 1));
            else if(evilCounter >= evilThreshhold)
                StartCoroutine(uiManager.EndLoopSequence(1f, "The King was killed!", 0));
            else
                StartCoroutine(uiManager.EndLoopSequence(1f, "The ball was a success!", 1));

            
            
        }
    }
    private System.Collections.IEnumerator NextGuest(int guestNum)
    {
        Debug.Log($"Guest {listCounter}, {listOfGuests[guestNum].title}. Imposter? {listOfGuests[guestNum].IsImposter}");
        guestManager.SetGuest(listOfGuests[guestNum].CharacterSprite, listOfGuests[guestNum].MaskSprite);
        guestManager.EnterGuest(npcStartPosition.transform.position, npcEndPosition.transform.position, 1.5f);
        yield return new WaitForSeconds(1.5f);

        uiManager.introducingText.text = listOfGuests[guestNum].title;
        StartCoroutine(uiManager.Fade(uiManager.introducingText, 0f, 1f, 1f));

        yield return new WaitForSeconds(1.5f);
        uiManager.decisionButton.gameObject.SetActive(true);
        uiManager.decisionText.text = "Let Him In?";


        StartCoroutine(Decisions(1f));
        
    }

//If decision button is pressed
    public void MakeDecision()
    {
        decisionMade = true;
    }

    System.Collections.IEnumerator Decisions(float prepTime)
    {
        yield return new WaitForSeconds(prepTime);

        timerToDecide = timeToDecide;
        decisionMade = false;

        while(timerToDecide > 0)
        {
            timerToDecide -= Time.deltaTime;
            uiManager.decisionText.text = "Let Him In?" + Mathf.Round(timerToDecide);

            if(decisionMade == true)
            {
                StartCoroutine(GuestWasKicked());
                yield break;
            }

            yield return null;
        }

        StartCoroutine(GuestWasLetIn());
    }

    private System.Collections.IEnumerator GuestWasKicked()
    {
        Debug.Log("Kicked!");

        if(!listOfGuests[currentGuestNum].IsImposter)
        {
            Debug.Log("Kicked a royal guest!");
            goodCounter++;
        }

        StartCoroutine(uiManager.Fade(uiManager.introducingText, 1f, 0f, 0.5f));
        uiManager.decisionButton.gameObject.SetActive(false);
        uiManager.subtitleText.text = "Off with ya now!";
        yield return new WaitForSeconds(2f);
        uiManager.subtitleText.text = String.Empty;
        StartCoroutine(guestManager.KickOutGuest(0.7f));

        yield return new WaitForSeconds(2f);
        GameplayLoop();

    }

    private System.Collections.IEnumerator GuestWasLetIn()
    {
        Debug.Log($"Let In!");

        if (listOfGuests[currentGuestNum].IsImposter)
        {
            Debug.Log("Let in an assassin!");
            evilCounter++;
        }
        
        StartCoroutine(uiManager.Fade(uiManager.introducingText, 1f, 0f, 0.5f));
        uiManager.decisionButton.gameObject.SetActive(false);
        uiManager.subtitleText.text = "Alright, off you go...";
        yield return new WaitForSeconds(2f);
        uiManager.subtitleText.text = String.Empty;
        StartCoroutine(guestManager.LetInGuest(2f));

        yield return new WaitForSeconds(2f);
        GameplayLoop();
    }

}
