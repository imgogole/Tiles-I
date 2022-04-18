using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameSettingsButton : MonoBehaviour
{
    public Outline outline;
    public GameSettings GS;
    public GameObject settings;
    public TMP_Text statutText;

    private void Start()
    {
        outline.enabled = false;
    }

    private void OnMouseOver()
    {
        if (!GS.isGameSettingsOpened && !settings.activeSelf && Photon.Pun.PhotonNetwork.IsMasterClient)
        {
            outline.enabled = true;
            statutText.gameObject.SetActive(true);
            statutText.text = "Open Game settings";
        }
    }

    private void OnMouseExit()
    {
        outline.enabled = false;
        statutText.gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (!GS.isGameSettingsOpened && !settings.activeSelf && Photon.Pun.PhotonNetwork.IsMasterClient) GS.isGameSettingsOpened = true;
    }
}
