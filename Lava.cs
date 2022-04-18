using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    public string id;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<Photon.Pun.PhotonView>().Owner.UserId != id)
        {
            collision.gameObject.GetComponent<ExplosionCube>().DeadWithExplosion();
        }
    }
}
