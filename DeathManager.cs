using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class DeathManager : MonoBehaviourPun
{
    public CubeMovement cubeMovement;
    public GameObject player, nameTag;
    public Vector3 cameraLook;
    public int cameraSpeed = 30;
    GameObject mainCamera;
    public bool isDead;
    public float timeBeforeRespawning = 3f;
    public float time;
    TMP_Text textCounter;
    GameObject textPanel;

    public int respawnTime = 6;

    private void Start()
    {
        mainCamera = GameObject.Find("Camera");
        textCounter = GameObject.Find("UI/CounterPanel/Counter").GetComponent<TMP_Text>();
        textPanel = GameObject.Find("UI/CounterPanel");
        time = timeBeforeRespawning + 0.1f;
        isDead = false;
        textPanel.SetActive(false);
    }

    public void OnDead()
    {
        isDead = true;
        player.transform.position = new Vector3(0, 0.75f, 0);
        player.transform.rotation = Quaternion.Euler(0, 0, 0);
        player.GetComponent<CubeMovement>().dead = true;
        photonView.RPC("DeathState", RpcTarget.All, false, true, false);
    }

    void Update()
    {
        if (isDead)
        {
            if (time <= 0.1f)
            {
                cubeMovement.dead = false;
                textPanel.SetActive(false);
                photonView.RPC("DeathState", RpcTarget.All, true, false, true);
                isDead = false;
            }
            else
            {
                time -= Time.deltaTime;
                textCounter.text = $"Respawn in {Mathf.CeilToInt(time)}";
                textPanel.SetActive(true);
            }
        }
        else
        {
            time = timeBeforeRespawning + 0.1f;
        }
    }

    [PunRPC]
    public void DeathState(bool renderer, bool kinematic, bool nametag)
    {
        player.GetComponent<Renderer>().enabled = renderer;
        player.GetComponent<Rigidbody>().isKinematic = kinematic;
        nameTag.SetActive(nametag);
    }
}
