using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AjustVolum : MonoBehaviour
{
    Settings settings;
    public AudioSource[] AudioToAjust;

    private void Update()
    {
        settings = GameObject.Find("ClientManager").GetComponent<Settings>();
        foreach (AudioSource audio in AudioToAjust)
        {
            audio.volume = settings.volume;
        }
    }
}
