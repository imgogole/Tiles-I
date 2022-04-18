using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MobileManager : MonoBehaviour
{
    [SerializeField] ClientManager clientManager;
    [SerializeField] Button settingsButtonMobile;
    [SerializeField] Joystick joystick;
    bool isMobile;

    private void Start()
    {
        isMobile = clientManager.isMobile;
        settingsButtonMobile.gameObject.SetActive(isMobile);
        joystick.gameObject.SetActive(isMobile);
    }

    public void OnSettingsButtonMobileTouched()
    {
        clientManager.OnSettingsButtonPressed();
    }
}
