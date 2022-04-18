using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class AFKManager : MonoBehaviourPun
{
    public float TimeOut = 120;
    public float TimeAFK;
    public GameServerManager GSM;
    ErrorType error;

    private void Start()
    {
        TimeAFK = TimeOut;
    }

    private void Update()
    {
        error = GameObject.Find("ErrorSwitcher").GetComponent<ErrorType>();
        if (GSM.isInGame)
        {
            TimeAFK -= Time.deltaTime;
            if (TimeAFK < 0)
            {
                KickPlayer();
            }
        }
    }

    void KickPlayer()
    {
        photonView.RPC("Clear", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.UserId);
        PhotonNetwork.LeaveRoom();
        error.OnErrorGetted("KickedByAFK");
    }

    [PunRPC]
    void Clear(string id)
    {
        GSM.ClearPlayer(id);
    }

    public void ResetKey()
    {
        TimeAFK = TimeOut;
    }
}
