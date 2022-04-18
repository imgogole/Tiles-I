using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;

public class ShowError : MonoBehaviour
{
    ErrorType errorType;
    public TMP_Text reason;

    private void Start()
    {
        errorType = GameObject.Find("ErrorSwitcher").GetComponent<ErrorType>();
        reason.text = errorType.ErrorReason;
    }

    public void OnReturnToLobby()
    {
        Destroy(errorType.gameObject);
        SceneManager.LoadSceneAsync(0);
    }
}
