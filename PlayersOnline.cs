using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using Photon.Pun;

public class PlayersOnline : MonoBehaviourPun
{
    public GameSettings GS;
    public TMP_Text playersOnlineText;

    private void Start()
    {
        playersOnlineText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            if (!GS.isGameSettingsOpened)
            {
                Debug.Log("Showing Players.");
                playersOnlineText.gameObject.SetActive(true);
                string result = $"Server : {PhotonNetwork.CurrentRoom.Name}\n";
                result += $"Region : {PhotonNetwork.CloudRegion}\n";
                result += "<size=28>Players online :</size>";
                foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
                {
                    int ping;
                    try
                    {
                        ping = (int)player.CustomProperties["ping"];
                    }
                    catch
                    {
                        ping = 0;
                    }
                    
                    string pingToShow = PingFormat(ping);
                    result += $"\n{player.NickName} - {pingToShow}";
                }
                playersOnlineText.text = result;
            }
            else
            {
                playersOnlineText.gameObject.SetActive(false);
            }
        }
        else
        {
            playersOnlineText.gameObject.SetActive(false);
        }
    }

    private string PingFormat(int ping)
    {
        string format = "";
        if (ping <= 10)
        {
            format += $"<color=#09FF00>{ping}</color>";
        }
        else if (ping > 10 && ping <= 50)
        {
            format += $"<color=#56FF00>{ping}</color>";
        }
        else if(ping > 50 && ping <= 90)
        {
            format += $"<color=#C2FF00>{ping}</color>";
        }
        else if (ping > 90 && ping <= 120)
        {
            format += $"<color=#FFFB00>{ping}</color>";
        }
        else if (ping > 120 && ping <= 190)
        {
            format += $"<color=#FFC800>{ping}</color>";
        }
        else if (ping > 190 && ping <= 5000)
        {
            format += $"<color=#FF2300>{ping}</color>";
        }
        return format;
    }
}
