using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class TeamsChangeColor : MonoBehaviourPun
{
    public int teamColorOne;
    public int teamColorTwo;

    public string teamNameOne;
    public string teamNameTwo;

    public Button buttonTeamOne, buttonTeamTwo;
    public TMP_Text buttonTextOne, buttonTextTwo;

    public TMP_InputField inputOne, inputTwo;

    public ClientManager clientManager;

    private void Start()
    {
        teamColorOne = 0;
        teamColorTwo = 4;
        teamNameOne = "Children of atom";
        teamNameTwo = "World Eaters";
        RefreshTeamsOptions();
    }

    public void OnNameTeamOneChanged(string name)
    {
        teamNameOne = name;
        RefreshTeamsOptions();
    }

    public void OnNameTeamTwoChanged(string name)
    {
        teamNameTwo = name;
        RefreshTeamsOptions();
    }

    public void OnColorTeamChanged(int team)
    {
        if (team == 1)
        {
            teamColorOne = (teamColorOne + 1) % clientManager.colorsAvailable.Length;
        }
        else if (team == 2)
        {
            teamColorTwo = (teamColorTwo + 1) % clientManager.colorsAvailable.Length;
        }
        RefreshTeamsOptions();
    }

    void RefreshTeamsOptions()
    {
        photonView.RPC("Refresh", RpcTarget.AllBuffered, teamColorOne, teamColorTwo, teamNameOne, teamNameTwo);
    }

    [PunRPC]
    public void Refresh(int teamColorO, int teamColorT, string teamNameO, string teamNameT)
    {
        teamColorOne = teamColorO;
        teamColorTwo = teamColorT;
        teamNameOne = teamNameO;
        teamNameTwo = teamNameT;
        buttonTeamOne.image.color = clientManager.colorsAvailable[teamColorO];
        buttonTeamTwo.image.color = clientManager.colorsAvailable[teamColorT];
        buttonTextOne.text = clientManager.colorText[teamColorO];
        buttonTextTwo.text = clientManager.colorText[teamColorT];
        inputOne.text = teamNameO;
        inputTwo.text = teamNameT;
    }
}
