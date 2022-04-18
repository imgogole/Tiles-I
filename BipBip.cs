using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BipBip : MonoBehaviourPun
{
    public AudioSource boom;
    public GameObject explosion;
    public GameObject mine;
    public string owner;
    GameObject Player;
    Settings settings;

    private void Start()
    {
        Invoke("Explosion", 1f);
        settings = GameObject.Find("ClientManager").GetComponent<Settings>();
    }

    void Explosion()
    {
        Player = GameObject.Find("Mine");
        explosion.SetActive(true);
        float distance = Vector3.Distance(Player.transform.position, gameObject.transform.position);
        Debug.Log($"Distance between mine and Player is equals to {distance}");
        if (distance <= 5 && !Player.GetComponent<CubeMovement>().dead && Player.GetComponent<PhotonView>().Owner.UserId != owner) 
        {
            Player.GetComponent<ExplosionCube>().DeadWithExplosion();
        }
        boom.volume = settings.volume;
        boom.Play();
        Destroy(mine);
        Destroy(gameObject, 2f);
    }
}
