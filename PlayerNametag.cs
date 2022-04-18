using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerNametag : MonoBehaviourPun
{
    Settings settings;

    public TMP_Text playerNametagText;

    private void Start()
    {
        settings = GameObject.Find("ClientManager").GetComponent<Settings>();
    }

    private void Update()
    {
        SetName();
    }

    public void SetName()
    {
        playerNametagText.text = photonView.Owner.NickName;
        playerNametagText.richText = false;
    }
}
