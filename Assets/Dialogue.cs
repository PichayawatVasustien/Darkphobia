using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string lines;
    public float textSpeed;
    public GameObject readyButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        readyButton.SetActive(false);
        textComponent.text = string.Empty;
        startDialog();
    }

    void startDialog()
    {
        StartCoroutine(Typeline());
    }

    IEnumerator Typeline()
    {
        foreach (char c in lines.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSecondsRealtime(textSpeed); 
        }
        yield return new WaitForSecondsRealtime(1f);
        readyButton.SetActive(true);
    }
}
