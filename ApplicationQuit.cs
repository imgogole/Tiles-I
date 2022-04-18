using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationQuit : MonoBehaviour
{
    public Button[] allButtonsScene;

    public void ApplicationQuitFunction()
    {
        Application.Quit();
        if (!Application.isEditor)
        {
            foreach (Button button in allButtonsScene)
            {
                button.interactable = false;
            }
        }
    }
}
