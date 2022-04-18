using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasGasGasAudio : MonoBehaviour
{
    public bool isGoingFar = false;
    public AudioSource audio;

    private void Update()
    {
        if (isGoingFar)
        {
            print("Playing GasGasGas");
            audio.Play();
            isGoingFar = false;
        }
    }


}
