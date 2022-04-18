using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System;

public class GameServerManager : MonoBehaviourPunCallbacks
{
    public Dictionary<string, string> playersOnlines = new Dictionary<string, string>();
    public GameObject[] players;
    public AudioSource Cm, CoundDown;
    public SendChat chatManager;
    public bool isCountDownStarting, isInGame, isTimerEnclenched, isGameEnded, showStats = false, isEveryoneReady;
    public float startingTime, timeGame;
    public GenerateMap GM;
    public GameSettings GS;
    public GameObject clientManager, startButton, leaveButton, settingsButton, lobbyMap, map, timerOfGameText, cameraMain, gameOverText, label, textLeaveRoom, result, statutText, t_Versus, t_TeamOne, t_TeamTwo;
    public Image background;
    public Settings settings;
    public TeamsChangeColor teamsChangeColor;
    public List<GameObject> teamOne = new List<GameObject>(), teamTwo = new List<GameObject>();
    public Animator[] anims;
    public ErrorType error;
    [HideInInspector] public float currentTime, currentGameTime;
    public int numberOfReady;
    float timeStartCountingDown;

    public bool isTimeStopped;

    private void Start()
    {
        currentTime = startingTime;
        currentGameTime = timeGame;
        t_TeamOne.SetActive(false);
        t_TeamTwo.SetActive(false);
        t_Versus.SetActive(false);
        timerOfGameText.SetActive(false);
        gameOverText.SetActive(false);
        result.SetActive(false);
        background.gameObject.SetActive(false);
        playersOnlines.Clear();
        isCountDownStarting = false;
        isInGame = false;
        isTimerEnclenched = false;
        PhotonNetwork.CurrentRoom.IsOpen = true;
        showStats = false;
        isTimeStopped = false;
        timeStartCountingDown = 3f;
        error = GameObject.Find("ErrorSwitcher").GetComponent<ErrorType>();
    }

    private void Update()
    {
        if (!isInGame)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
        }


        if (!isInGame)
        {
            startButton.SetActive(true);
            leaveButton.SetActive(true);
            settingsButton.SetActive(true);

            if (isCountDownStarting)
            {
                currentTime -= 1 * Time.deltaTime;
                if (currentTime <= 1)
                {
                    startButton.SetActive(false);
                    leaveButton.SetActive(false);
                    settingsButton.SetActive(false);
                    statutText.SetActive(false);
                    isCountDownStarting = false;
                    photonView.RPC("TimeGame", RpcTarget.All);
                    Destroy(lobbyMap);
                    label.SetActive(false);
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    PhotonNetwork.CurrentRoom.IsVisible = false;
                    isInGame = true;
                    GS.isGameSettingsOpened = false;
                    foreach (GameObject player in players)
                    {
                        player.GetComponent<CubeMovement>().canMove = false;
                        player.GetComponent<CubeMovement>().isGameStarted = true;
                        player.transform.position = new Vector3(0, 1, 0);
                        player.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    if (PhotonNetwork.IsMasterClient)
                    {
                        if (GS.hasTeams)
                        {
                            Debug.Log("The Game is played with teams. Choosing teams.");
                            ChooseTeam();
                        }
                        GM.GeneratingMap();
                    }
                    currentGameTime = GS.gamingTime;
                    if (GS.hasTeams)
                    {
                        TransitionTeams();
                    }
                    isTimerEnclenched = true;
                }
            }
            else
            {
                currentTime = startingTime;
            }
        }
        if (isTimerEnclenched && !isGameEnded)
        {
            timerOfGameText.SetActive(true);
            if (isEveryoneReady)
            {
                // Ici est le code pendant la partie. C'est-à-dire à la fin du compte à rebours et la fin du jeu.

                if (PhotonNetwork.GetPing() >= 1200)
                {
                    PhotonNetwork.LeaveRoom();
                    error.OnErrorGetted("ClientTimeout2");
                }

                if (!isTimeStopped) currentGameTime -= Time.deltaTime;
                CheckGasGas();
                timerOfGameText.GetComponent<TMP_Text>().fontSize = 42;
                timerOfGameText.GetComponent<TMP_Text>().text = string.Format("{0:0}:{1:00}", Mathf.Floor(currentGameTime / 60), Mathf.Floor(currentGameTime % 60));
                if (currentGameTime <= 0)
                {
                    GameEnded();
                }
            }
            else
            {
                if (numberOfReady >= PhotonNetwork.CurrentRoom.PlayerCount)
                {
                    if (timeStartCountingDown == 3)
                    {
                        CoundDown.Play();
                    }
                    if (timeStartCountingDown >= 0)
                    {
                        timeStartCountingDown -= Time.deltaTime;
                        timerOfGameText.GetComponent<TMP_Text>().text = Mathf.CeilToInt(timeStartCountingDown).ToString();
                    }
                    else
                    {
                        timerOfGameText.GetComponent<TMP_Text>().text = "Waiting for host";
                    }
                    if (timeStartCountingDown <= 0 && PhotonNetwork.IsMasterClient)
                    {
                        photonView.RPC("CanEveryoneMove", RpcTarget.All);
                        Debug.Log("Everyone can move");
                    }
                }
                else
                {
                    timerOfGameText.GetComponent<TMP_Text>().text = "Wainting for everyone ready...";
                    photonView.RPC("Repair", RpcTarget.All, GS.isGamePowered);
                }
            }

        }
        if (isGameEnded && gameOverText.activeSelf)
        {
            gameOverText.GetComponent<TMP_Text>().color = Color.Lerp(gameOverText.GetComponent<TMP_Text>().color, Color.white, 3.5f * Time.deltaTime);
            background.color = Color.Lerp(background.color, new Color(0, 0, 0, 0.9f), 4 * Time.deltaTime);
            if (gameOverText.GetComponent<TMP_Text>().color.a > 0.98f)
            {
                background.color = Color.Lerp(background.color, new Color(0, 0, 0, 0.9f), 4 * Time.deltaTime);
                gameOverText.transform.localPosition = Vector3.Lerp(gameOverText.transform.localPosition, new Vector3(0, 172, 0), 3.5f * Time.deltaTime);
                gameOverText.GetComponent<TMP_Text>().fontSize = Mathf.Lerp(gameOverText.GetComponent<TMP_Text>().fontSize, 60, 3.5f * Time.deltaTime);
                
                if (!showStats && PhotonNetwork.IsMasterClient)
                {
                    result.GetComponent<ResultGame>().CalculResult();
                    showStats = true;
                }         
            }
        }
    }

    private void CheckGasGas()
    {
        if (GS.gameType == GameSettings.Template.GasGasGas)
        {
            print(currentGameTime);
            cameraMain.GetComponent<Camera>().backgroundColor = Color.black;
            var player = GameObject.Find("Mine").GetComponent<CubeMovement>();

            // Modifier le temps à laquelle "Gas gas gas" apparait.
            if (currentGameTime <= 250)
            {
                timerOfGameText.SetActive(false);
            }

            if (currentGameTime <= 222f && currentGameTime >= 184f)
            {
                if (player.isTumbling == false) player.tumblingDuration = 0.05f;
            }
            else if (currentGameTime <= 159.5f && currentGameTime >= 109.5f)
            {
                if (player.isTumbling == false) player.tumblingDuration = 0.05f;
            }
            else if (currentGameTime <= 83.1f)
            {
                if (player.isTumbling == false) player.tumblingDuration = 0.05f;
            }
            else
            {
                player.tumblingDuration = 0.2f;
            }
        }
        timerOfGameText.GetComponent<TMP_Text>().text = "Game'll ending when music is over.";
        timerOfGameText.GetComponent<TMP_Text>().fontSize = 28;
    }

    public void GameEnded()
    {
        isGameEnded = true;
        currentGameTime = 0;
        Cm.Stop();
        settingsButton.SetActive(false);
        gameOverText.SetActive(true);
        background.gameObject.SetActive(true);
        background.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        gameOverText.GetComponent<TMP_Text>().color = new Color(1, 1, 1, 0);
        foreach (GameObject player in players)
        {
            player.GetComponent<CubeMovement>().canMove = false;
        }
        timerOfGameText.SetActive(false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        playersOnlines.Add(newPlayer.NickName, newPlayer.UserId);
        chatManager.OnRequestToSend($"<color=green>{newPlayer.NickName} joined the game</color>", "", false, true);
        isCountDownStarting = false;
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        playersOnlines.Remove(otherPlayer.NickName);
        chatManager.OnRequestToSend($"<color=red>{otherPlayer.NickName} left the game</color>", "", false, true);
        isCountDownStarting = false;
        ClearPlayer(otherPlayer.UserId);
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    public void ClearPlayer(string player)
    {
        if (PhotonNetwork.IsMasterClient) photonView.RPC("Clear", RpcTarget.All, player);
    }

    [PunRPC]
    public void Clear(string otherPlayer)
    {
        if (isInGame)
        {
            foreach (GameObject tile in GameObject.FindGameObjectsWithTag("floor"))
            {
                if (tile.GetComponent<ChangeColor>().playerWhoTouchedThisTile == otherPlayer)
                {
                    var changeColor = tile.GetComponent<ChangeColor>();
                    tile.GetComponent<Renderer>().material.color = changeColor.startingColor;
                    changeColor.playerWhoTouchedThisTile = null;
                    changeColor.colorOfThePlayerWhoTouchedThisTile = 0;
                    changeColor.isTouched = false;
                }
            }
        }/*
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if (player.UserId == otherPlayer)
            {
                PhotonNetwork.CloseConnection(player);
            }
        }*/
    }

    [PunRPC]
    public void TimeGame() {
        currentTime = GS.gamingTime;
    }

    public void ChooseTeam()
    {
        int numberOfPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        int teamLength = Mathf.FloorToInt(numberOfPlayers / 2);
        print("Players : " + numberOfPlayers + ". Players per teams : " + teamLength);
        int i = 0;
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (i % 2 == 0)
            {
                teamOne.Add(player);
            }
            else
            {
                teamTwo.Add(player);
            }
            i++;
        }

        int teamColorOne = teamsChangeColor.teamColorOne, teamColorTwo = teamsChangeColor.teamColorTwo;
        string teamNameOne = teamsChangeColor.teamNameOne, teamNameTwo = teamsChangeColor.teamNameTwo;
        foreach (GameObject player in teamOne)
        {
            player.GetComponent<TeamIdentificator>().SelectTeam(1, teamColorOne, teamNameOne);
        }
        foreach (GameObject player in teamTwo)
        {
            player.GetComponent<TeamIdentificator>().SelectTeam(2, teamColorTwo, teamNameTwo);
        }
    }

    void TransitionTeams()
    {
        foreach (Animator anim in anims)
        {
            anim.gameObject.SetActive(true);
        }
        t_TeamOne.GetComponent<TMP_Text>().text = teamsChangeColor.teamNameOne;
        t_TeamTwo.GetComponent<TMP_Text>().text = teamsChangeColor.teamNameTwo;
        t_TeamOne.GetComponent<TMP_Text>().color = clientManager.GetComponent<ClientManager>().colorsAvailable[teamsChangeColor.teamColorOne];
        t_TeamTwo.GetComponent<TMP_Text>().color = clientManager.GetComponent<ClientManager>().colorsAvailable[teamsChangeColor.teamColorTwo];
        foreach (Animator anim in anims)
        {    
            anim.SetBool("start", true);
        }
    }

    public void OnPlayerReady()
    {
        photonView.RPC("OnReady", RpcTarget.All);
        print("A player is ready");
    }

    [PunRPC]
    public void OnReady()
    {
        numberOfReady++;
    }

    [PunRPC]
    public void CanEveryoneMove()
    {
        GameObject.Find("Mine").GetComponent<CubeMovement>().canMove = true;
        settings.GameStarted();
        isEveryoneReady = true;
        timeGame -= PhotonNetwork.GetPing() * 0.001f;
    }

    public void OnChangedBackground(Color color)
    {
        photonView.RPC("ChangeBackground", RpcTarget.AllBuffered, color.r, color.g, color.b);
    }

    [PunRPC]
    void ChangeBackground(float r, float g, float b)
    {
        cameraMain.GetComponent<Camera>().backgroundColor = new Color(r, g, b, 1);
    }
    [PunRPC]
    void Repair(bool isGamePowered)
    {
        startButton.SetActive(false);
        leaveButton.SetActive(false);
        settingsButton.SetActive(false);
        statutText.SetActive(false);
        isCountDownStarting = false;
        photonView.RPC("TimeGame", RpcTarget.All);
        Destroy(lobbyMap);
        label.SetActive(false);
        GS.isGamePowered = isGamePowered;
    }
}
