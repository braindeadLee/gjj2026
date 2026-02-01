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
    [SerializeField] public Sprite[] backgrounds;

    [SerializeField] public GameObject fadeoutObject;
    private Color transparent = new Color(0,0,0,0);

    [HideInInspector] public UnityEvent introFinished;
    [HideInInspector] public UnityEvent endingFinished;

    public static UIManager Instance {get; private set;}

    private void Awake()
    {
        if (Instance != null && Instance != this) 
            Destroy(this); 
        else 
            Instance = this; 

        decisionText = decisionButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    public IEnumerator StartingMenu()
    {
        Debug.Log("Starting Menu");

        menuUI.SetActive(true); 
        gameplayUI.SetActive(false);

        titleText.gameObject.SetActive(true);
        titleText.alpha = 0f;

        decisionButton.gameObject.SetActive(false);
        subtitleText.text = String.Empty;
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
    }

    public void OnStartClick()
    {
        StartCoroutine(StartGameplay(3f));
    }

    public IEnumerator StartGameplay(float delayTime)
    {

        gameplayUI.SetActive(true);

        Debug.Log("Gameplay Menu");

        introducingText.text = String.Empty;
        introducingText.alpha = 0f;
        subtitleText.text = String.Empty;

        StartCoroutine(Fade(fadeoutObject, transparent, Color.black, 2f));
        yield return new WaitForSeconds(3f);
        menuUI.SetActive(false); 
        backgroundObject.GetComponent<Image>().sprite = backgrounds[1];
        StartCoroutine(Fade(fadeoutObject, Color.black, transparent, 0.5f));
        yield return new WaitForSeconds(delayTime);
        fadeoutObject.SetActive(false);
        
        StartCoroutine(Fade(introducingText, 0f, 1f, 2f));
        introducingText.text = "Start Shift!";
        yield return new WaitForSeconds(3f);
        StartCoroutine(Fade(introducingText, 1f, 0f, 0.5f));

        introFinished?.Invoke();
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