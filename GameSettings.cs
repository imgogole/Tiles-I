using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class GameSettings : MonoBehaviourPunCallbacks
{

    [HideInInspector] public bool isGameSettingsOpened;

    public GameObject options, teamsOptions, gameSettingsPanel, go_mapHeigthText, go_mapWedthText, timerOfGameText, chooseMap, mh, mw, randomHolesToggle;

    [Range(0, 100)]
    public int mapHolesPercentages;
    public int mapHeight, mapWedth, gamingTime, mapOptimized;
    public bool isMapHoled, isGamePowered, hasTeams, isMapOptimized;

    public Toggle goToggle, power, mapOptimizedToggle;
    public TMP_Dropdown goEnd, typeOfGame;
    public Slider goH, goW;
    public Button goTeamOne, goTeamTwo;
    public TMP_InputField goInputOne, goInputTwo;

    public Camera cameraMain;

    public enum Template { Default, GasGasGas, Customizable, Teams };
    public enum FinishType { Timer, Spread, MusicFinished };

    public Template gameType;
    public FinishType finishType;

    bool hasMusic;

    private void Start()
    {
        chooseMap.SetActive(isMapOptimized);
        isMapOptimized = false;
    }

    public void Reset()
    {
        gameType = Template.Default;
        finishType = FinishType.Timer;
        gamingTime = 300;
        isMapHoled = false;
        hasTeams = false;
        mapHolesPercentages = 0;
        mapHeight = 45;
        mapWedth = 35;
    }

    public void QuitGameSettings()
    {
        isGameSettingsOpened = !isGameSettingsOpened;
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            typeOfGame.interactable = false;
            goEnd.interactable = false;
            goH.interactable = false;
            goW.interactable = false;
            goToggle.interactable = false;
            power.interactable = false;
            goInputOne.interactable = false;
            goInputTwo.interactable = false;
            goTeamOne.interactable = false;
            goTeamTwo.interactable = false;
        }
        else
        {
            typeOfGame.interactable = true;
            goEnd.interactable = true;
            goH.interactable = true;
            goW.interactable = true;
            goToggle.interactable = true;
            power.interactable = true;
            goInputOne.interactable = true;
            goInputTwo.interactable = true;
            goTeamOne.interactable = true;
            goTeamTwo.interactable = true;
        }
        if (isGameSettingsOpened)
        {
            gameSettingsPanel.SetActive(true);
        }
        else
        {
            gameSettingsPanel.SetActive(false);
        }

        if (gameType == Template.Default)
        {
            options.SetActive(false);
            teamsOptions.SetActive(false);

            mapOptimized = 0;
            isMapOptimized = true;

            mapHeight = 35;
            mapWedth = 45;

            gamingTime = 300;

            isMapHoled = false;
            isGamePowered = false;
            hasTeams = false;
            finishType = FinishType.Timer;
        }
        else if (gameType == Template.GasGasGas) {
            options.SetActive(false);
            teamsOptions.SetActive(false);

            mapOptimized = 0;
            isMapOptimized = false;

            mapHeight = 9;
            mapWedth = 85;

            gamingTime = 300;

            isMapHoled = false;
            isGamePowered = false;
            hasTeams = false;
            finishType = FinishType.MusicFinished;
        }
        else if (gameType == Template.Teams)
        {
            options.SetActive(false);
            teamsOptions.SetActive(true);

            mapOptimized = 1;
            isMapOptimized = true;

            mapHeight = 45;
            mapWedth = 45;

            gamingTime = 300;

            isMapHoled = false;
            isGamePowered = false;
            hasTeams = true;
            finishType = FinishType.MusicFinished;
        }
        else
        {
            options.SetActive(true);
            teamsOptions.SetActive(false);
            hasTeams = false;
        }
        chooseMap.SetActive(mapOptimizedToggle.isOn);
        mh.SetActive(!mapOptimizedToggle.isOn);
        mw.SetActive(!mapOptimizedToggle.isOn);
        randomHolesToggle.SetActive(!mapOptimizedToggle.isOn);
    }

    public void OnTypeOfGameChanged(int type)
    {
        photonView.RPC("settings", RpcTarget.AllBuffered, type);
    }

    public void OnTypeOfEndChanged(int typeOf)
    {
        switch (typeOf)
        {
            case 0:
                finishType = FinishType.Timer;
                break;
            case 1:
                finishType = FinishType.Spread;
                break;
            case 2:
                finishType = FinishType.MusicFinished;
                break;
        }
        options.SetActive(true);
    }

    public void OnMapHeigthChanged(float heigth)
    {
        mapHeight = (int)heigth;
        go_mapHeigthText.GetComponent<TMP_Text>().text = "Map heigth : " + mapHeight;
    }

    public void OnMapWedthChanged(float wedth)
    {
        mapWedth = (int)wedth;
        go_mapWedthText.GetComponent<TMP_Text>().text = "Map wedth : " + mapWedth;
    }

    public void OnMapHoledChanged(bool holed)
    {
        isMapHoled = holed;
    }

    public void OnMapOptimizedChanged(bool isOp)
    {
        isMapOptimized = isOp;
        chooseMap.SetActive(isMapOptimized);
        photonView.RPC("OnOptimizedMapChanged", RpcTarget.AllBuffered, isOp, mapOptimized);
    }

    public void OnChangedMapInt(int map)
    {
        mapOptimized = map;
        photonView.RPC("OnOptimizedMapChanged", RpcTarget.AllBuffered, true, map);
    }

    [PunRPC]
    public void OnOptimizedMapChanged(bool isMapOp, int map)
    {
        mapOptimized = map;
        isMapOptimized = isMapOp;
    }

    public void OnGamePoweredChanged(bool powered)
    {
        isGamePowered = powered;
        photonView.RPC("GamePowered", RpcTarget.AllBuffered, powered);
    }

    public void OnGameTimeChanged(float time)
    {
        gamingTime = (int)time;
        timerOfGameText.GetComponent<TMP_Text>().text = string.Format("Time - {0:0}:{1:00}", Mathf.Floor(gamingTime / 60), Mathf.Floor(gamingTime % 60));
        photonView.RPC("timeSet", RpcTarget.AllBuffered, gamingTime);
    }

    [PunRPC]
    public void GamePowered(bool isPowered)
    {
        isGamePowered = isPowered;
    }

    [PunRPC]
    public void timeSet(int time)
    {
        gamingTime = time;
    }

    [PunRPC]
    public void settings(int type)
    {
        switch (type)
        {
            case 0:
                gameType = Template.Default;
                break;
            case 1:
                gameType = Template.GasGasGas;
                break;
            case 2:
                gameType = Template.Customizable;
                break;
            case 3:
                gameType = Template.Teams;
                break;
        }
    }

    public void OnCameraReset()
    {
        photonView.RPC("ResetCam", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void ResetCam()
    {
        cameraMain.backgroundColor = new Color(1, 0.6288218f, 0.3254902f, 1);
    }
}
