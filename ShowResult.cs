using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowResult : MonoBehaviour
{
    public AudioClip bip;
    public AudioSource audioSource;

    public Settings settings;
    public Color[] colors;

    public TMP_Text text;

    public bool isFinishToShow = false;
    int percentages;
    int colorOfBody;

    private void Start()
    {
        settings = GameObject.Find("ClientManager").GetComponent<Settings>();
    }

    public void Show(string username, int percentage, int color)
    {
        percentages = percentage;
        colorOfBody = color;
        StartCoroutine("showR", username);
    }

    IEnumerator showR(string username)
    {
        for (int i = 0; i < percentages; i++)
        {
            text.text = username + " - " + i + "%";
            text.color = colors[colorOfBody];
            audioSource.clip = bip;
            audioSource.Play();
            yield return new WaitForSeconds(0.05f);
        }
        text.text = username + " - " + percentages + "%";
        text.color = colors[colorOfBody];
        audioSource.clip = bip;
        audioSource.volume = settings.volume;
        audioSource.Play();
        isFinishToShow = true;
    }
}
