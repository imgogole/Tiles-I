using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (PV.IsMine)
        {
            InstanciatePlayer();
        }
    }

    void InstanciatePlayer()
    {
        Debug.Log("Instanciate Player : " + PhotonNetwork.NickName);
        var player = PhotonNetwork.Instantiate("Player", new Vector3(0, 1, 0), Quaternion.identity);
        player.name = "Mine";
    }
}
