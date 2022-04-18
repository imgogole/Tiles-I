using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class ShowFPS : MonoBehaviour
{
    public TMP_Text fpsText;
    float deltaTime;
    int NextUpdate = 1;

    private void Update()
    {
        if (Time.time >= NextUpdate)
        {
            NextUpdate = Mathf.FloorToInt(Time.time) + 1;
            UpdateEverySecond();
        }
    }

    void UpdateEverySecond()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = Mathf.Ceil(fps).ToString() + " FPS";
    }
}