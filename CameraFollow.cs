using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public GameServerManager GSM;

    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    private void Update()
    {
        if (!GSM.isGameEnded)
        {
            target = GameObject.Find("Mine").GetComponent<Transform>();
        }
        if (!GSM.isInGame)
        {
            offset = new Vector3(0, 16, -7);
        }
        else
        {
            offset = new Vector3(0, 16, -5.5f);
        }
    }

    private void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothPosition;
    }
}
