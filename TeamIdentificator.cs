using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TeamIdentificator : MonoBehaviourPun
{
    TeamsChangeColor teamsChangeColor;

    public void SelectTeam(int team, int colorTeam, string nameTeam)
    {
        photonView.RPC("Team", RpcTarget.AllBuffered, team, colorTeam, nameTeam);
    }

    private void LateUpdate()
    {
        teamsChangeColor = GameObject.Find("UI/GameSettings/GameSettingsPanel/TeamsOptions").GetComponent<TeamsChangeColor>();
    }

    [PunRPC]
    public void Team(int team, int colorTeam, string nameTeam)
    {
        CubeMovement playerCM = gameObject.GetComponent<CubeMovement>();
        playerCM.colorTeam = colorTeam;
        playerCM.team = team;
        playerCM.nameTeam = nameTeam;
    }
}
