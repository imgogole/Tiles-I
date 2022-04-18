using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConnectPublic : MonoBehaviour
{
    public string serverName;
    public int actualPlayers, maxPlayers;

    public TMP_Text text;

    private void Start()
    {
        text.text = $"{serverName} - {actualPlayers}/{maxPlayers}";
    }

    public void OnClick()
    {
        GameObject.Find("UI").GetComponent<RoomCreator>().JoinGame(serverName);
    }
}
