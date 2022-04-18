using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBombTouch : MonoBehaviour
{
    public PaintBombe PB;
    public GameObject Explosion;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("floor"))
        {
            PB.OnBombTouched(collision.gameObject);
            Explosion.SetActive(true);
        }
    }
}
