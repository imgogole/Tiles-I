using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMinimapPlayer : MonoBehaviour
{
    public GameServerManager GSM;
    public GameObject minimap;

    private void LateUpdate()
    {
        if (GSM.isInGame && !GSM.isGameEnded)
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                minimap.SetActive(!minimap.activeSelf);
            }
            GameObject player = GameObject.Find("Mine");
            Vector3 newPos = player.transform.position;
            newPos.y = transform.position.y;
            transform.position = newPos;
        }
        else
        {
            minimap.SetActive(false);
        }
    }
}
