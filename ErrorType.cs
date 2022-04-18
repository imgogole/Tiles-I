using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ErrorType : MonoBehaviour
{
    [HideInInspector] public string ErrorReason;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }


    //<summary>Hey</summary>
    public void OnErrorGetted(string error, short errorInt = 0)
    {
        Debug.LogError("ERROR : " + error + ".INDEX : " + errorInt);
        switch (error)
        {
            default:
                ErrorReason = "Unknow reason";
                break;
            case "Operation not allowed in current state":
                ErrorReason = "Unable to connect to the server.";
                break;
            case "Invalid operation code":
                ErrorReason = "Unable to perform operations on your request to join the server.";
                break;
            case "Invalid operation":
                ErrorReason = "Unable to reach the server.\nPlease try again.";
                break;
            case "Internal server error":
                ErrorReason = "The server is having problems.\nPlease try again later.\nIf the problem persists, well, too bad.";
                break;
            case "Invalid authentication":
                ErrorReason = "Unable to communicate with servers with this version.\nPlease update the game.";
                break;
            case "GameId already exists":
                ErrorReason = "This server name already exists.\nPlease choose another server name.";
                break;
            case "Game full":
                ErrorReason = "The game is full.\nContact the host for see if you can join the server later.";
                break;
            case "Game closed":
                ErrorReason = "Game already started.\nContact the host for see if you can join the server later.";
                break;
            case "Server full":
                ErrorReason = "Servers are currently full.\nCome back later.";
                break;
            case "Game does not exist":
                ErrorReason = "This server does not exist.\nMake sure you have correctly marked its name.";
                break;
            case "Max ccu reached":
                ErrorReason = "Servers are currently full.\nPlease wait.";
                break;
            case "Slot error":
                ErrorReason = "The game is full.\nContact the host for see if you can join the server later.";
                break;
            case "ClientTimeout":
                ErrorReason = "Client didn't responding after a few time.\nCheck your connection.";
                break;
            case "ServerTimeout":
                ErrorReason = "Server didn't responding after a few time.\nLook at another server.";
                break;
            case "KickedByAFK":
                ErrorReason = "You have been AFK too long.\nContact the host if you can join the server next time.";
                break;
            case "ClientTimeout2":
                ErrorReason = "Client didn't responding after a few time.\nPing was superior to 1200 ms.\nCheck your connection.";
                break;
        }

        SceneManager.LoadSceneAsync(3);
    }
}
