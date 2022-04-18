using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using TMPro;

public class Settings : MonoBehaviourPun
{
    public float volume = 1;
    public int FOV = 33;
    public bool isOrth;
    public int isChatDialog = 1;
    public Camera mainCamera;
    public AudioSource music;
    public AudioSource[] audioSources;
    public AudioClip[] clips;
    public GameSettings GS;
    public Slider volumeSlider, fovSlider;
    public GameObject FPSText;
    public GameServerManager GSM;
    public Toggle isDialogChatToggle;
    bool musicPlaying = false, hasAlreadyCalledGameOver = false;

    public TMP_Text volumeText, fovText;
    public Toggle isOrthoToggle;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("volume")) PlayerPrefs.SetFloat("volume", volume);
        volume = PlayerPrefs.GetFloat("volume");
        if (!PlayerPrefs.HasKey("fov")) PlayerPrefs.SetInt("fov", FOV);
        FOV = PlayerPrefs.GetInt("fov");
        if (!PlayerPrefs.HasKey("orth")) PlayerPrefs.SetInt("orth", isOrth ? 1 : 0);
        isOrth = PlayerPrefs.GetInt("orth") == 1 ? true : false;
        if (!PlayerPrefs.HasKey("dialog")) PlayerPrefs.SetInt("dialog", isChatDialog);
        isChatDialog = PlayerPrefs.GetInt("dialog");
        volumeSlider.value = volume;
        fovSlider.value = FOV;
        volumeText.text = string.Format($"Volume - {Mathf.FloorToInt(volume * 100)}%");
        fovText.text = string.Format($"Field of View - {FOV}");
        isOrthoToggle.isOn = isOrth;
        if (isChatDialog == 1)
        {
            isDialogChatToggle.isOn = true;
        }
        else
        {
            isDialogChatToggle.isOn = false;
        }
    }

    private void Update()
    {
        if (GS.gameType == GameSettings.Template.GasGasGas)
        {
            music.loop = false;
        }
        else
        {
            music.loop = true;
        }
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.volume = volume;
        }
        //pingText.GetComponent<TMPro.TMP_Text>().text = $"Ping : {PhotonNetwork.GetPing()}";

        if (musicPlaying)
        {
            if (!music.isPlaying && GS.gameType == GameSettings.Template.GasGasGas)
            {
                if (!hasAlreadyCalledGameOver)
                {
                    GSM.GameEnded();
                    hasAlreadyCalledGameOver = true;
                }          
            }
        }

        volumeText.text = string.Format($"Volume - {Mathf.FloorToInt(volume * 100)}%");
        fovText.text = string.Format($"Field of View - {FOV}");
    }

    private void LateUpdate()
    {
        if (isOrth)
        {
            mainCamera.orthographicSize = FOV / 7.5f;
            mainCamera.orthographic = true;
        }
        else
        {
            mainCamera.fieldOfView = FOV;
            mainCamera.orthographic = false;
        }
    }

    public void GameStarted()
    {
        if (GS.gameType == GameSettings.Template.GasGasGas)
        {
            music.clip = clips[1];
        }
        else
        {
            music.clip = clips[0];
        }
        music.Play();
        musicPlaying = true;
    }

    public void OnVolumeChanged(float newValue)
    {
        volume = newValue;
        PlayerPrefs.SetFloat("volume", newValue);
        PlayerPrefs.Save();
    }

    public void OnFOVChanged(float newValue)
    {
        int value = Mathf.FloorToInt(newValue);
        FOV = value;
        PlayerPrefs.SetInt("fov", value);
        PlayerPrefs.Save();
    }

    public void OnShowFPSStatueChanged (bool statue)
    {
        FPSText.SetActive(statue);
    }

    public void ChangePerspectiveView(bool option)
    {
        isOrth = option;
        PlayerPrefs.SetFloat("orth", option ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void OnDialogChanged(bool newDialog)
    {
        if (newDialog)
        {
            isChatDialog = 1;
            PlayerPrefs.SetInt("dialog", 1);
            PlayerPrefs.Save();
        }
        else
        {
            isChatDialog = 0;
            PlayerPrefs.SetInt("dialog", 0);
            PlayerPrefs.Save();
        }
    }
}
