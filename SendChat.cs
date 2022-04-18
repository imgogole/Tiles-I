using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;


public class SendChat : MonoBehaviourPun
{
    public GameObject prefab, tchat, input;

    public void OnRequestToSend(string message, string player, bool isLocal = false, bool isInfo = false)
    {
        input.SetActive(false);
        if (message.StartsWith("/"))
        {
            ExecuteCommand(message, player);
            return;
        }
        if (isLocal)
        {
            var messagePrefab = Instantiate(prefab, tchat.transform);
            if (!isInfo)
            {
                messagePrefab.GetComponent<TMP_Text>().text = $"{player} : {message}";
                messagePrefab.GetComponent<TMP_Text>().richText = false;
                messagePrefab.GetComponent<TMP_Text>().color = Color.gray;
            }
            else
            {
                messagePrefab.GetComponent<TMP_Text>().text = $"{message}";
                messagePrefab.GetComponent<TMP_Text>().richText = true;
            }
            Destroy(messagePrefab, 10f);
        }
        else
        {
            photonView.RPC("Send", RpcTarget.All, message, player, isInfo);
        }
    }

    [PunRPC]
    void Send(string message, string player, bool isInfo = false)
    {
        var messagePrefab = Instantiate(prefab, tchat.transform);
        if (!isInfo)
        {
            messagePrefab.GetComponent<TMP_Text>().text = $"{player} : {message}";
            messagePrefab.GetComponent<TMP_Text>().richText = false;
            messagePrefab.GetComponent<TMP_Text>().color = Color.gray;
        }
        else
        {
            messagePrefab.GetComponent<TMP_Text>().text = $"{message}";
            messagePrefab.GetComponent<TMP_Text>().richText = true;
        }
        Destroy(messagePrefab, 5f);
    }

    void ExecuteCommand(string messageWithSlash, string owner)
    {
        string message = messageWithSlash.Substring(1);
        string[] args = message.Split(' ');
        switch (args[0].ToLower())
        {
            case "kick":
                if (PhotonNetwork.IsMasterClient)
                {
                    string player = "";
                    foreach (string arg in args)
                    {
                        if (arg == "kick") continue;
                        player += arg + " ";
                    }
                    player = player.TrimEnd();
                    foreach (Player plyr in PhotonNetwork.CurrentRoom.Players.Values)
                    {
                        if (plyr.NickName == player)
                        {
                            PhotonNetwork.CloseConnection(plyr);
                            OnRequestToSend($"<color=orange>You kicked {player}</color>", "", true, true);
                        }
                    }
                }
                else
                {
                    OnRequestToSend("<color=red>You don't have permissions.</color>", "", true, true);
                }
                break;
        }
    }
}
