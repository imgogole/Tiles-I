using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GetPing : MonoBehaviourPun
{
    ExitGames.Client.Photon.Hashtable PlayerPing = new ExitGames.Client.Photon.Hashtable();
    int NextUpdate = 1;

    private void Awake()
    {
        PlayerPing["ping"] = 0;
        PhotonNetwork.LocalPlayer.CustomProperties = PlayerPing;
    }

    private void Update()
    {
        if (Time.time >= NextUpdate)
        {
            NextUpdate = Mathf.FloorToInt(Time.time) + 1;
            UpdateEverySecond();
        }
    }

    private void UpdateEverySecond()
    {
        PlayerPing["ping"] = PhotonNetwork.GetPing();
        PhotonNetwork.LocalPlayer.CustomProperties = PlayerPing;
    }
}
