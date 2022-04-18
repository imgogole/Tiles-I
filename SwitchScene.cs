using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    public void OnSwitchScene(int index)
    {
        SceneManager.LoadSceneAsync(index);
    }
}
