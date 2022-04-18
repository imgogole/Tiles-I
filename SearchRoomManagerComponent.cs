using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class SearchRoomManagerComponent : MonoBehaviourPun
{
    private void Start()
    {
        if (GameObject.Find("RoomManager") == null)
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadSceneAsync(2);
        }
    }
}
