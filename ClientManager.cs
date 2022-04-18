using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClientManager : MonoBehaviour
{
    public GameObject clientManagerGameObject;
    public GameObject settingsPanel;
    public Color[] colorsAvailable;
    public string[] colorText;
    public bool isSettingsOpened;
    public GameObject chat;
    public ResultGame resultGame;
    public TMP_Text inputMessage;
    public bool isMobile;
    GameObject Player;

    private void Start()
    {
        isSettingsOpened = false;
        settingsPanel.SetActive(false);
        Player = GameObject.Find("Player");
        inputMessage.color = Color.white;
    }

    private void Update()
    {
        if (Player == null)
        {
            Player = GameObject.Find("Mine");
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnSettingsButtonPressed();
        }
    }

    public void OnSettingsButtonPressed()
    {
        if (!chat.activeSelf && resultGame.canOpenChat)
        {
            isSettingsOpened = true;
            settingsPanel.SetActive(true);
        }
    }

    public void OnQuitSettingsButtonPressed()
    {
        isSettingsOpened = false;
        settingsPanel.SetActive(false);
    }

    public void OnMessageSend(string message)
    {
        if (!string.IsNullOrEmpty(message)) Player.GetComponent<Chat>().SendMessageToEveryone(message);
    }

    public void DetectCommand(string message)
    {
        if (message.StartsWith("/"))
        {
            inputMessage.color = Color.yellow;
        }
        else
        {
            inputMessage.color = Color.white;
        }
    }
}
