using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class Chat : MonoBehaviourPun
{
    public GameObject chatDialog;
    public TMP_Text message;

    TMP_InputField chatInput;
    GameSettings GS;
    GameServerManager GSM;
    ClientManager CM;
    GameObject ChatGameObject;
    SendChat chatManager;
    Settings settings;

    bool isShowingChat = false;

    float timeToLive;

    float ttl = 3f;

    private void Awake()
    {
        GS = GameObject.Find("UI/GameSettings").GetComponent<GameSettings>();
        GSM = GameObject.Find("GameServerManager").GetComponent<GameServerManager>();
        CM = GameObject.Find("ClientManager").GetComponent<ClientManager>();
        ChatGameObject = GameObject.Find("UI/Chat/ChatBar");
        chatInput = ChatGameObject.GetComponent<TMP_InputField>();
        chatManager = GameObject.Find("ChatManager").GetComponent<SendChat>();
        settings = GameObject.Find("ClientManager").GetComponent<Settings>();
        message.richText = false;
    }

    private void Start()
    {
        ChatGameObject.SetActive(false);
    }

    private void Update()
    {
        if (GS.isGameSettingsOpened || CM.isSettingsOpened || GSM.isGameEnded)
        {
            ChatGameObject.SetActive(false);
            print("No.");
            return;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            print("Opening chat");
            ChatGameObject.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            chatInput.text = "";
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ChatGameObject.SetActive(false);
            chatInput.text = "";
        }

        if (isShowingChat && !chatDialog.activeSelf)
        {
            chatDialog.SetActive(true);
            timeToLive = 5;
        }

        if (isShowingChat && chatDialog.activeSelf)
        {
            timeToLive -= Time.deltaTime;
            if (timeToLive <= 0)
            {
                isShowingChat = false;
                chatDialog.SetActive(false);
            }
        }
    }

    public void SendMessageToEveryone(string messageToShow)
    {
        photonView.RPC("Verify", RpcTarget.All, messageToShow, photonView.Owner.NickName);
    }

    [PunRPC]
    void Verify(string messageToShow, string owner)
    {
        settings = GameObject.Find("ClientManager").GetComponent<Settings>();
        if (settings.isChatDialog == 1)
        {
            Send(messageToShow);
        }
        else
        {
            chatManager.OnRequestToSend(messageToShow, owner, true);
        }
        chatInput.text = "";
    }

    public void Send(string messageToShow)
    {
        timeToLive = 8;
        message.text = messageToShow;
        isShowingChat = true;
        ChatGameObject.SetActive(false);
    }
}
