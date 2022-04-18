using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PaintBombe : MonoBehaviourPun
{
    [HideInInspector] public int colorOfBody;
    [HideInInspector] public string nameID;
    ClientManager clientManager;
    GameObject[] players;
    public GameObject meshBomb;
    public GameObject[] BombMesh, floors;
    public float force = 600;

    private void Awake()
    {
        clientManager = GameObject.Find("ClientManager").GetComponent<ClientManager>();
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            foreach (GameObject floor in floors)
            {
                Physics.IgnoreCollision(player.GetComponent<Collider>(), floor.GetComponent<Collider>());
            }
        }
    }

    [PunRPC]
    public void UpdateThis(int color, string nameID)
    {
        GetComponent<PaintBombe>().colorOfBody = color;
        GetComponent<PaintBombe>().nameID = nameID;
    }

    private void Start()
    {
        photonView.RPC("UpdateThis", RpcTarget.All, colorOfBody, nameID);
        foreach (GameObject mesh in BombMesh)
        {
            mesh.GetComponent<MeshRenderer>().material.color = clientManager.colorsAvailable[colorOfBody];
        }
        GetComponent<Rigidbody>().freezeRotation = true;
        GetComponent<Rigidbody>().AddForce(Vector3.up * force);
    }

    private void FixedUpdate()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            foreach (GameObject floor in floors)
            {
                Physics.IgnoreCollision(player.GetComponent<Collider>(), floor.GetComponent<Collider>());
            }
        }
    }

    public void OnBombTouched(GameObject tileCollision)
    {
        if (tileCollision.GetComponent<ChangeColor>().isTouchable)
        {
            tileCollision.GetComponent<Renderer>().material.color = clientManager.colorsAvailable[colorOfBody];
            tileCollision.GetComponent<ChangeColor>().playerWhoTouchedThisTile = nameID;
            tileCollision.GetComponent<ChangeColor>().isTouched = true;
        }
        Destroy(meshBomb);
        Invoke("Destroy", 1.5f);
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
