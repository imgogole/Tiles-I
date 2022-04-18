using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ChangeColor : MonoBehaviourPunCallbacks
{
    public bool isTouched = false;
    public bool isTouchable = true;
    public bool isLavable = false;
    public bool isSpawn;
    public int positionX, positionY;
    public GameObject lava;
    public string playerWhoTouchedThisTile = null;
    public Color startingColor;
    public int colorOfThePlayerWhoTouchedThisTile;
    public GameObject cadenas;
    ClientManager clientManager;
    GameServerManager gameServerManager;

    private void Start()
    {
        //clientMagr = GameObject.Find("ClientManager").GetComponent<ClientManager>();
        startingColor = gameObject.GetComponent<Renderer>().material.color;
        clientManager = GameObject.Find("ClientManager").GetComponent<ClientManager>();
        isTouchable = true;
    }

    public void ChangeTouchable(float timeRemaining)
    {
        cadenas.SetActive(true);
        Invoke("ChangeCadenas", timeRemaining);
    }

    public void Lavable(float timeRemaining)
    {
        lava.SetActive(true);
        int[] rotationY = new int[] { 0, 90, 180, 270};
        lava.transform.rotation = Quaternion.Euler(0, rotationY[Random.Range(0, 3)], 0);
        isLavable = true;
        Invoke("ChangeLava", timeRemaining);
    }
    /*
        public void OnTouchedByPlayer(int color, string userID, bool isPowered, float timeLeft)
        {
            photonView.RPC("OnTouched", RpcTarget.All, color, userID, isPowered, timeLeft);
        }

        [PunRPC]
        void OnTouched(int color, string userID, bool isPowered, float timeLeft)
        {
            if (isTouchable)
            {
                GetComponent<Renderer>().material.color = clientManager.colorsAvailable[color];
                playerWhoTouchedThisTile = userID;
                isTouched = true;
                if (isPowered)
                {
                    isTouchable = false;
                    ChangeTouchable(timeLeft);
                }
            }
        }
    */
    private void ChangeCadenas()
    {
        if (GameObject.Find("GameServerManager").GetComponent<GameServerManager>().isGameEnded) return;
        isTouchable = true;
        cadenas.SetActive(false);
    }
    private void ChangeLava()
    {
        if (GameObject.Find("GameServerManager").GetComponent<GameServerManager>().isGameEnded) return;
        isLavable = false;
        lava.SetActive(false);
    }
}
