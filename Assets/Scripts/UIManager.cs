using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.Events; 
using TMPro;
using System.Collections;
using System;

public class UIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] public TextMeshProUGUI subtitleText;
    [SerializeField] public TextMeshProUGUI titleText;
    [SerializeField] public Button decisionButton;
    private TextMeshProUGUI decisionText;
    [SerializeField] public GameObject backgroundObject;
    [SerializeField] public Sprite[] backgrounds;

    [SerializeField] public GameObject fadeoutObject;
    private Color transparent = new Color(0,0,0,0);

    [HideInInspector] public UnityEvent introFinished;

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
        subtitleText.gameObject.SetActive(true);
        subtitleText.alpha = 0f;
        titleText.gameObject.SetActive(true);
        titleText.alpha = 0f;

        decisionButton.gameObject.SetActive(false);
        backgroundObject.SetActive(true);
        fadeoutObject.SetActive(true);

        backgroundObject.GetComponent<Image>().sprite = backgrounds[0];
        // fadeoutObject.GetComponent<Image>().color = Color.black;

        titleText.text = String.Empty;
        subtitleText.text = String.Empty;

        StartCoroutine(Fade(fadeoutObject, Color.black, transparent, 1f));

        titleText.text = "Masker Raid";
        StartCoroutine(Fade(titleText,0f, 1f, 1f));

        yield return new WaitForSeconds(1f);
        fadeoutObject.SetActive(false);

        subtitleText.text = "Click To Start";
        StartCoroutine(Fade(subtitleText,0f, 1f, 1f));
    }

    public void OnStartClick()
    {
        StartCoroutine(StartGameplay());
    }

    public IEnumerator StartGameplay()
    {
        StartCoroutine(Fade(titleText,1f, 0f, 0.5f));
        StartCoroutine(Fade(subtitleText,1f, 0f, 0.5f));

        subtitleText.gameObject.SetActive(false);
        titleText.gameObject.SetActive(false);

        fadeoutObject.SetActive(true);
        StartCoroutine(Fade(fadeoutObject, transparent, Color.black, 0.5f));
        yield return new WaitForSeconds(0.5f);
        backgroundObject.GetComponent<Image>().sprite = backgrounds[1];
        StartCoroutine(Fade(fadeoutObject, Color.black, transparent, 0.5f));
        yield return new WaitForSeconds(0.5f);

        introFinished?.Invoke();
    }

    public void showDecideButton()
    {
        decisionButton.gameObject.SetActive(true);
    }

    public void decideButtonText(String textToDisplay)
    {
        decisionText.text = textToDisplay;
    }

    private IEnumerator Fade(TextMeshProUGUI text, float value1, float value2, float fadeTime)
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

    private IEnumerator Fade(GameObject theObject, Color value1, Color value2, float fadeTime)
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