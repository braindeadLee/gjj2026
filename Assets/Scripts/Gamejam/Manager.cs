using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
// using Microsoft.Unity.VisualStudio.Editor;


public class Manager : MonoBehaviour
{
    UIManager uiManager;
    GuestManager guestManager;

    AudioManager audioManager;

    [Header("Winning Conditions")]
    private List<int> guestLoadOrder;
    private int listLength;
    private int listCounter = 0;

    bool letin = false;
    

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
        audioManager = AudioManager.Instance;

        listOfGuests = guestManager.ListOfGuests;

        uiManager.endingFinished.AddListener(StartMenu);

        StartMenu();
    }

    public void StartMenu()
    {
        StartCoroutine(uiManager.StartingMenu());

        SetGuestListOrder(guestManager.ListOfGuests.Length);
        listCounter = 0;
        listLength = listOfGuests.Length;

        audioManager.FadeMusic(0.1f, 2f);
        
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

    private System.Collections.IEnumerator SwitchMusic(AudioClip clip, float delay)
    {

        audioManager.FadeMusic(0f, delay);
        yield return new WaitForSeconds(delay);
        audioManager.SetMusic(clip);
        audioManager.FadeMusic(0.1f, delay);

    }

    // Inside Manager.cs

private void StartGameplay()
{
    audioManager.FadeMusic(0f, 1.5f); 
    
    StartCoroutine(startGameplayLoop(5f));

}

    private System.Collections.IEnumerator startGameplayLoop(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        audioManager.SetMusic(audioManager.gamePlay_Music);
        audioManager.FadeMusic(0.05f, 2f);

        GameplayLoop();
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

            audioManager.FadeMusic(0f, 2f);
            if(goodCounter >= goodThreshhold)
                StartCoroutine(uiManager.EndLoopSequence(1f, "You've been fired!", 4));
            if(evilCounter >= evilThreshhold)
                StartCoroutine(uiManager.EndLoopSequence(1f, "The King was killed!", 3));
            if(goodCounter < goodThreshhold && evilCounter < evilThreshhold)
                StartCoroutine(uiManager.EndLoopSequence(1f, "The ball was a success!", 2));

            goodCounter = evilCounter = 0;
        }
    }
    private System.Collections.IEnumerator NextGuest(int guestNum)
    {
        
        StartCoroutine(subtitleAndAudio(4, 2f, 1f));
        Debug.Log($"Guest {listCounter}, {listOfGuests[guestNum].title}. Imposter? {listOfGuests[guestNum].IsImposter}");
        guestManager.SetGuest(listOfGuests[guestNum].CharacterSprite, listOfGuests[guestNum].MaskSprite);
        guestManager.EnterGuest(npcStartPosition.transform.position, npcEndPosition.transform.position, 1.5f);
        AudioManager.Instance.play_SFX("walkingin", AudioCategory.StateSFX, 2f);
        yield return new WaitForSeconds(1.5f);

        uiManager.introducingText.text = listOfGuests[guestNum].title;
        StartCoroutine(uiManager.Fade(uiManager.introducingText, 0f, 1f, 1f));

        StartCoroutine(Decisions(2f));
    }

//If decision button is pressed
    public void MakeDecisionLetIn()
    {
        decisionMade = true;
        letin = true;

    }

    public void MakeDecisionKickOut()
    {
        decisionMade = true;
        letin = false;
    }

    System.Collections.IEnumerator Decisions(float prepTime)
    {
        yield return new WaitForSeconds(prepTime);

        uiManager.decisionButton1.enabled = true;
        uiManager.decisionButton1.GetComponent<Image>().enabled = true;
        uiManager.decisionButton2.enabled = true;
        uiManager.decisionButton2.GetComponent<Image>().enabled = true;

        uiManager.decisionText.text = String.Empty;

        timerToDecide = timeToDecide;
        decisionMade = false;
        letin = false;

        while(timerToDecide > 0)
        {
            timerToDecide -= Time.deltaTime;
            uiManager.decisionText.text = "" + Mathf.Round(timerToDecide);

            if(decisionMade == true)
            {
                uiManager.decisionText.text = String.Empty;
                if(letin)
                    StartCoroutine(GuestWasLetIn());
                else
                    StartCoroutine(GuestWasKicked());
                yield break;
            }

            yield return null;
        }

        uiManager.decisionText.text = String.Empty;
        if(letin)
               StartCoroutine(GuestWasLetIn());
            else
                StartCoroutine(GuestWasKicked());
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

        int line = UnityEngine.Random.Range(0, 1) + 2;
        StartCoroutine(subtitleAndAudio(line, 2f, 0f));

        uiManager.decisionButton1.enabled = false;
        uiManager.decisionButton1.GetComponent<Image>().enabled = false;
        uiManager.decisionButton2.enabled = false;
        uiManager.decisionButton2.GetComponent<Image>().enabled = false;

        uiManager.subtitleText.alpha = 1f;
        uiManager.subtitleText.text = "Off with ya now!";
        yield return new WaitForSeconds(2f);
        uiManager.subtitleText.text = String.Empty;
        StartCoroutine(guestManager.KickOutGuest(0.6f));

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

        int line = UnityEngine.Random.Range(0, 1);
        StartCoroutine(subtitleAndAudio(line, 2f, 0f));
        
        StartCoroutine(uiManager.Fade(uiManager.introducingText, 1f, 0f, 0.5f));

        uiManager.decisionButton1.enabled = false;
        uiManager.decisionButton1.GetComponent<Image>().enabled = false;
        uiManager.decisionButton2.enabled = false;
        uiManager.decisionButton2.GetComponent<Image>().enabled = false;

        uiManager.subtitleText.alpha = 1f;
        uiManager.subtitleText.text = "Alright, off you go...";
        yield return new WaitForSeconds(2f);
        uiManager.subtitleText.text = String.Empty;
        audioManager.play_SFX("dooropen", AudioCategory.StateSFX, 1f);
        StartCoroutine(guestManager.LetInGuest(2f));

        yield return new WaitForSeconds(2f);
        GameplayLoop();
    }

    private System.Collections.IEnumerator subtitleAndAudio(int num, float subtitleStayTime, float subtitleDelay = 0f)
    {

        String voicename = audioManager.voiceAndSubs[num].name;
        String subtext = audioManager.voiceAndSubs[num].line;
        audioManager.play_SFX(voicename, AudioCategory.VoiceLines, 2f);
    yield return new WaitForSeconds(subtitleDelay);
        uiManager.subtitleText.text = subtext;
        yield return new WaitForSeconds(subtitleStayTime);
        uiManager.subtitleText.text = String.Empty;
    }

}
