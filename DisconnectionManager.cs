using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DisconnectionManager : MonoBehaviourPunCallbacks
{
    GameObject errorType;

    private void Start()
    {
        errorType = GameObject.Find("ErrorSwitcher");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (!cause.ToString().Equals("DisconnectByClientLogic", System.StringComparison.InvariantCultureIgnoreCase))
        {
            errorType.GetComponent<ErrorType>().OnErrorGetted(cause.ToString());
        }
    }
}
