using UnityEngine;
using UnityEngine.UI; 
using TMPro;
using System.Collections;
using System;

public class UIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] public TextMeshProUGUI subtitleText;
    [SerializeField] public TextMeshProUGUI titleText;
    [SerializeField] public Button decisionButton;
    [SerializeField] public GameObject backgroundObject;
    [SerializeField] public Sprite[] backgrounds;

    public IEnumerator startingMenu()
    {
        subtitleText.gameObject.SetActive(true);
        titleText.gameObject.SetActive(true);
        decisionButton.gameObject.SetActive(false);
        backgroundObject.GetComponent<Image>().sprite = backgrounds[0];

        titleText.text = String.Empty;
        subtitleText.text = String.Empty;

        yield return new WaitForSeconds(1f);

        titleText.text = "Masker Raid";

        yield return new WaitForSeconds(1f);
        
        subtitleText.text = "Click To Start";
    }

    public IEnumerator startGameplay()
    {
        subtitleText.gameObject.SetActive(false);
        titleText.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);

        backgroundObject.GetComponent<Image>().sprite = backgrounds[1];
    }
}