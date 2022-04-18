using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;

public class LeaveRoom : MonoBehaviourPunCallbacks
{
    public Outline outline;
    public GameSettings GS;
    public GameObject loading;
    public GameObject settings;
    public TMP_Text statutText;

    bool isLeavingRoomByHimself = false;

    private void Start()
    {
        loading.SetActive(false);
        outline.enabled = false;
    }

    private void OnMouseOver()
    {
        if (!GS.isGameSettingsOpened && !settings.activeSelf)
        {
            outline.enabled = true;
            statutText.gameObject.SetActive(true);
            statutText.text = "Leave game";
        }
    }

    private void OnMouseExit()
    {
        outline.enabled = false;
        statutText.gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (!GS.isGameSettingsOpened && !settings.activeSelf) Quit();
    }

    public void Quit()
    {
        PhotonNetwork.LeaveRoom();
        loading.SetActive(true);
        Destroy(GameObject.Find("RoomManager"));
        isLeavingRoomByHimself = true;
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
        if (isLeavingRoomByHimself) SceneManager.LoadSceneAsync(0);
    }
}
