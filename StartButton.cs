using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class StartButton : MonoBehaviourPun
{
    public GameServerManager GSM;
    public GameSettings GS;
    public Outline outline;
    public TMP_Text startText;
    public TMP_Text statutText;
    public GameObject startButtonGO, settings;
    public Material materialCanClickable, materialCantClickable;

    bool clickable;

    private void Start()
    {
        clickable = true;
        outline.enabled = false;
    }

    private void Update()
    {
        if (GS.isGameSettingsOpened || settings.activeSelf)
        {
            clickable = false;
            startText.text = "Settings opened";
            startText.fontSize = 3f;
            startButtonGO.GetComponent<Renderer>().material = materialCantClickable;
        }
        else if (GSM.isCountDownStarting )
        {
            clickable = false;
            startText.text = GSM.currentTime.ToString("0");
            startText.fontSize = 5f;
            startButtonGO.GetComponent<Renderer>().material = materialCantClickable;
        }
        else if (!GSM.isCountDownStarting && !settings.activeSelf && PhotonNetwork.IsMasterClient)
        {
            clickable = true;
            startText.text = "Start";
            startText.fontSize = 5f;
            startButtonGO.GetComponent<Renderer>().material = materialCanClickable;
        }
        else
        {
            clickable = false;
            startText.text = "Waiting for host";
            startText.fontSize = 3.16f;
            startButtonGO.GetComponent<Renderer>().material = materialCantClickable;
        }
    }

    private void OnMouseOver()
    {
        if (clickable)
        {
            outline.enabled = true;
            statutText.gameObject.SetActive(true);
            statutText.text = $"Start game ({GSM.startingTime} seconds countdown)";
        }
    }

    private void OnMouseExit()
    {
        outline.enabled = false;
        statutText.gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        outline.enabled = false;
        if (!GS.isGameSettingsOpened && !settings.activeSelf && clickable) photonView.RPC("OnStartButtonPressed", RpcTarget.All);
    }

    [PunRPC]
    void OnStartButtonPressed()
    {
        GSM.isCountDownStarting = true;
    }
}
