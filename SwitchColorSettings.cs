using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SwitchColorSettings : MonoBehaviour
{
    public Button button;
    public TMP_Text buttonText, colorText;
    public ClientManager clientManager;
    public GameServerManager GSM;
    GameObject player;

    private void Start()
    {
        player = GameObject.Find("Mine");
        buttonText.text = clientManager.colorText[player.GetComponent<CubeMovement>().colorOfBody];
        button.image.color = clientManager.colorsAvailable[player.GetComponent<CubeMovement>().colorOfBody];

        print(this.gameObject.name);
    }

    private void Update()
    {
        if (GSM.isInGame)
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
    }

    public void OnChangeColor()
    {
        int newColor = (player.GetComponent<CubeMovement>().colorOfBody + 1) % clientManager.colorsAvailable.Length;
        player.GetComponent<CubeMovement>().ChangeColorBody(newColor);
        buttonText.text = clientManager.colorText[newColor];
        button.image.color = clientManager.colorsAvailable[newColor];
        PlayerPrefs.SetInt("color", newColor);
    }
}
