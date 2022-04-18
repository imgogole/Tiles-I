using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LeaveSettings : MonoBehaviourPunCallbacks
{
    [SerializeField] Power power;

    public void IsQuitting()
    {
        QuitGame();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadSceneAsync(0);
    }

    void QuitGame()
    {
        if (power.isUsingPower && power.PowerChoosenIndicator == 0)
        {
            power.CallPowerZaWarudo(false);
        }
        PhotonNetwork.LeaveRoom();
        Destroy(GameObject.Find("RoomManager"));
    }

    public void OnApplicationQuit()
    {
        QuitGame();
    }
}
