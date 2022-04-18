using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class RoomCreator : MonoBehaviourPunCallbacks
{
    public GameObject PlayButton, RestartButton, ServerButton;
    public TMP_InputField inputUsernameInputField;
    public TMP_Text textUnderButton;
    public GameObject roomManager;
    public ErrorType errorType;
    string username;

    public Button[] creatingButtons, playButton;

    // Options for creating room
    public int maxPlayersInGame = 8;
    public bool isPublic = true;
    public string nameServer = "";

    public Slider maxPlayerSlider;
    public Button visibilityButton, joinGame;
    public TMP_Text visibilityButtonText, maxPlayersText, errorExists, connecting;
    public TMP_InputField nameServerInput;
    public Button backButton;

    public Transform Panel;

    string nameOfServerClientWantsToJoin;

    List<string> currentRooms = new List<string>();
    List<RoomInfo> currentRoomsInfos = new List<RoomInfo>();

    void Start()
    {
        errorExists.gameObject.SetActive(false);
        creatingButtons[0].interactable = false;
        IsTyping(null);
        OnRefreshGames();
        RefreshOptions();
        OnTryToConnectToServer();
    }

    public void OnTryToConnectToServer()
    {
        OnConnectedToGame();
        Invoke("Restart", 15f);
    }

    public void OnConnectedToGame()
    {
        PhotonNetwork.Disconnect();
        Debug.Log("Waiting for connecting to master.");
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.NickName = "Default";
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.SendRate = 200;
        PhotonNetwork.SerializationRate = 200;
        PlayButton.SetActive(false);
        RestartButton.SetActive(false);
        if (!PlayerPrefs.HasKey("nickname")) PlayerPrefs.SetString("nickname", "Player");
        username = PlayerPrefs.GetString("nickname");
        PhotonNetwork.NickName = username;
        inputUsernameInputField.text = username;
        textUnderButton.gameObject.SetActive(true);
        textUnderButton.text = "Connecting to master...";
        foreach (Button btn in playButton) btn.interactable = false;
        OnUsernameEnter(inputUsernameInputField.text);
        /*var rm = Instantiate(roomManager);
        rm.SetActive(true);
        rm.name = "RoomManager";*/
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master.");
        textUnderButton.text = "Connected, joining lobby...";
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined " + PhotonNetwork.CloudRegion + " lobby.");
        PlayButton.SetActive(true);
        RestartButton.SetActive(false);
        textUnderButton.gameObject.SetActive(false);
        foreach (Button btn in playButton) btn.interactable = true;
    }



    public void OnCreatedRoom()
    {
        nameServerInput.interactable = false;
        maxPlayerSlider.interactable = false;
        textUnderButton.gameObject.SetActive(true);
        foreach (Button btn in creatingButtons)
        {
            btn.interactable = false;
        }
        connecting.text = "Connecting";
        connecting.color = Color.yellow;
        PlayButton.SetActive(false);
        RestartButton.SetActive(false);
        textUnderButton.text = "Connecting to server...";
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)maxPlayersInGame;
        roomOptions.PublishUserId = true;
        roomOptions.IsVisible = isPublic;
        PhotonNetwork.CreateRoom(nameServer, roomOptions, TypedLobby.Default);
    }

    public void OnUsernameChanged(string newUsername)
    {
        username = newUsername;
        PlayerPrefs.SetString("nickname", username);
        PhotonNetwork.NickName = username;
        inputUsernameInputField.text = username;
    }

    public override void OnJoinedRoom()
    {
        //CheckNickname();
        textUnderButton.text = "Loading map...";
        PhotonNetwork.LoadLevel(1);
    }

    public void OnTouchUsernameChange()
    {
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, true, true);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        foreach (Button btn in creatingButtons)
        {
            btn.interactable = true;
        }
        errorType.OnErrorGetted(message, returnCode);
    }

    public void CheckNickname()
    {
        bool hasUniqueNickName = false;
        int index = 0;
        while (!hasUniqueNickName)
        {
            foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                if (player.NickName == PhotonNetwork.NickName)
                {
                    index++;
                    PhotonNetwork.NickName = PhotonNetwork.NickName + " " + index.ToString();
                }
                else
                {
                    hasUniqueNickName = true;
                }
            }
        }
    }

    public void Restart()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            RestartButton.SetActive(true);
        }
    }

    void RefreshOptions()
    {
        maxPlayerSlider.value = maxPlayersInGame;
        maxPlayersText.text = maxPlayersInGame.ToString();
        visibilityButtonText.text = isPublic ? "Public" : "Private";
        nameServerInput.text = nameServer;
    }

    public void OnMaxPlayersValueChanged(float value)
    {
        maxPlayersInGame = (int)value;
        RefreshOptions();
    }

    public void OnInputServerNameEnter(string name)
    {
        errorExists.gameObject.SetActive(false);
        bool isGameExists = CheckIfGameExists(name);
        if (isGameExists)
        {
            nameServer = "";
            errorExists.gameObject.SetActive(true);
            creatingButtons[0].interactable = false;
        }
        else
        {
            errorExists.gameObject.SetActive(false);
            nameServer = name;
            creatingButtons[0].interactable = !string.IsNullOrEmpty(name);
            RefreshOptions();
        }
    }

    public void OnVisibilityChanged()
    {
        isPublic = !isPublic;
        RefreshOptions();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        currentRooms.Clear();
        foreach (RoomInfo room in roomList)
        {
            Debug.Log($"Added {room.Name} as new server.");
            currentRooms.Add(room.Name);
            currentRoomsInfos.Add(room);
        }
    }

    bool CheckIfGameExists(string nameOfRoom)
    {
        foreach (string room in currentRooms)
        {
            if (room.Equals(nameOfRoom))
            {
                return true;
            }
        }
        return false;
    }

    public void OnRefreshGames()
    {
        foreach (Transform button in Panel.transform)
        {
            Destroy(button.gameObject);
        }
        foreach (RoomInfo room in currentRoomsInfos)
        {
            if (room.IsVisible)
            {
                var button = GameObject.Instantiate(ServerButton, Panel);
                button.name = $"[Button] {room.Name}";
                ConnectPublic connectPublic = button.GetComponent<ConnectPublic>();
                connectPublic.serverName = room.Name;
                connectPublic.actualPlayers = room.PlayerCount;
                connectPublic.maxPlayers = room.MaxPlayers;
            }
        }
    }

    public void IsTyping(string name)
    {
        joinGame.interactable = !string.IsNullOrEmpty(name);
        nameOfServerClientWantsToJoin = name;
    }

    public void JoinGamePrivate()
    {
        foreach (Button btn in creatingButtons)
        {
            btn.interactable = false;
        }
        joinGame.interactable = false;
        joinGame.GetComponentInChildren<TMP_Text>().text = "Connecting...";
        PhotonNetwork.JoinRoom(nameOfServerClientWantsToJoin);
        Debug.Log($"Name : {nameOfServerClientWantsToJoin}");
    }
    public void EnterInTestRoom()
    {
        foreach (Button btn in creatingButtons)
        {
            btn.interactable = false;
        }
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 8;
        roomOptions.PublishUserId = true;
        roomOptions.IsVisible = false;
        PhotonNetwork.JoinOrCreateRoom("InternalTestServerOfDev[>18chars]", roomOptions, TypedLobby.Default);
    }
    public void JoinGame(string name)
    {
        foreach (Button btn in creatingButtons)
        {
            btn.interactable = false;
        }
        PhotonNetwork.JoinRoom(name);
    }
    public void OnUsernameEnter(string name)
    {
        foreach (Button btn in playButton)
        {
            btn.interactable = !string.IsNullOrEmpty(name);
        }
    }
}
