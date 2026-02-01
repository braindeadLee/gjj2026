using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.Events; 
using TMPro;
using System.Collections;
using System;
using System.Collections.Specialized;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] public GameObject menuUI;
    [SerializeField] public GameObject gameplayUI;
    [SerializeField] public Button startButton;
    [SerializeField] public TextMeshProUGUI subtitleText;
    [SerializeField] public TextMeshProUGUI titleText;
    [SerializeField] public TextMeshProUGUI introducingText;
    [SerializeField] public Button decisionButton;
    public TextMeshProUGUI decisionText;
    [SerializeField] public GameObject backgroundObject;
    [SerializeField] public GameObject backgroundObject2;
    private bool bgSwitch = false;
    [SerializeField] public GameObject helmetObject;
    [SerializeField] public Sprite[] backgrounds;

    [SerializeField] public GameObject fadeoutObject;
    private Color transparent = new Color(0,0,0,0);

    private Vector3 helmetStartScale;
    private Color helmetStartColor;

    [HideInInspector] public UnityEvent introFinished;
    [HideInInspector] public UnityEvent endingFinished;

    public static UIManager Instance {get; private set;}

    private void Awake()
    {
        if (Instance != null && Instance != this) 
            Destroy(this); 
        else 
            Instance = this; 

    }

    private void Start()
    {
        helmetStartScale = helmetObject.transform.localScale;
        helmetStartColor = helmetObject.GetComponent<SpriteRenderer>().color ;

        decisionText = decisionButton.GetComponentInChildren<TextMeshProUGUI>();
        
    }

    public IEnumerator StartingMenu()
    {
        Debug.Log("Starting Menu");

        ResetHelmet();

        menuUI.SetActive(true); 
        gameplayUI.SetActive(true);
        
        startButton.gameObject.SetActive(true);

        titleText.gameObject.SetActive(true);
        titleText.alpha = 0f;

        decisionButton.gameObject.SetActive(false);
        subtitleText.text = "Accept Invitation To Play";
        introducingText.text = String.Empty;
        backgroundObject.SetActive(true);
        fadeoutObject.SetActive(true);

        backgroundObject.GetComponent<Image>().sprite = backgrounds[0];
        // fadeoutObject.GetComponent<Image>().color = Color.black;

        titleText.text = String.Empty;

        StartCoroutine(Fade(fadeoutObject, Color.black, transparent, 1f));

        titleText.text = "Masker Raid";
        StartCoroutine(Fade(titleText,0f, 1f, 1f));

        yield return new WaitForSeconds(1f);
        fadeoutObject.SetActive(false);

    }

    private void ResetHelmet()
    {
        helmetObject.transform.localScale = helmetStartScale;
        helmetObject.GetComponent<SpriteRenderer>().color  = helmetStartColor;
    }

    private IEnumerator ExpandHelmet(float tranSpeed)
    {
        float tranTimer = 0f;

        Vector3 hugeScale = new Vector3(100f,100f,100f);
        Color finishedColor = Color.clear;
        while(tranTimer < tranSpeed)
        {
            helmetObject.transform.localScale = Vector3.Lerp(helmetStartScale, hugeScale, tranTimer/tranSpeed);
            helmetObject.GetComponent<SpriteRenderer>().color = Color.Lerp(helmetStartColor, finishedColor, tranTimer/tranSpeed);
            tranTimer += Time.deltaTime;

            yield return null;
        }
    }

    public void OnStartClick()
    {
        StartCoroutine(StartGameplay(0f));
    }

    public IEnumerator StartGameplay(float delayTime)
    {
        startButton.gameObject.SetActive(false);

        yield return new WaitForSeconds(delayTime);

        StartCoroutine(ExpandHelmet(2f));
        StartCoroutine(BackgroundCrossfade(2f, 1));

        gameplayUI.SetActive(true);

        Debug.Log("Gameplay Menu");

        StartCoroutine(Fade(titleText, 1f, 0f, 2f));

        introducingText.text = String.Empty;
        introducingText.alpha = 0f;
        subtitleText.text = String.Empty;
        yield return new WaitForSeconds(2f);
        
        StartCoroutine(Fade(introducingText, 0f, 1f, 3f));
        introducingText.text = "Start Shift!";
        yield return new WaitForSeconds(3f);
        StartCoroutine(Fade(introducingText, 1f, 0f, 0.5f));

    }

    public System.Collections.IEnumerator BackgroundCrossfade(float delayTime, int bgNum)
    {
        if (!bgSwitch)
        {
            backgroundObject2.GetComponent<Image>().sprite = backgrounds[bgNum];
            backgroundObject.GetComponent<Image>().color = Color.white;
            StartCoroutine(Fade(backgroundObject2, Color.clear, Color.white, delayTime));
            yield return new WaitForSeconds(delayTime);
            StartCoroutine(Fade(backgroundObject, Color.white, Color.clear, delayTime));
            
            bgSwitch = true;
        }
        else
        {
            backgroundObject.GetComponent<Image>().sprite = backgrounds[bgNum];
            backgroundObject2.GetComponent<Image>().color = Color.white;
            StartCoroutine(Fade(backgroundObject, Color.clear, Color.white, delayTime));
            yield return new WaitForSeconds(delayTime);
            StartCoroutine(Fade(backgroundObject2, Color.white, Color.clear, delayTime));

            bgSwitch = false;
            
        }
    }

    public IEnumerator EndLoopSequence(float delayTime, String endingText, int bgNumber)
    {
        yield return new WaitForSeconds(delayTime);

        introducingText.alpha = 0f;
        introducingText.text = "Shift Over!";
        StartCoroutine(Fade(introducingText, 0f, 1f, 1f));
        yield return new WaitForSeconds(3f);

        fadeoutObject.SetActive(true);
        fadeoutObject.GetComponent<Image>().color = Color.clear;
        StartCoroutine(Fade(fadeoutObject, Color.clear, Color.black, 2.5f));
        yield return new WaitForSeconds(2.5f);
        //change background here
        introducingText.text = String.Empty;

        StartCoroutine(EndingSequence(delayTime, endingText, bgNumber));
    }

    public IEnumerator EndingSequence(float delayTime, String endingText, int bgNumber)
    {
        backgroundObject.GetComponent<Image>().sprite = backgrounds[bgNumber];
        StartCoroutine(Fade(fadeoutObject, Color.black, Color.clear, 2f));

        yield return new WaitForSeconds(delayTime);
        introducingText.text = endingText;

        yield return new WaitForSeconds(3f);
        StartCoroutine(Fade(fadeoutObject, Color.clear, Color.black, 2f));
        yield return new WaitForSeconds(3f);
        introducingText.text = String.Empty;

        endingFinished?.Invoke();
    }

    public IEnumerator Fade(TextMeshProUGUI text, float value1, float value2, float fadeTime)
    {
        text.alpha = value1;

        float fadeTimer = 0f;
        
        while(fadeTimer < fadeTime)
        {
            text.alpha = Mathf.Lerp(value1, value2, fadeTimer / fadeTime);

            fadeTimer += Time.deltaTime;
            yield return null;
        }

        text.alpha = value2;
    }

    public IEnumerator Fade(GameObject theObject, Color value1, Color value2, float fadeTime)
    {
        float fadeTimer = 0f;

        Image fadingImage = theObject.GetComponent<Image>();

        fadingImage.color = value1;

        while(fadeTimer < fadeTime)
        {
            fadingImage.color = Color.Lerp(value1, value2, fadeTimer/fadeTime);

            fadeTimer += Time.deltaTime;
            yield return null;
        }

        fadingImage.color = value2;
    }
}