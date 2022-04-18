using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resolution : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5) && !Application.isEditor)
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }
}
